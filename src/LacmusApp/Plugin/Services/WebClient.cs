using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl;
using LacmusApp.Plugin.Interfaces;
using LacmusApp.Plugin.Models;
using LacmusPlugin;
using Newtonsoft.Json;

namespace LacmusApp.Plugin.Services
{
    internal class WebClient : IWebClient
    {
        private string baseUrl;
        private HttpClient httpClient;
        public WebClient(string baseUrl)
        {
            this.baseUrl = baseUrl;
            httpClient = new HttpClient();
        }
        
        public async Task<int> GetMaxPage()
        {
            var url = Url.Combine(baseUrl, "/plugin-repository/api/v1/pagesCount");
            var response = await httpClient.GetAsync(url);
            var jsonStr = await response.Content.ReadAsStringAsync();
            var pageCount = JsonConvert.DeserializeObject<MaxPageCount>(jsonStr);
            return pageCount.Count;
        }
        
        public async Task<IEnumerable<IObjectDetectionPlugin>> GetPluginInfoFromPage(int page)
        {
            var url = Url.Combine(baseUrl, $"/plugin-repository/api/v1/plugins?page={page}");
            var response = await httpClient.GetAsync(url);
            var jsonStr = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<PluginInfo>>(jsonStr);
        }

        public async Task<Stream> GetZipFile(string tag, int api, int major, int minor)
        {
            var url = Url.Combine(baseUrl, "/plugin-repository/api/v1/plugin?",
                $"tag={tag}", $"api={api}", 
                $"major={major}", $"minor={minor}");
            return await httpClient.GetStreamAsync(url);
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }
    }
}