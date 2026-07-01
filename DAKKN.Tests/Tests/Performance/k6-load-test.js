import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
  // Test scenario with sequential stages to scale concurrent users (VUs) 
  stages: [
    { duration: '1m', target: 10 },  // Baseline with 10 users
    { duration: '2m', target: 50 },  // Scale to 50 users (expected db degradation threshold)
    { duration: '2m', target: 100 }, // Scale to 100 users
    { duration: '2m', target: 200 }, // Scale to 200 users (heavy load)
    { duration: '2m', target: 300 }, // Scale to 300 users (stress testing)
    { duration: '1m', target: 0 },   // Cool down to 0 users
  ],
  thresholds: {
    // Assert that the error rate remains under 5% throughout the entire test
    http_req_failed: ['rate<0.05'],
    // Assert that 90% of requests complete in less than 1.5 seconds (1500ms)
    http_req_duration: ['p(90)<1500'],
  },
};

export default function () {
  // Configurable base URL via environment variable TARGET_URL
  const baseUrl = __ENV.TARGET_URL || 'http://localhost:5218';

  // Define requests to test
  const routes = [
    { url: `${baseUrl}/Home/Privacy`, name: 'Privacy Page' },
    { url: `${baseUrl}/shop/products`, name: 'Shop Products' },
  ];

  for (const route of routes) {
    const res = http.get(route.url);
    
    // Validate responses
    check(res, {
      [`${route.name} status is 200`]: (r) => r.status === 200,
      [`${route.name} response time < 1.5s`]: (r) => r.timings.duration < 1500,
    });

    // Simulate 1 second user think time between actions
    sleep(1);
  }
}
