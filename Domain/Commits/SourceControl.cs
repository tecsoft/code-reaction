using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CodeReaction.Domain.Commits
{
    public class SourceControl : ISourceControl
    {
        public IEnumerable<Commit> GetSince(long revision, int maxNumber)
        {
            SvnLogReader reader = new SvnLogReader();

            return reader.GetLatestLogs(revision, maxNumber);
        }

        public CommitDiff GetRevision( long revision)
        {
            SvnLogReader reader = new SvnLogReader();

            var diffs = reader.GetRevisionDiffs(revision);

            return diffs;
        }
    }
}
