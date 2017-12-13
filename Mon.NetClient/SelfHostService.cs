using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace Mon.NetClient
{
    partial class SelfHostService : ServiceBase
    {
        private HttpSelfHostConfiguration config;
        private HttpSelfHostServer server;

        public SelfHostService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            config = new HttpSelfHostConfiguration("http://localhost:8080");

            config.Routes.MapHttpRoute(
                name: "API",
                routeTemplate: "{controller}/{action}",
                defaults: new { id = RouteParameter.Optional }
            );

            server = new HttpSelfHostServer(config);
            server.OpenAsync().Wait();
        }

        protected override void OnStop()
        {
            if (server != null)
            {
                server.CloseAsync().Wait();
                server.Dispose();
            }
        }
    }
}
