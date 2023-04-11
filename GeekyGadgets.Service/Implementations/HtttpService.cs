using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GeekyGadgets.Service.Implementations
{
    public class HttpService
    {
        private readonly HttpClient _httpClient;

        public HttpService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> GetAsync(string url, string token = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            return await _httpClient.SendAsync(request);
        }

        public async Task<HttpResponseMessage> PostAsync(string url, HttpContent content, string token = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            request.Content = content;
            return await _httpClient.SendAsync(request);
        }

        public async Task<HttpResponseMessage> PutAsync(string url, HttpContent content, string token = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, url);
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            request.Content = content;
            return await _httpClient.SendAsync(request);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string url, string token = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, url);
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            return await _httpClient.SendAsync(request);
        }
    }

}
