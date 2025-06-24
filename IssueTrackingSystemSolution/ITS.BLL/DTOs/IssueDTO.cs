using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITS.BLL.DTOs
{
    public class IssueDTO
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Type { get; set; } // Bug or Feature

        public string Description { get; set; }

        public int StatusId { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }

        public string AttachmentPath { get; set; }
    }

    public class IssueFilterDTO
    {
        public string TitleKeyword { get; set; }
        public int? StatusId { get; set; }
        public int? CreatedByUserId { get; set; }
        public string Type { get; set; } // Bug or Feature
        public DateTime? CreatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }
    }

}
