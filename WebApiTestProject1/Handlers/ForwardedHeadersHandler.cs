using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace WebApiTestProject1.Handlers
{
    public class ForwardedHeadersHandler : DelegatingHandler
    {
        // There are a new standard and a old standard, check developer.mozilla.org... 

        // New style header
        const string _fwdHeader = "Forwarded";

        // Old style, separate headers
        const string _fwdProtoHeader = "X-Forwarded-Proto";
        const string _fwdHostHeader = "X-Forwarded-Host";
        public const string _fwdForHeader = "X-Forwarded-For";

        // Key to which the value will be saved in the header, 
        // though methods are used so that the user doesn't have to use this key
        public const string MyClientBaseUrlProperty = "MyClientBaseUrl";

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Start with the basuc URI based on the server's view of the request
            UriBuilder builder = new UriBuilder(
                request.RequestUri.Scheme, request.RequestUri.Host, request.RequestUri.Port);

            // Client IP
            string ip = null;

            // Override host and protocol if found in headers

            // First, check legacy x-forwarded headers
            if (request.Headers.Contains(_fwdProtoHeader))
            {
                // Protocol, http or https
                var proto = request.Headers.GetValues(_fwdProtoHeader)
                    .FirstOrDefault(s => !String.IsNullOrEmpty(s));

                if (proto != null)
                    builder.Scheme = proto;
            }

            if (request.Headers.Contains(_fwdHostHeader))
            {
                // The forwarded host string
                var host = request.Headers.GetValues(_fwdHostHeader)
                    .FirstOrDefault(s => !String.IsNullOrEmpty(s));

                if (host != null)
                    SetHostAndPort(builder, host);
            }

            if (request.Headers.Contains(_fwdForHeader))
            {
                ip = request.Headers.GetValues(_fwdForHeader).FirstOrDefault(s => !string.IsNullOrEmpty(s));
                if (!string.IsNullOrEmpty(ip) && ip.Contains(","))
                    ip = ip.Substring(0, ip.IndexOf(','));
            }

            // Secondly, try the newer Forwarded header
            if (request.Headers.Contains(_fwdHeader))
            {
                // Grab the forwarded host string
                var fwd = request.Headers
                    .GetValues(_fwdHeader)
                    .FirstOrDefault(s => !string.IsNullOrEmpty(s))
                    .Split(';')
                    .Select(s => s.Trim());

                // Syntax for the Forwarded header: Forwarded: by=<identifier>;for=<identifier>;host=<host>;proto=<http|https>
                var proto = fwd.FirstOrDefault(s => s.ToLowerInvariant().StartsWith("proto="));
                if (!string.IsNullOrEmpty(proto))
                {
                    proto = proto.Substring(6).Trim();
                    if (!string.IsNullOrEmpty(proto))
                        builder.Scheme = proto;
                }

                var host = fwd.FirstOrDefault(s => s.ToLowerInvariant().StartsWith("host="));
                if (!string.IsNullOrEmpty(host))
                {
                    host = host.Substring(5).Trim();
                    if (!string.IsNullOrEmpty(host))
                        SetHostAndPort(builder, host);
                }

                ip = fwd.FirstOrDefault(s => s.ToLowerInvariant().StartsWith("for="));
                if (!string.IsNullOrEmpty(ip))
                {
                    ip = ip.Substring(4);
                }
                else
                {
                    // If no ip is set, take it from the context
                    if (request.Properties.ContainsKey("MS_HttpContext"))
                    {
                        var ctx = request.Properties["MS_HttpContext"] as HttpContextBase;
                        if (ctx != null)
                            // :1 is the compressed format IPV6 loopback address 0:0:0:0:0:0:0:1. It is the equivalent of the IPV4 address 127.0.0.1.
                            ip = ctx.Request.UserHostAddress;
                    }
                }
            }

            builder.Path = "/";

            // Store the IP if found
            if (!string.IsNullOrEmpty(ip))
                request.Properties.Add(_fwdForHeader, ip);

            // Add the final calculated URL to the Properties collection
            request.Properties.Add(MyClientBaseUrlProperty, builder.Uri);

            // Continue the handler chain
            return base.SendAsync(request, cancellationToken);
        }

        private static void SetHostAndPort(UriBuilder builder, string host)
        {
            var hostAndPort = host.Split(':');
            builder.Host = hostAndPort[0];

            // If a non standard port is used, it'll be available here, else set -1
            if (hostAndPort.Length > 1)
                builder.Port = int.Parse(hostAndPort[1]);
            else
                builder.Port = -1;
        }
    }

    public static class HttpRequestMessageBaseUrlExtension
    {
        public static Uri GetSelfReferenceBaseUrl(this HttpRequestMessage request)
        {
            if (request == null)
            {
                return null;
            }

            if (request.Properties.TryGetValue(ForwardedHeadersHandler.MyClientBaseUrlProperty, 
                 out object baseUrl))
            {
                return (Uri)baseUrl;
            }

            return null;
        }

        public static Uri RebaseUrlForClient(this HttpRequestMessage request, Uri serverUrl)
        {
            Uri clientBase = GetSelfReferenceBaseUrl(request);

            if (clientBase == null)
                return null;

            if (serverUrl == null)
                return clientBase;

            // Rest the base scheme/host/port to the client version
            var builder = new UriBuilder(serverUrl);
            builder.Scheme = clientBase.Scheme;
            builder.Host = clientBase.Host;
            builder.Port = clientBase.Port;

            return builder.Uri;
        }

        public static string GetClientIpAddress(this HttpRequestMessage request)
        {
            if (request == null)
                return null;

            if (request.Properties.TryGetValue(ForwardedHeadersHandler._fwdForHeader, out object ip))
            {
                return (string)ip;
            }

            return null;
        }
    }
}