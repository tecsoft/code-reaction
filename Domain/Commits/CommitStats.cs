namespace CodeReaction.Domain.Commits
{
    public class CommitStats
    {
        public CommitStats( int nbReviewers, int nbComments, int nbReplies, int nbLikes )
        {
            NumberReviewers = nbReviewers;
            NumberComments = nbComments;
            NumberReplies = nbReplies;
            NumberLikes = nbLikes;
        }
        public int NumberReviewers { get; private set; }
        public int NumberComments { get; private set; }
        public int NumberReplies { get; private set; }
        public int NumberLikes { get; private set; }
    }
}
