using CodeReaction.Domain;
using CodeReaction.Domain.Commits;
using CodeReaction.Domain.HouseKeeping;
using CodeReaction.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace CodeReaction.Web
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        static TaskScheduler scheduler = new TaskScheduler();

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            scheduler.Start(new System.Threading.TimerCallback(ImportLatestLogs), 60);
        }

        void ImportLatestLogs(object o)
        {
            UnitOfWork unitOfWork = null; ; 
            try
            {
                unitOfWork = new UnitOfWork();

                var sourceControl = new SourceControl();
                var houseKeeper = new HouseKeepingService(unitOfWork, sourceControl);

                int logsImported = houseKeeper.ImportLatestLogs();

                unitOfWork.Save();

                System.Diagnostics.Trace.TraceInformation(logsImported + " revisions imported");
            }
            catch(Exception ex)
            {
                System.Diagnostics.Trace.TraceError("ImportLatestLogs: " + ex);
            }
            finally
            {
                if (unitOfWork != null)
                    unitOfWork.Dispose();
            }
        }
    }
}
