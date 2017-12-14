using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace MonCore
{
    public class MonitoringController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage process()
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            
            return response;
        }
    }
}
