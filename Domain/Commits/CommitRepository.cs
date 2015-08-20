using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CodeReaction.Domain.Commits
{
    public class CommitRepository
    {
        public IEnumerable<Commit> GetSince(long revision, int maxNumber)
        {
            CommitReader reader = new CommitReader(@"d:\travail\trunk");

            return reader.GetLatestLogs(revision, maxNumber);
        }

        public CommitDiff GetRevision( long revision)
        {
            CommitReader reader = new CommitReader(@"d:\travail\trunk");

            var diffs = reader.GetRevisionDiffs(revision);

            return diffs;
        }
    }
}
