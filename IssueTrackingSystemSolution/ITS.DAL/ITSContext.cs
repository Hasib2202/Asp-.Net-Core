using ITS.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITS.DAL
{
    public class ITSContext : DbContext
    {
        public ITSContext() : base("name=ITSConnection") { }

        public DbSet<User> Users { get; set; }
        public DbSet<Issue> Issues { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Token> Tokens { get; set; }
    }
}
