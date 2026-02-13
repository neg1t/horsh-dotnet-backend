using System;
using System.Collections.Generic;
using System.Text;

namespace testApp.Data.Models
{
    public class Order
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; } = "???";
        public User? User { get; set; }
        public int? UserId { get; set; }
    }
}
