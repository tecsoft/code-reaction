using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeReaction.Domain.Feedback;

namespace CodeReaction.Domain.Entities
{
    public class Like : IAnnotation
    {
        [Key]
        public long Id { get; set; }
        public string User { get; set; }
        public long Revision { get; set; }
        public string File { get; set; }
        public string LineId { get; set; }

    }
}
