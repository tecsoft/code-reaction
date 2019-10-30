using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReaction.Domain.Projects
{
    public class Project
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public bool IsDefault { get; set; }
    }
}
