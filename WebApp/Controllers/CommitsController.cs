using CodeReviewer.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApp.Controllers
{
    public class CommitsController : ApiController
    {
        static CommitRepository repos = new CommitRepository();
        /// <summary>
        /// Return all commits more recent than 
        /// </summary>summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route( "api/commits/since/{revision}/{max}")]
        public IHttpActionResult GetSince(int revision, int max )
        {
            return Ok(repos.GetSince(revision, max));
        }

        [Route("api/commits/revision/{revision}")]
        public IHttpActionResult GetRevision(int revision)
        {
            return Ok(repos.GetRevision(revision));
        }
    }
}
