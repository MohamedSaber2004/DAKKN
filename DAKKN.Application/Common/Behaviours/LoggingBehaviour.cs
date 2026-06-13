using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace DAKKN.Application.Common.Behaviours
{
    public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;

        public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;

            // Safe logging to avoid System.Text.Json errors during destructuring
            _logger.LogInformation("Handling Request: {Name}", requestName);

            try 
            {
                // We avoid {@Request} here if it's causing serialization issues with certain types
                var jsonRequest = JsonSerializer.Serialize(request, new JsonSerializerOptions 
                { 
                    WriteIndented = false,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                });
                _logger.LogInformation("Request Body: {JSON}", jsonRequest);
            }
            catch 
            {
                _logger.LogInformation("Request Body: [Serialization Failed]");
            }

            var response = await next();

            _logger.LogInformation("Handled Request: {Name}", requestName);

            return response;
        }
    }
}
