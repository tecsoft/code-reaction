using System.Collections.Generic;

namespace CodeReaction.Domain.Commits
{
    public interface ISourceControl
    {
        CommitDiff GetRevision(long revision);
        IEnumerable<Commit> GetSince(long revision, int maxNumber);
    }
}