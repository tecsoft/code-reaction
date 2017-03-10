using CodeReaction.Domain.Entities;

namespace CodeReaction.Domain.Services
{
    public class LikeService
    {
        UnitOfWork unitOfWork;
        public LikeService(UnitOfWork aUnitOfWork)
        {
            unitOfWork = aUnitOfWork;
        }

        public Comment LikeLine(string user, int revision, string lineId)
        {
            var like = new Comment { User = user, Revision = revision, LineId = lineId, IsLike = true };
            unitOfWork.Context.Comments.Add(like);
            return like;
        }
    }
}