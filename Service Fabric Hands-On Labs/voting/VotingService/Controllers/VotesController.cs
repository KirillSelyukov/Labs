using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Web.Http;

namespace VotingService.Controllers
{
    public class VotesController : ApiController
    {
        // Used for health checks.
        public static long _requestCount;

        // Holds the votes and counts. NOTE: THIS IS NOT THREAD SAFE FOR THE PURPOSES OF THE LAB ONLY.
        private static readonly Dictionary<string, int> _counts = new Dictionary<string, int>();

        // GET api/votes 
        [HttpGet]
        [Route("api/votes")]
        public HttpResponseMessage Get()
        {
            Interlocked.Increment(ref _requestCount);

            var votes = new List<KeyValuePair<string, int>>(_counts.Count);
            foreach (var kvp in _counts)
                votes.Add(kvp);

            var response = Request.CreateResponse(HttpStatusCode.OK, votes);
            response.Headers.CacheControl = new CacheControlHeaderValue {NoCache = true, MustRevalidate = true};
            return response;
        }

        [HttpPost]
        [Route("api/{key}")]
        public HttpResponseMessage Post(string key)
        {
            Interlocked.Increment(ref _requestCount);

            if (false == _counts.ContainsKey(key))
                _counts.Add(key, 1);
            else
                _counts[key] = _counts[key] + 1;

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        [HttpDelete]
        [Route("api/{key}")]
        public HttpResponseMessage Delete(string key)
        {
            Interlocked.Increment(ref _requestCount);

            if (_counts.ContainsKey(key))
                if (_counts.Remove(key))
                    return Request.CreateResponse(HttpStatusCode.OK);

            return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        [HttpGet]
        [Route("api/{file}")]
        public HttpResponseMessage GetFile(string file)
        {
            string response = null;
            var responseType = "text/html";

            Interlocked.Increment(ref _requestCount);

            // Validate file name.
            if ("index.html" == file)
            {
                // This hardcoded path is only for the lab. Later in the lab when the version is changed, this
                // hardcoded path must be changed to use the UX. In part 2 of the lab, this will be calculated
                // using the connected service path.
                var path = string.Format(@"..\VotingServicePkg.Code.1.0.0\{0}", file);
                response = File.ReadAllText(path);
            }

            if (null != response)
                return Request.CreateResponse(HttpStatusCode.OK, response, responseType);
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "File");
        }
    }
}