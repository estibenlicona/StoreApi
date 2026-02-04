using System;

namespace AppStore.Core.Entities
{
    public partial class Product : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public int Stock { get; set; }
        public DateTime Date { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public float Price { get; set; }
        public bool IsActive { get; set; }
    }
}
