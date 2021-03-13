using System;
using System.ComponentModel.DataAnnotations;

namespace HealthEdgeApi.Model
{
    public class InventoryItem
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        public string GetKeyName() => Name.ToLower();
    }
}