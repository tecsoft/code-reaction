using CodeReaction.Domain.Entities;
using CodeReaction.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReaction.Domain.Services
{
    public class LikeService
    {
        UnitOfWork unitOfWork;
        public LikeService(UnitOfWork aUnitOfWork)
        {
            unitOfWork = aUnitOfWork;
        }

        public void LikeFile(string user, int revision, string file)
        {
            unitOfWork.Context.Likes.Add(
                    new Like() { User = user, Revision = revision, File = file });
        }

        public void LikeLine(string user, int revision, string fileId, string lineId)
        {
            unitOfWork.Context.Likes.Add(
                    new Like() { User = user, Revision = revision, File = fileId, LineId = lineId });
        }

        public IEnumerable<Like> GetLikes(long revision)
        {
            return unitOfWork.Context.Likes.Where(l => l.Revision == revision);
        }
    }
}
