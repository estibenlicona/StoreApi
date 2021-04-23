using System;

namespace AppStore.Core.Entities
{
    public partial class Product : BaseEntity
    {
        public string Name { get; set; }
        public int Stock { get; set; }
        public DateTime Date { get; set; }
        public string ImageUrl { get; set; }
        public float Price { get; set; }
        public bool IsActive { get; set; }
    }
}
