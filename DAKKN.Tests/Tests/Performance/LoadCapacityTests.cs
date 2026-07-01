using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace DAKKN.Tests.Tests.Performance
{
    public class LoadCapacityTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;

        public LoadCapacityTests(CustomWebApplicationFactory factory, ITestOutputHelper output)
        {
            _output = output;
            // Disable rate limiting for these tests by configuring the factory
            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.Configure<Microsoft.AspNetCore.RateLimiting.RateLimiterOptions>(options =>
                    {
                        options.GlobalLimiter = null;
                    });
                });
            }).CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = true,
                HandleCookies = true,
                BaseAddress = new Uri("http://localhost")
            });
        }

        [Fact]
        public async Task Run_Load_Test_And_Find_Maximum_Capacity()
        {
            var routesToTest = new[]
            {
                (Route: "/Home/Privacy", Name: "Privacy Page (Static Baseline)"),
                (Route: "/shop/products", Name: "Shop Products (Heavy SQL Query)")
            };

            var concurrencyLevels = new[] { 5, 25, 50, 100, 200 };
            var duration = TimeSpan.FromSeconds(5); // 5 seconds per test run

            var sb = new StringBuilder();
            sb.AppendLine("# DAKKN Server Load Capacity Report");
            sb.AppendLine();
            sb.AppendLine($"Test executed at: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"Duration per run: {duration.TotalSeconds} seconds");
            sb.AppendLine("This report determines the maximum concurrent user capacity of the DAKKN server by analyzing throughput (RPS), latencies, and error rates.");
            sb.AppendLine();

            foreach (var routeInfo in routesToTest)
            {
                sb.AppendLine($"## Target: {routeInfo.Name} (`{routeInfo.Route}`)");
                sb.AppendLine();
                sb.AppendLine("| Concurrency (Users) | Total Requests | Throughput (RPS) | Avg Latency (ms) | P95 Latency (ms) | Success Rate | Status |");
                sb.AppendLine("|---------------------|----------------|------------------|------------------|------------------|--------------|--------|");

                _output.WriteLine($"Starting load test for target: {routeInfo.Name} ({routeInfo.Route})");

                // Warm up
                await RunLoadIterationAsync(routeInfo.Route, 5, TimeSpan.FromSeconds(2));

                foreach (var concurrency in concurrencyLevels)
                {
                    _output.WriteLine($"Running with concurrency: {concurrency} users...");
                    var result = await RunLoadIterationAsync(routeInfo.Route, concurrency, duration);

                    var status = "Stable";
                    if (result.SuccessRate < 95.0)
                    {
                        status = "Degraded (Errors)";
                    }
                    else if (result.P95Latency > 1500)
                    {
                        status = "Degraded (High Latency)";
                    }
                    else if (concurrency > 50 && result.Throughput < 10) // arbitrary low throughput check
                    {
                        status = "Unstable";
                    }

                    sb.AppendLine($"| {concurrency,-19} | {result.TotalRequests,-14} | {result.Throughput,-16:F1} | {result.AvgLatency,-16:F1} | {result.P95Latency,-16:F1} | {result.SuccessRate,-12:F1}% | {status} |");
                }
                sb.AppendLine();
            }

            // Print the report to the test output
            _output.WriteLine(sb.ToString());

            // Save report to disk in the solution directory
            var reportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "load_capacity_report.md");
            await File.WriteAllTextAsync(reportPath, sb.ToString());
            _output.WriteLine($"Detailed markdown report saved to: {reportPath}");
        }

        private async Task<IterationResult> RunLoadIterationAsync(string route, int concurrency, TimeSpan duration)
        {
            var latencies = new ConcurrentBag<double>();
            var successCount = 0;
            var failureCount = 0;

            var cts = new CancellationTokenSource(duration);
            var token = cts.Token;

            var tasks = new List<Task>();
            var stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < concurrency; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var taskClient = _client; // Share client safely
                    var taskStopwatch = new Stopwatch();

                    while (!token.IsCancellationRequested)
                    {
                        taskStopwatch.Restart();
                        try
                        {
                            using var response = await taskClient.GetAsync(route, token);
                            taskStopwatch.Stop();

                            var elapsed = taskStopwatch.Elapsed.TotalMilliseconds;
                            latencies.Add(elapsed);

                            if (response.IsSuccessStatusCode)
                            {
                                Interlocked.Increment(ref successCount);
                            }
                            else
                            {
                                Interlocked.Increment(ref failureCount);
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            // Expected when token cancels
                            break;
                        }
                        catch (Exception)
                        {
                            taskStopwatch.Stop();
                            latencies.Add(taskStopwatch.Elapsed.TotalMilliseconds);
                            Interlocked.Increment(ref failureCount);
                        }
                    }
                }));
            }

            try
            {
                await Task.WhenAll(tasks);
            }
            catch (Exception)
            {
                // Ignore task cancellation exceptions
            }

            stopwatch.Stop();
            var actualDurationSec = stopwatch.Elapsed.TotalSeconds;

            var total = successCount + failureCount;
            if (total == 0)
            {
                return new IterationResult(0, 0, 0, 0, 0);
            }

            var sortedLatencies = latencies.ToArray();
            Array.Sort(sortedLatencies);

            var avgLatency = sortedLatencies.Length > 0 ? sortedLatencies.Average() : 0;
            var p95Index = (int)(sortedLatencies.Length * 0.95);
            var p95Latency = sortedLatencies.Length > 0 ? sortedLatencies[Math.Min(p95Index, sortedLatencies.Length - 1)] : 0;

            var throughput = total / actualDurationSec;
            var successRate = ((double)successCount / total) * 100.0;

            return new IterationResult(total, throughput, avgLatency, p95Latency, successRate);
        }

        private record IterationResult(
            int TotalRequests,
            double Throughput,
            double AvgLatency,
            double P95Latency,
            double SuccessRate);
    }
}
