using System;
using System.Collections.Generic;
using System.Text;

namespace testApp.Data.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public int? Age { get; set; }
        public IEnumerable<Order>? Orders { get; set; }
    }
}
