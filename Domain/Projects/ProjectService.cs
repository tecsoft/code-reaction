using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReaction.Domain.Projects
{
    public class ProjectService
    {
        UnitOfWork unitOfWork;
        public ProjectService(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public IList<Project> GetAll()
        {
            var projects = unitOfWork.Context.Projects.ToList();

            if (projects.Count == 0)
            {
                return new List<Project> { DefaultProject };
            }
            else
            {
                return projects;
            }
        }

        public Project DefaultProject
        {
            get
            {
                return new Project { Id = 0, Name = "Default", Path = DefaultPath, IsDefault = true };
            }
        }

        private string DefaultPath
        {
            get
            {
                var svnConfig = (ConfigurationManager.GetSection("codeReaction") as CodeReactionConfigurationSection).Svn;
                return svnConfig.DefaultProjectPath;
            }
        }
    }
}
