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
            HouseKeepingService.ImportLatestLogs();
        }
    }
}
