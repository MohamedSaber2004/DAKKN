using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAKKN.Application.DTOs;
using DAKKN.Application.Interfaces;

namespace DAKKN.Application.Services
{
    public class MockDashboardService : IDashboardService
    {
        public Task<DashboardDto> GetCustomerDashboardDataAsync(Guid customerId)
        {
            var imageUrl = "https://lh3.googleusercontent.com/aida/AP1WRLvk0o2fHlsgnzmZ9xrrRAnUTq9hl5CW063LX44OhYiXj0ZAadSE46fIAmtHQKBEzfk8NKsMl6AwohhVGiDgxsbst33TGFkDW68Y9_d3C1Cvf-mtviXqQYHM-Aqnsunu3Q6Yy2-9A5mXcsO80D173UvbE44Odz0GI41kQbHK_cLtDiruHCKTT15snGH5XT7Y6MLMfFd4rUCTY71ZJwiOba19bKW0Du2pMXYtJtF-I-h5V-wypv1z0Vztp_M";

            var data = new DashboardDto
            {
                LastOrder = new OrderDto
                {
                    Id = Guid.NewGuid(),
                    OrderNumber = "DK-9021",
                    Status = "Shipped",
                    ItemCount = 3,
                    OrderDate = DateTime.Now.AddDays(-2),
                    TotalAmount = 210
                },
                Recommendations = new List<ProductDto>
                {
                    new ProductDto { Id = Guid.NewGuid(), Name = "Cyberpunk Ramen", Price = 85, ImageUrl = imageUrl, CategoryName = "Gaming" },
                    new ProductDto { Id = Guid.NewGuid(), Name = "Neon Skull", Price = 80, ImageUrl = imageUrl, CategoryName = "Anime" },
                    new ProductDto { Id = Guid.NewGuid(), Name = "Clean Code", Price = 60, ImageUrl = imageUrl, CategoryName = "Tech" },
                    new ProductDto { Id = Guid.NewGuid(), Name = "Git Master", Price = 60, ImageUrl = imageUrl, CategoryName = "Tech" },
                    new ProductDto { Id = Guid.NewGuid(), Name = "Docker Whale", Price = 70, ImageUrl = imageUrl, CategoryName = "Tech" },
                    new ProductDto { Id = Guid.NewGuid(), Name = "Doge Much Wow", Price = 50, ImageUrl = imageUrl, CategoryName = "Memes" }
                }
            };

            return Task.FromResult(data);
        }
    }
}
