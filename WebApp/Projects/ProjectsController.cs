using CodeReaction.Domain;
using CodeReaction.Domain.Projects;
using CodeReaction.Web.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace CodeReaction.Web.Projects
{
    //[Authorize]
    public class ProjectsController : ApiController
    {
        /// <summary>
        /// Return all commits more recent than 
        /// </summary>summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/projects/create")]
        [AuthorizeAdmin]
        [HttpPost]
        public IHttpActionResult CreateProject(string name, string path)
        {
            UnitOfWork unitOfWork = null;

            try
            {
                unitOfWork = new UnitOfWork();
                Project newProject = new Project { Name = name, Path = path };
                unitOfWork.Context.Projects.Add(newProject);

                return Ok(newProject);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Create Project: " + ex);
                return InternalServerError(ex);
            }
            finally
            {
                if (unitOfWork != null)
                    unitOfWork.Dispose();
            }
        }

        [Route("api/projects")]
        [Authorize]
        [HttpGet]
        public IHttpActionResult GetProjects()
        {
            UnitOfWork unitOfWork = null;

            try
            {
                unitOfWork = new UnitOfWork();

                ProjectService projectService = new ProjectService(unitOfWork);

                return Ok(projectService.GetAll());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Create Project: " + ex);
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