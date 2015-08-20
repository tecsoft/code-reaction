using CodeReaction.Domain.Feedback;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReaction.Domain.Entities
{
    public class Comment : IAnnotation
    {
        [Key]
        public long Id { get; set; }
        public string User { get; set; }
        public long Revision { get; set; }
        public int? FileId { get; set; }
        public string LineId { get; set; }
        public string Text { get; set; }
        public long? ReplyToId { get; set; }
    }
}
