using DAKKN.Domain.Common;
using System.Collections.Generic;

namespace DAKKN.Domain.Entities
{
    public class Category : BaseEntity<Guid>
    {
        public string CategoryName { get; set; } = string.Empty;
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
