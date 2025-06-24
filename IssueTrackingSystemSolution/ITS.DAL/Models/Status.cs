using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITS.DAL.Models
{
    public class Status
    {
        public int Id { get; set; }
        public string Name { get; set; } // Open, In Progress, Resolved, Closed

        public virtual ICollection<Issue> Issues { get; set; } = new List<Issue>();
    }
}
