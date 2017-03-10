using CodeReaction.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace CodeReaction.Domain.Feedback
{
    public class CommentQuery
    {
        public long? Revision { get; set;  }

        private IQueryable<Comment> query;

        public CommentQuery( IQueryable<Comment>  comments)
        {
            query = comments;    
        }

        public IEnumerable<Comment> Execute()
        {
            if ( Revision.HasValue )
            {
                query = query.Where(c => c.Revision == Revision && !c.IsLike);
            }

            return query.ToList();
        }
    }
}
