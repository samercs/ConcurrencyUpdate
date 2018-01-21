using System;
using System.ComponentModel.DataAnnotations;

namespace ConcurrencyUpdate.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Budget { get; set; }
        public DateTime StartDate { get; set; }
        public string Administrator { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}