using CodeReaction.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace CodeReaction.Domain.Feedback
{
    public class LikeQuery
    {
        public long? Revision { get; set; }
        public long? LikeId { get; set; }

        private IQueryable<Comment> query;

        public LikeQuery(IQueryable<Comment> likes)
        {
            query = likes;
        }

        public IEnumerable<Comment> Execute()
        {
            if (Revision.HasValue)
            {
                query = query.Where(c => c.Revision == Revision && c.IsLike);
            }

            if (LikeId.HasValue)
            {
                query = query.Where(c => c.Id == LikeId && c.IsLike);
            }

            return query.ToList();
        }
    }
}
