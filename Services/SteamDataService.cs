using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ModelClasses;

namespace Services
{
    public interface ISteamDataService
    {
    
    }

    public class SteamDataService : ISteamDataService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private const long SteamIdToDota2IdDiff = 76561197960265728;

        public SteamDataService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
        }



    }
}