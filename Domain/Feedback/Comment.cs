using CodeReaction.Domain.Feedback;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReaction.Domain.Entities
{
    public class Comment : IAnnotation
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string User { get; set; }

        [Required]
        public long Revision { get; set; }

        public string File { get; set; }

        public string LineId { get; set; }

        //[Required]
        public string Text { get; set; }

        //public bool IsLike { get; set; }

        public long? ReplyToId { get; set; }

        [ForeignKey("ReplyToId")]
        public Comment ReplyTo { get; set; }

        //[InverseProperty("ReplyTo")]
        public virtual ICollection<Comment> Replies { get; set; }

        public DateTime? Timestamp { get; set; }
    }
}
