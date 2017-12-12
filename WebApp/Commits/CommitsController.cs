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
    public class CommitsController : ApiController
    {
        /// <summary>
        /// Return all commits more recent than 
        /// </summary>summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/commits")]
        public IHttpActionResult GetCommits()
        {
            CommitsModel vm;
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
                query.Max = Parser.ParseNullable<int>(parameters.FirstOrDefault(p => p.Key == "max").Value);
                query.IncludeAuthor = parameters.FirstOrDefault(p => p.Key == "include").Value;
                query.ExcludeAuthor = parameters.FirstOrDefault(p => p.Key == "exclude").Value;

                var list = new List<Tuple<Commit, CommitStats>>();
                foreach (var commit in query.Execute())
                {
                    var comments = unitOfWork.Context.Comments.Where(c => c.Revision == commit.Revision).ToList();

                    var replies = comments.Where(c => c.User == commit.Author && c.IsLike == false);
                    var reviews = comments.Where(c => c.User != commit.Author && c.IsLike == false);
                    var likes = comments.Where(c => c.IsLike == true);

                    var stats = new CommitStats(
                        reviews.Select(c => c.User).Distinct().Count(),
                        reviews.Count(),
                        replies.Count(),
                        likes.Count());

                    list.Add(new Tuple<Commit, CommitStats>(commit, stats));
                }
                vm = new CommitsModel(list);

                return Ok(vm);
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
    }
}
