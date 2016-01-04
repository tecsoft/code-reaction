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

        public void ApproveCommit( int revision, string author )
        {
            var commit = unitOfWork.Context.Commits.Single( c => c.Revision == revision);
            commit.ApprovedBy = author;
        }
    }
}
