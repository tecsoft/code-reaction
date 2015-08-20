using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeReaction.Domain.Commits
{
    public class CommitStats
    {
        public CommitStats( int nbReviewers, int nbComments, int nbReplies )
        {
            NumberReviewers = nbReviewers;
            NumberComments = nbComments;
            NumberReplies = nbReplies;
        }
        public int NumberReviewers { get; private set; }
        public int NumberComments { get; private set; }
        public int NumberReplies { get; private set; }
    }
}
