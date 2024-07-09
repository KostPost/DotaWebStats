using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModelClasses;

public interface IDotaDataService
{

}

public class DotaDataService : IDotaDataService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly ILogger<DotaDataService> _logger;
    private readonly string _apiKey;
    private const string BaseUrl = "https://api.steampowered.com/";

    public DotaDataService(IConfiguration configuration, HttpClient httpClient, ILogger<DotaDataService> logger)
    {
        _configuration = configuration;
        _httpClient = httpClient;
        _logger = logger;
        _apiKey = _configuration["Steam:ApiKey"];

        if (string.IsNullOrEmpty(_apiKey))
        {
            _logger.LogError("Steam API key is not configured.");
            throw new InvalidOperationException("Steam API key is not configured.");
        }
    }



}