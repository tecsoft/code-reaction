﻿using CodeReaction.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReaction.Domain.Services
{
    public class CommentService
    {
        UnitOfWork unitOfWork;
        public CommentService(UnitOfWork aUnitOfWork)
        {
            unitOfWork = aUnitOfWork;
        }

        public void CommentLine(string user, int revision, int? fileId, string lineId, string text)
        {
            unitOfWork.Context.Comments.Add(
                    new Comment() { User = user, Revision = revision, FileId = fileId, LineId = lineId, Text = text });
        }

        public void Reply(long idComment, string author, string text)
        {
            Comment originalComment = unitOfWork.Context.Comments.First(c => c.Id == idComment);

            var reply = new Comment()
            {
                FileId = originalComment.FileId,
                LineId = originalComment.LineId,
                Revision = originalComment.Revision,
                User = author,
                Text = text,
                ReplyToId = idComment
            };

            unitOfWork.Context.Comments.Add( reply );
        }
    }
}
