using CodeReaction.Domain;
using CodeReaction.Domain.Commits;
using CodeReaction.Domain.Repositories;
using CodeReaction.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using CodeReaction.Web.Models;
using CodeReaction.Web.Auth;
using CodeReaction.Web.RevisionDetail;

namespace CodeReaction.Web.Controllers
{
    [IdentityBasicAuthentication]
    [Authorize]
    public class RevisionDetailController : ApiController
    {
        // TODO 
        static CommitRepository repos = new CommitRepository();

        T? ParseNullable<T>(string str) where T : struct
        {
            if (string.IsNullOrEmpty(str))
                return null;

            return (T)Convert.ChangeType(str, typeof(T));
        }

        /// <summary>
        /// Return all commits more recent than 
        /// </summary>summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/commits")]
        public IHttpActionResult GetCommits()
        {
            CommitViewModel vm;

            var parameters = this.Request.GetQueryNameValuePairs();

            using (UnitOfWork uow = new UnitOfWork())
            {
                CommitQuery query = new CommitQuery(uow);
                var keyword = parameters.FirstOrDefault(p => p.Key == "keyword").Value;

                if ( string.IsNullOrEmpty(keyword) == false )
                {
                    query.Keyword = parameters.FirstOrDefault(p => p.Key == "keyword").Value;
                    query.ExcludeApproved = false;
                }
                else
                {
                    query.ExcludeApproved = true;
                }
                
                query.Max = ParseNullable<int>(parameters.FirstOrDefault(p => p.Key == "max").Value);
                query.IncludeAuthor = parameters.FirstOrDefault(p => p.Key == "include").Value;
                query.ExcludeAuthor = parameters.FirstOrDefault(p => p.Key == "exclude").Value;

                var list = new List<Tuple<Commit, CommitStats>>();
                foreach (var commit in query.Execute() )
                {
                    var comments = uow.Context.Comments.Where(c => c.Revision == commit.Revision);

                    var replies = comments.Where(c => c.User == commit.Author);
                    var reviews = comments.Where(c => c.User != commit.Author);

                    var stats = new CommitStats(
                        reviews.Select( c => c.User ).Distinct().Count(),
                        reviews.Count(), 
                        replies.Count());

                    list.Add(new Tuple<Commit, CommitStats>(commit, stats));
                }
                vm = new CommitViewModel(list);
            }

            return Ok(vm);  // try catch?
        }

        [Route("api/commits/revision/{revision}")]
        public IHttpActionResult GetRevision(long revision)
        {
            var commitDiff= repos.GetRevision(revision);

            UnitOfWork unitOfWork = new UnitOfWork();
            var likes = new LikeService(unitOfWork).GetLikes(revision).ToList();
            var comments = new CommentService(unitOfWork).GetComments(revision).ToList();

            // merge data to view model efficiently

            Commit commit = unitOfWork.Context.Commits.FirstOrDefault(c => c.Revision == revision);

            RevisionDetailViewModel viewModel = RevisionDetailViewModel.Create( commit, commitDiff, likes, comments);

            return Ok(viewModel);
        }

        //[Route("api/commits/like/{user}/{revision}/{file}")]
        //public IHttpActionResult LikeFile(string user, int revision, int file)
        //{
        //    UnitOfWork unitOfWork = new UnitOfWork();

        //    try
        //    {
        //        new LikeService(unitOfWork).LikeFile(user, revision, file);
        //        unitOfWork.Save();
        //    }
        //    catch (Exception ex)
        //    {
        //        return InternalServerError(ex);
        //    }
        //    finally
        //    {
        //        unitOfWork.Dispose();
        //    }
        //    return Ok();
        //}

        //[Route("api/commits/like/{user}/{revision}/{file}/{lineId}")]
        //public IHttpActionResult LikeLine(string user, int revision, int file, string lineId )
        //{
        //    UnitOfWork unitOfWork = new UnitOfWork();

        //    try
        //    {
        //        new LikeService(unitOfWork).LikeLine(user, revision, file, lineId);

        //        unitOfWork.Save();

        //    }
        //    catch (Exception ex)
        //    {
        //        return InternalServerError(ex);
        //    }
        //    finally
        //    {
        //        unitOfWork.Dispose();
        //    }
        //    return Ok();
        //}

        [Route("api/commits/comment/{user}/{revision}")]
        public IHttpActionResult ReviewCommit(string user, int revision)
        {
            UnitOfWork unitOfWork = new UnitOfWork();

            try
            {
                var parameters = this.Request.GetQueryNameValuePairs().FirstOrDefault(i => i.Key == "comment");

                new CommentService(unitOfWork).CommentLine(user, revision, null, null, parameters.Value);

                unitOfWork.Save();

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }
            return Ok();
        }

        [Route("api/commits/comment/{user}/{revision}/{file}/{lineId}")]
        public IHttpActionResult CommentLine(string user, int revision, int file, string lineId)
        {
            UnitOfWork unitOfWork = new UnitOfWork();

            try
            {
                var parameters = this.Request.GetQueryNameValuePairs().FirstOrDefault(i => i.Key == "comment");

                new CommentService(unitOfWork).CommentLine(user, revision, file, lineId, parameters.Value);

                unitOfWork.Save();

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }
            return Ok();
        }

        [Route("api/commits/reply/{idComment}/{author}")]
        public IHttpActionResult CommentLine(long idComment, string author)
        {
            UnitOfWork unitOfWork = new UnitOfWork();

            try
            {
                var parameters = this.Request.GetQueryNameValuePairs().FirstOrDefault(i => i.Key == "comment");

                new CommentService(unitOfWork).Reply( idComment, author, parameters.Value );

                unitOfWork.Save();

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }
            return Ok();
        }

        [Route("api/commits/approve/{revision}/{approver}")]
        public IHttpActionResult ApproveRevision( int revision, string approver )
        {
            UnitOfWork unitOfWork = new UnitOfWork();
            try
            {
                new CommitService(unitOfWork).ApproveCommit(revision, approver);
                unitOfWork.Save();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return Ok();
        }
    }
}
