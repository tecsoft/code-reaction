using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using System.IO;
using System.Web.Http;
using CodeReaction.Domain;
using CodeReaction.Domain.Commits;
using CodeReaction.Domain.HouseKeeping;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Cookies;
using CodeReaction.Domain.Database;

[assembly: OwinStartup(typeof(CodeReaction.Web.Startup))]

namespace CodeReaction.Web
{
    public partial class Startup
    {
        static CodeReaction.Domain.HouseKeeping.TaskScheduler scheduler = new CodeReaction.Domain.HouseKeeping.TaskScheduler();


        public void Configuration(IAppBuilder app)
        {
            SchemaMigration.Execute();

            HttpConfiguration httpConfiguration = new HttpConfiguration();
            ConfigureAuth(app);
            WebApiConfig.Register(httpConfiguration);
            app.UseWebApi(httpConfiguration);

            // Perhaps there's a better place for this call?
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
            catch (Exception ex)
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
