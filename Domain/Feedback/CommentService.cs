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

        public Comment CommentLine(string user, int revision, string file, string lineId, string text)
        {
            Comment newComment = new Comment()
            {
                User = user,
                Revision = revision,
                File = file,
                LineId = lineId,
                Text = text,
                Timestamp = DateTime.UtcNow
            };

            unitOfWork.Context.Comments.Add(newComment);
            return newComment;
        }

        public Comment Reply(long idComment, string author, string text)
        {
            Comment originalComment = unitOfWork.Context.Comments.First(c => c.Id == idComment);

            var reply = new Comment()
            {
                File = originalComment.File,
                LineId = originalComment.LineId,
                Revision = originalComment.Revision,
                User = author,
                Text = text,
                ReplyToId = idComment,
                Timestamp = DateTime.UtcNow
            };

            return unitOfWork.Context.Comments.Add( reply );

            //return reply;
        }
    }
}
