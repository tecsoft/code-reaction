using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReaction.Domain.Commits
{
    public class CommitDiff
    {
        public long Revision { get; set; }
        public IList<FileDiff> FileDiffs { get; set; }

        public CommitDiff(long revision)
        {
            Revision = revision;
            FileDiffs = new List<FileDiff>();
        }

        public FileDiff AddFileDiff(FileDiff fileDiff)
        {
            FileDiffs.Add(fileDiff);
            return fileDiff;
        }
            
    }
}
