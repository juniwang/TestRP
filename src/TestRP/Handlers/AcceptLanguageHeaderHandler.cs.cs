using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestRP.Handlers
{
    class AcceptLanguageHeaderHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken token)
        {
            var culture = GetCultureFromRequest(request);

            var currentCulture = Thread.CurrentThread.CurrentCulture;
            var currentUICulture = Thread.CurrentThread.CurrentUICulture;

            try
            {
                if (culture != null)
                {
                    Thread.CurrentThread.CurrentCulture = culture;
                    Thread.CurrentThread.CurrentUICulture = culture;
                }

                return await base.SendAsync(request, token);
            }
            finally
            {
                //Set thread's culture back to default value.
                Thread.CurrentThread.CurrentCulture = currentCulture;
                Thread.CurrentThread.CurrentUICulture = currentUICulture;
            }
        }

        public static CultureInfo GetCultureFromRequest(HttpRequestMessage request)
        {
            // If Accept-Language is set in the request, it must be localized to that language. 
            // http://sharepoint/sites/AzureUX/Sparta/Shared%20Documents/Specs/Resource%20Provider%20API%20v2.docx

            CultureInfo culture = null;
            var languages = new List<StringWithQualityHeaderValue>();

            if (request.Headers.AcceptLanguage != null)
            {
                languages.AddRange(request.Headers.AcceptLanguage);
            }

            // rfc2616 - the quality value defaults to "q=1"
            languages = languages.OrderByDescending(ln => (ln.Quality ?? 1.0)).ToList();

            foreach (StringWithQualityHeaderValue lang in languages)
            {
                try
                {
                    culture = CultureInfo.GetCultureInfo(lang.Value);
                    break;
                }
                catch (CultureNotFoundException)
                {
                    // if no valid culture is specified, default culture will be picked up
                }
            }

            return culture;
        }
    }
}
