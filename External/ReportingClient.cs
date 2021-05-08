using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace binlookup.External
{



	public class ReportingClient : IReportingClient
    {
        private readonly HttpClient c_httpClient;
        private readonly string c_requestUri;


        public ReportingClient(
            AppSettings appSettings)
        {
            this.c_httpClient = new HttpClient();
            this.c_requestUri = appSettings.ReportingRequestUri;
        }


        public async void LogActivity(
            string requestId,
            string correlationId,
            string activity,
            string activityDetail)
        {
            var _logActivityRequest = new binlookup.External.LogActivityRequest(activity, activityDetail);
            var _logActivityRequestAsString = JsonConvert.SerializeObject(_logActivityRequest);
            Console.WriteLine(_logActivityRequestAsString);
            var _content = new StringContent(_logActivityRequestAsString);
            _content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            _content.Headers.Add("X-Request-Id", requestId);
            _content.Headers.Add("X-Correlation-Id", correlationId);
            await this.c_httpClient.PostAsync(this.c_requestUri, _content);
            Console.WriteLine(_logActivityRequestAsString);
            return;
        }
    }
}