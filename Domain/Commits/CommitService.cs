using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReaction.Domain.Commits
{
    public class CommitService
    {
        UnitOfWork unitOfWork;

        public CommitService(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public int? GetLastKnownRevision()
        {
            return unitOfWork.Context.Commits.Max(c => (int?)c.Revision);
        }

        public int PersistLatestCommitData(int lastRevision)
        {
            var repos = new CommitRepository();

            var list = repos.GetSince(lastRevision, 20);

            int result = list.Count();

            if (result > 0)
            {
                unitOfWork.Context.Commits.AddRange(list);
            }

            return result;
        }

        public void ApproveCommit( int revision, string author )
        {
            var commit = unitOfWork.Context.Commits.Single( c => c.Revision == revision);
            commit.ApprovedBy = author;
        }
    }
}
