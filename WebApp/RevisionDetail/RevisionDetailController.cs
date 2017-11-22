using CodeReaction.Domain;
using CodeReaction.Domain.Commits;
using CodeReaction.Domain.Entities;
using CodeReaction.Domain.Feedback;
using CodeReaction.Domain.Services;
using CodeReaction.Web.Auth;
using CodeReaction.Web.Models;
using CodeReaction.Web.RevisionDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace CodeReaction.Web.Controllers
{
    [Authorize]
    public class RevisionDetailController : ApiController
    {
        /// <summary>
        /// Return all commits more recent than 
        /// </summary>summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/commits")]
        public IHttpActionResult GetCommits()
        {
            CommitViewModel vm;
            UnitOfWork unitOfWork = null;

            System.Diagnostics.Trace.TraceInformation("Get Commits");

            try
            {
                unitOfWork = new UnitOfWork();

                var parameters = this.Request.GetQueryNameValuePairs();
                CommitQuery query = new CommitQuery(unitOfWork.Context.Commits);
                var keyword = parameters.FirstOrDefault(p => p.Key == "keyword").Value;

                if (string.IsNullOrEmpty(keyword) == false)
                {
                    query.Keyword = parameters.FirstOrDefault(p => p.Key == "keyword").Value;
                }

                query.ExcludeApproved = String.Equals("true", parameters.FirstOrDefault(p => p.Key == "excludeApproved").Value, StringComparison.InvariantCulture);
                query.Max = ParseNullable<int>(parameters.FirstOrDefault(p => p.Key == "max").Value);
                query.IncludeAuthor = parameters.FirstOrDefault(p => p.Key == "include").Value;
                query.ExcludeAuthor = parameters.FirstOrDefault(p => p.Key == "exclude").Value;

                var list = new List<Tuple<Commit, CommitStats>>();
                foreach (var commit in query.Execute())
                {
                    var comments = unitOfWork.Context.Comments.Where(c => c.Revision == commit.Revision);

                    var replies = comments.Where(c => c.User == commit.Author);
                    var reviews = comments.Where(c => c.User != commit.Author);

                    var stats = new CommitStats(
                        reviews.Select(c => c.User).Distinct().Count(),
                        reviews.Count(),
                        replies.Count());

                    list.Add(new Tuple<Commit, CommitStats>(commit, stats));
                }
                vm = new CommitViewModel(list);
            }
            catch(Exception ex )
            {
                System.Diagnostics.Trace.TraceError("GetRevision: " + ex);
                return InternalServerError(ex);
            }
            finally
            {
                if (unitOfWork != null)
                    unitOfWork.Dispose();
            }

            return Ok(vm);  // try catch?
        }

        [Route("api/commits/revision/{revision}")]
        public IHttpActionResult GetRevision(long revision)
        {
            RevisionDetailViewModel viewModel = null;

            UnitOfWork unitOfWork = null;

            try
            {
                unitOfWork = new UnitOfWork();

                var commitDiff = new SourceControl().GetRevision(revision);

                var commentQuery = new CommentQuery(unitOfWork.Context.Comments)
                {
                    Revision = revision
                };

                var comments = commentQuery.Execute();

                // merge data to view model efficiently

                Commit commit = unitOfWork.Context.Commits.FirstOrDefault(c => c.Revision == revision);

                viewModel = RevisionDetailViewModel.Create(commit, commitDiff,comments);
            }
            catch( Exception ex )
            {
                System.Diagnostics.Trace.TraceError("GetRevision: " + ex);
                return InternalServerError(ex);
            }
            finally
            {
                if (unitOfWork != null)
                    unitOfWork.Dispose();
            }

            return Ok(viewModel);
        }

        [Route("api/commits/revision2/{revision}")]
        public IHttpActionResult GetRevision2(long revision)
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

        [Route("api/commits/comment/{user}/{revision}")]
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

        [Route("api/commits/comment/{user}/{revision}/{lineId}")]
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

        [Route("api/commits/reply/{idComment}/{author}")]
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

        [Route("api/commits/approve/{revision}/{approver}")]
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

        [Route("api/commits/file/{revision}")]
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

        T? ParseNullable<T>(string str) where T : struct
        {
            if (string.IsNullOrEmpty(str))
                return null;

            return (T)Convert.ChangeType(str, typeof(T));
        }
    }
}
