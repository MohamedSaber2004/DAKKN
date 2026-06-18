using System;

namespace DAKKN.Application.DTOs
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string ArName { get; set; } = string.Empty;
        public int ProductsCount { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class CreateCategoryDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public string ArName { get; set; } = string.Empty;
    }

    public class UpdateCategoryDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public string ArName { get; set; } = string.Empty;
    }
}
