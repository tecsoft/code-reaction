using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReaction.Domain.Commits
{
    public enum FileState
    {
        None,
        Added,
        Deleted,
        Moved,
        Modified,
    }

    public enum FileType
    {
        None,
        Text,
        Binary,
        Directory
    }
}
