using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReaction.Domain.Commits
{
    public class Commit
    {
        [Key]
        public long Id { get; set; }
        public long Revision { get; set; }
        public string Author { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; } 
        public string ApprovedBy { get; set; }
        public long? Project { get; set; }

        [NotMapped]
        public string[] Files { get; set; }
    }
}
