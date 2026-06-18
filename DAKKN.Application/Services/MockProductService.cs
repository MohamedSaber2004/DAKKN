using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAKKN.Application.DTOs;
using DAKKN.Application.Interfaces;

namespace DAKKN.Application.Services
{
    public class MockProductService : IProductService
    {
        public Task<ProductDto?> GetProductByIdAsync(Guid id)
        {
            var product = new ProductDto
            {
                Id = id,
                Name = "Retro Cassette",
                Description = "Nostalgic 80s cassette tape design with a modern holographic finish. Engineered with 5-layer industrial vinyl for extreme durability.",
                Price = 70,
                AverageRating = 4.9,
                ReviewCount = 120,
                ImageUrl = "https://lh3.googleusercontent.com/aida/AP1WRLvk0o2fHlsgnzmZ9xrrRAnUTq9hl5CW063LX44OhYiXj0ZAadSE46fIAmtHQKBEzfk8NKsMl6AwohhVGiDgxsbst33TGFkDW68Y9_d3C1Cvf-mtviXqQYHM-Aqnsunu3Q6Yy2-9A5mXcsO80D173UvbE44Odz0GI41kQbHK_cLtDiruHCKTT15snGH5XT7Y6MLMfFd4rUCTY71ZJwiOba19bKW0Du2pMXYtJtF-I-h5V-wypv1z0Vztp_M",
                FinishOptions = new List<string> { "Holographic", "Matte Vinyl", "Glossy" },
                SizeOptions = new List<string> { "3\"", "4\"", "5\"" },
                CategoryName = "Gaming"
            };

            return Task.FromResult<ProductDto?>(product);
        }
    }
}
