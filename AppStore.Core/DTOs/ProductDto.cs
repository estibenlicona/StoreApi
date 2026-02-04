using System;
using System.Collections.Generic;
using System.Text;

namespace AppStore.Core.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Stock { get; set; }
        public DateTime Date { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public float Price { get; set; }
        public bool IsActive { get; set; }
    }
}
