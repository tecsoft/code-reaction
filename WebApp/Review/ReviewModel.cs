using CodeReaction.Domain;
using CodeReaction.Domain.Commits;
using CodeReaction.Domain.Entities;
using CodeReaction.Domain.Feedback;
using CodeReaction.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeReaction.Web.Models
{
    public class CommentModel
    {
        public long Id { get; set; }
        public string Author { get; set; }
        public string Text { get; set; }
        public string FileId { get; set; }
        public string LineId { get; set; }
        public IList<CommentModel> Replies { get; set; }
        public DateTime? Timestamp { get; set; }
    }

    public class LineModel
    {
        public long Revision { get; set; }
        public string Id { get; set; }
        public string File { get; set; }
        public string Text { get; set; }
        public ChangeState ChangeState { get; set; }
        public int RemovedLineNumber { get; set; }
        public int AddedLineNumber { get; set; }
        public IList<CommentModel> Comments { get; set; }
        public IList<string> Likes { get; set; }
        public string Author { get; set; }
    }
    public class FileModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ModText { get; set; }
        public IList<LineModel> Lines { get; set; }
        public long Revision { get; set; }
    }
    public class ReviewModel
    {
        public long Revision { get; set; }
        public string Author { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public string ApprovedBy { get; set; }
        public IList<CommentModel> CommitComments { get; set; }
        public IList<FileModel> Files { get; set; }
    }

    public class ModelBuilder
    {
        SourceControl _sourceControl;
        DbCodeReview _database;
        public ModelBuilder( SourceControl sourceControl, DbCodeReview database )
        {
            _sourceControl = sourceControl;
            _database = database;
        }

        public ReviewModel Build( long revision )
        {
            var sourceCode = _sourceControl.GetRevision(revision);
            var commit = _database.Commits.FirstOrDefault( c => c.Revision == revision);
            var comments = _database.Comments.Where(comment => comment.Revision == revision).ToList();

            var model = new ReviewModel
            {
                Author = commit.Author,
                Message = commit.Message,
                Revision = commit.Revision,
                Timestamp = commit.Timestamp,
                ApprovedBy = commit.ApprovedBy,
                Files = new List<FileModel>(),
            };

           var commitComments = comments
                .Where( c => c.ReplyTo == null && string.IsNullOrEmpty(c.File) /*&& !c.IsLike*/)
                .OrderBy(c => c.Id);

            Func<Comment, CommentModel> recursiveConvert = default(Func<Comment, CommentModel>);
            recursiveConvert = (comment) =>
            {
                var converted = new CommentModel
                {
                    Id = comment.Id,
                    Author = comment.User,
                    Text = comment.Text,
                    FileId = comment.File,
                    LineId = comment.LineId,
                    Timestamp = comment.Timestamp,
                    Replies = new List<CommentModel>()
                };

                foreach( var reply in comment.Replies )
                {
                    converted.Replies.Add(recursiveConvert(reply));
                }
                return converted;
            };

            model.CommitComments = commitComments.Select(c => recursiveConvert(c)).ToList();

            foreach (var fileDiff in sourceCode.FileDiffs.OrderBy(f => f.Name))
            {
                var fileModel = new FileModel
                {
                    Name = fileDiff.Name,
                    ModText = fileDiff.FileState.ToString(),
                    Lines = new List<LineModel>(),
                    Revision = revision
                };

                model.Files.Add(fileModel);

                foreach (var lineDiff in fileDiff.Lines)
                {
                    var lineModel = new LineModel
                    {
                        Id = lineDiff.Id,
                        Revision = revision,
                        File = fileModel.Name,
                        Text = lineDiff.Text,
                        AddedLineNumber = lineDiff.AddedLineNumber,
                        RemovedLineNumber = lineDiff.RemovedLineNumber,
                        ChangeState = lineDiff.Changed,
                        Comments = new List<CommentModel>(),
                        Likes = new List<string>(),
                        Author = model.Author,
                    };

                    fileModel.Lines.Add(lineModel);

                    var lineComments = comments
                        .Where(c => c.ReplyTo == null && c.LineId == lineModel.Id && c.File == lineModel.File)
                        .OrderBy(c => c.Id);

                    lineModel.Comments = lineComments/*.Where( c => ! c.IsLike )*/.Select(c => recursiveConvert(c)).ToList();

                    lineModel.Likes = lineComments/*.Where(c => c.IsLike)*/.Select(c => c.User).ToList();
                }
            }

            return model;
        }
    }
}