using System.Data.Entity;

namespace ConcurrencyUpdate.Models
{
    public class MyContext : DbContext
    {
        public MyContext() : base("DefaultConnection")
        {

        }

        public IDbSet<Department> Departments { get; set; }
    }
}