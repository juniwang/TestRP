using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace TestRP
{
    public sealed class HttpStatusResult : IHttpActionResult, IDisposable
    {
        HttpResponseMessage _response;

        public HttpStatusResult(HttpStatusCode code, HttpContent responseBody = null)
        {
            _response = new HttpResponseMessage(code);
            _response.Content = responseBody;
        }

        // Created to verify status in tests
        public HttpStatusCode StatusCode
        {
            get
            {
                return _response.StatusCode;
            }
        }

        public Task<HttpResponseMessage> ExecuteAsync(
            CancellationToken cancellationToken)
        {
            return Task.FromResult(_response);
        }

        public void Dispose()
        {
            _response.Dispose();
        }

        public void AddHeader(string name, string value)
        {
            try
            {
                this._response.Headers.Add(name, value);
            }
            catch (InvalidOperationException)
            {
                this._response.Content.Headers.Add(name, value);
            }
        }
    }
}
