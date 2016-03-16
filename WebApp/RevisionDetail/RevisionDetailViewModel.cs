using CodeReaction.Domain;
using CodeReaction.Domain.Commits;
using CodeReaction.Domain.Entities;
using CodeReaction.Domain.Feedback;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeReaction.Web.Models
{
    public class RevisionDetailViewModel
    {
        public IEnumerable<RevisedFileDetailViewModel> RevisedFileDetails { get; set; }

        public long Revision { get; set; }
        public string Author { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp{ get; set; }
        public string ApprovedBy { get; set; }
        public IEnumerable<CommentViewModel> Reviews { get; set; }

        


        public static RevisionDetailViewModel Create(Commit commit, CommitDiff commitDiff, IEnumerable<Like> likes, IEnumerable<Comment> comments)
        {
            RevisionDetailViewModel viewModel = new RevisionDetailViewModel()
            {
                RevisedFileDetails = 
                    commitDiff.FileDiffs
                        .OrderBy(fd => fd.Name)
                        .Select( fd => new RevisedFileDetailViewModel()
                                        {
                                            Filename = fd.Name,
                                            ModText = fd.FileState.ToString(),
                                            LikedBy = new List<string>(),
                                            //LikedBy = likes
                                            //            .Where(ld => ld.FileId == fd.Index && string.IsNullOrEmpty(ld.LineId)== false)
                                            //            .Select(like => like.User).ToList(),

                                            LineDetails = fd.Lines
                                                .Select( ld => new LineDetailViewModel()
                                                            {
                                                                Text = ld.Text,
                                                                RemovedLineNumber = ld.RemovedLineNumber,
                                                                AddedLineNumber = ld.AddedLineNumber,
                                                                ChangeState = ld.Changed,
                                                                LineId = ld.Id,
                                                                LikedBy = likes.Where(like => MatchesLine(like, fd, ld))
                                                                                .Select(like => like.User).ToList(),
                                                                Comments = comments.Where( comment => MatchesLine(comment, fd, ld ) )
                                                                                .Select( comment => CommentViewModel.CreateFrom(comment) ).ToList()
                                                            } )
                                        }
                        )
            };

            viewModel.Revision = commit.Revision;
            viewModel.Author = commit.Author;
            viewModel.Message = commit.Message;
            viewModel.Timestamp = commit.Timestamp;
            viewModel.ApprovedBy = commit.ApprovedBy;
            viewModel.Reviews = comments.Where(comment => IsReview(comment)).Select(comment => CommentViewModel.CreateFrom(comment)).ToList();
            return viewModel; ;
        }

        static bool  MatchesLine( IAnnotation annotation, FileDiff fileDiff, LineDiff lineDiff )
        {
            return annotation.File == fileDiff.Name && annotation.LineId == lineDiff.Id;
        }

        static bool IsReview(IAnnotation annotation)
        {
            return string.IsNullOrEmpty( annotation.File ) && string.IsNullOrEmpty(annotation.LineId );
        }
    }

    public class RevisedFileDetailViewModel
    {
        public string Filename { get; set; }
        public string ModText { get; set; }
        public IEnumerable<string> LikedBy { get; set; }
        public IEnumerable<LineDetailViewModel> LineDetails { get; set; }
    }

    public class LineDetailViewModel
    {
        public string Text { get; set; }
        public ChangeState ChangeState { get; set; }
        public int RemovedLineNumber { get; set; }
        public int AddedLineNumber { get; set; }
        public string LineId { get; set; }
        public IEnumerable<string> LikedBy { get; set; }
        public IEnumerable<CommentViewModel> Comments { get; set; }
    }

    public class CommentViewModel
    {
        public long Id { get; private set; }
        public string Author { get; private set; }
        public string Comment { get; private set;  }
        public long? ReplyToId { get; private set; }
        public static CommentViewModel CreateFrom(Comment comment)
        {
            return new CommentViewModel() { Id = comment.Id, Author = comment.User, Comment = comment.Text, ReplyToId = comment.ReplyToId };
        }
    }

}