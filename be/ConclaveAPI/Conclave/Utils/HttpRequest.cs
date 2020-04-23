using System.Net.Http;

namespace Conclave.Utils
{
    /*
     * A http client singleton
     */
    public static class HttpRequest
    {
        private static readonly HttpClient _httpClient;

        static HttpRequest()
        {
            _httpClient = new HttpClient();
        }

        internal static HttpClient Get()
        {
            return _httpClient;
        }
    }
}
