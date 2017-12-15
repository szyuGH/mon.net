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
        MonitoringProcessor processor;

        public MonitoringController()
            :base()
        {
            processor = new MonitoringProcessor();
        }

        [HttpPost]
        public HttpResponseMessage process([FromBody]MonRequest monRequest)
        {
            object monResponse = processor.Invoke(monRequest);


            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }
    }
}
