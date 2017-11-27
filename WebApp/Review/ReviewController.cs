using CodeReaction.Domain;
using CodeReaction.Domain.Commits;
using CodeReaction.Domain.Entities;
using CodeReaction.Domain.Feedback;
using CodeReaction.Domain.Services;
using CodeReaction.Web.Auth;
using CodeReaction.Web.Models;
using CodeReaction.Web.RevisionDetail;
using CodeReaction.Web.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace CodeReaction.Web.Controllers
{
    [Authorize]
    public class ReviewController : ApiController
    {
        
        [Route("api/review/revision/{revision}")]
        public IHttpActionResult GetRevision(long revision)
        {
            ReviewModel model = null;

            UnitOfWork unitOfWork = null;

            try
            {
                unitOfWork = new UnitOfWork();

                var builder = new ModelBuilder(new SourceControl(), unitOfWork.Context);

                model = builder.Build(revision);

                return Ok(model);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("GetRevision: " + ex);
                return InternalServerError(ex);
            }
            finally
            {
                if (unitOfWork != null)
                    unitOfWork.Dispose();
            }

            
        }

        [Route("api/review/comment/{user}/{revision}")]
        public IHttpActionResult ReviewCommit(string user, int revision)
        {
            UnitOfWork unitOfWork = null;
            Comment comment = null;
            try
            {
                unitOfWork = new UnitOfWork();

                var parameters = this.Request.GetQueryNameValuePairs().FirstOrDefault(i => i.Key == "comment");

                comment = new CommentService(unitOfWork).CommentLine(user, revision, null, null, parameters.Value);

                unitOfWork.Save();

                return Ok(comment.Id);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("ReviewCommit: " + ex);
                return InternalServerError(ex);
            }
            finally
            {
                if (unitOfWork != null)
                    unitOfWork.Dispose();
            }
            
        }

        [Route("api/review/comment/{user}/{revision}/{lineId}")]
        public IHttpActionResult CommentLine(string user, int revision, string lineId)
        {
            UnitOfWork unitOfWork = null;
            Comment comment = null;
            try
            {
                unitOfWork = new UnitOfWork();

                var parameters = this.Request.GetQueryNameValuePairs();
                var text = parameters.FirstOrDefault(i => i.Key == "comment");
                var file = parameters.FirstOrDefault(i => i.Key == "file");

                comment = new CommentService(unitOfWork).CommentLine(user, revision, file.Value, lineId, text.Value);

                unitOfWork.Save();

                return Ok(comment.Id);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("CommentLine: " + ex);
                return InternalServerError(ex);
            }
            finally
            {
                if (unitOfWork != null)
                    unitOfWork.Dispose();
            }
            
        }

        [Route("api/review/reply/{idComment}/{author}")]
        public IHttpActionResult CommentLine(long idComment, string author)
        {
            UnitOfWork unitOfWork = null;
            Comment comment = null;
            try
            {
                unitOfWork = new UnitOfWork();

                var parameters = this.Request.GetQueryNameValuePairs().FirstOrDefault(i => i.Key == "comment");

                comment = new CommentService(unitOfWork).Reply( idComment, author, parameters.Value );

                unitOfWork.Save();

                return Ok(comment.Id);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("CommentLine: " + ex);
                return InternalServerError(ex);
            }
            finally
            {
                if (unitOfWork != null)
                    unitOfWork.Dispose();
            }
            
        }

        [Route("api/review/approve/{revision}/{approver}")]
        public IHttpActionResult ApproveRevision( int revision, string approver )
        {
            UnitOfWork unitOfWork = null;
            try
            {
                unitOfWork = new UnitOfWork();

                new CommitService(unitOfWork).ApproveCommit(revision, approver);
                unitOfWork.Save();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("ApproveRevision: " + ex);
                return InternalServerError(ex);
            }
            finally
            {
                if (unitOfWork != null)
                    unitOfWork.Dispose();
            }

            return Ok();
        }

        [Route("api/review/file/{revision}")]
        public IHttpActionResult GetCompleteFile(long revision)
        {
            IList<string> viewModel = null;

            try
            {
                var parameters = this.Request.GetQueryNameValuePairs().FirstOrDefault(i => i.Key == "filename");

                viewModel = new SourceControl().GetFile(revision, parameters.Value);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("GetRevision: " + ex);
                return InternalServerError(ex);
            }

            return Ok(viewModel);
        }

        
    }
}
