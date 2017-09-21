using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReaction.Domain.Database
{
    public class SchemaVersion
    {
        [Key]
        public int Number { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
