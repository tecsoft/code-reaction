using CodeReaction.Domain.Commits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeReaction.Web.RevisionDetail
{
    public class CommitsModel
    {
        public IEnumerable<CommitModel> Commits { get; set; }

        public CommitsModel(IEnumerable<Tuple<Commit, CommitStats>> details)
        {
            Commits = details.Select(detail => new CommitModel(detail.Item1, detail.Item2)).ToList();
        }
    }

    public class CommitModel
    {
        public long Revision { get; set; }
        public string Author { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public int NumberReviewers { get; set; }
        public int NumberComments { get; set; }
        public int NumberReplies { get; set; }
        public int NumberLikes { get; set; }
        public bool CanApprove { get; set; }
        public string ApprovedBy { get; set; }

        public CommitModel(Commit commit, CommitStats stats)
        {
            Revision = commit.Revision;
            Author = commit.Author;
            Message = commit.Message;
            Timestamp = commit.Timestamp;
            NumberReviewers = stats.NumberReviewers;
            NumberComments = stats.NumberComments;
            NumberReplies = stats.NumberReplies;
            NumberLikes = stats.NumberLikes;
            ApprovedBy = commit.ApprovedBy;
        }
    }
}