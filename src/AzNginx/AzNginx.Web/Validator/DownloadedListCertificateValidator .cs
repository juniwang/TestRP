using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AzNginx.Common.Helpers;
using AzNginx.Common;

namespace AzNginx.Web.Validator
{
    public sealed class DownloadedListCertificateValidator : ICertificateValidator, IDisposable
    {
        string _csmCertificatesFetchUrl = null;
        ThumbprintBasedCertificateValidator _certificateValidator;
        Timer _reloadCsmCertificatesTimer;
        CancellationTokenSource _shutdown;

        public DownloadedListCertificateValidator(string csmCertificatesFetchUrl, params string[] alwaysTrustedCerts)
        {
            _csmCertificatesFetchUrl = csmCertificatesFetchUrl;
            _certificateValidator = new ThumbprintBasedCertificateValidator(alwaysTrustedCerts);
            _shutdown = new CancellationTokenSource();
            _reloadCsmCertificatesTimer = new Timer(
                ReloadCsmCertificates,
                alwaysTrustedCerts,
                TimeSpan.FromMilliseconds(1),
                TimeSpan.FromMinutes(15));
        }

        private void ReloadCsmCertificates(object alwaysTrustedCertsAsObj)
        {
            string[] alwaysTrustedCerts = alwaysTrustedCertsAsObj as string[];
            Task inBackground = ReloadCertificatesAsync(alwaysTrustedCerts, _shutdown.Token);
        }

        private async Task ReloadCertificatesAsync(string[] alwaysTrustedCerts, CancellationToken token)
        {
            try
            {
                string[] allTrustedCertThumbprints = null;
                DateTime endTime = DateTime.UtcNow.AddHours(1);
                while (endTime > DateTime.UtcNow)
                {
                    try
                    {
                        token.ThrowIfCancellationRequested();
                        AzureLog.Info("{0}",
                            $"Refreshing CSM Certificate Metadata: GET {_csmCertificatesFetchUrl}");

                        HttpClient client = new HttpClient();
                        HttpResponseMessage response = await client.GetAsync(_csmCertificatesFetchUrl, token);
                        if (response.IsSuccessStatusCode)
                        {
                            var rawCsmCertificates = await response.Content.ReadAsStringAsync();
                            CsmCertificates csmCertificates = JsonHelpers.DeserializeObject<CsmCertificates>(rawCsmCertificates);
                            var notExpiredCerts = csmCertificates.clientCertificates.Where(cert => cert.notAfter > DateTime.UtcNow);
                            allTrustedCertThumbprints = alwaysTrustedCerts.Concat(notExpiredCerts.Select(cert => cert.thumbprint)).ToArray();
                        }
                        else
                        {
                            if (response.Content != null)
                            {
                                string rawException = await response.Content.ReadAsStringAsync();
                                AzureLog.Error("Error occured in fetching certificates from {0}: {1}", _csmCertificatesFetchUrl, rawException);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        AzureLog.Error(e.Message, e);
                        allTrustedCertThumbprints = null;
                    }

                    if (allTrustedCertThumbprints != null && allTrustedCertThumbprints.Length > 0)
                    {
                        _certificateValidator = new ThumbprintBasedCertificateValidator(allTrustedCertThumbprints);
                        AzureLog.Info("Success. Trusted certificate thumbprints: [{0}]", string.Join(",", allTrustedCertThumbprints));
                        // successfully fetch certificates so return
                        return;
                    }
                    else
                    {
                        AzureLog.Info($"Not able to fetch CSM Certificates from URL: {_csmCertificatesFetchUrl}");
                    }

                    await Task.Delay(TimeSpan.FromSeconds(30), token);
                }
            }
            catch (Exception e)
            {
                AzureLog.Error(e.Message, e);
            }
        }

        public void Validate(X509Certificate2 certificate, Action<string> logAction)
        {
            _certificateValidator.Validate(certificate, logAction);
        }

        // Just _shutdown.Cancel() not _shutdown.Dispose() because we don't know when background tasks are done. Finalizer can clean up safely.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_shutdown")]
        public void Dispose()
        {
            _reloadCsmCertificatesTimer.Dispose();
            if (_shutdown != null)
            {
                _shutdown.Cancel();
                _shutdown = null;
            }
        }

        class CsmCertificates
        {
            public CsmCertificate[] clientCertificates { set; get; }
        }

        class CsmCertificate
        {
            public string thumbprint { set; get; }
            public DateTime notBefore { set; get; }
            public DateTime notAfter { set; get; }
            public string certificate { set; get; }
        }
    }
}
