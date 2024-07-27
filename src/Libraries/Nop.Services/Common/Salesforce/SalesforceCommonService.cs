using System;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Nop.Services.Common.Salesforce;

public class SalesforceCommonService : ISalesforceCommonService
{
    #region Fields

    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly string _salesforceClientId;
    private readonly string _salesforceClientSecret;
    private readonly string _salesforceUserName;
    private readonly string _salesforcePassword;
    private readonly string _salesforceBaseUrl;


    #endregion


    #region Ctor

    public SalesforceCommonService(IConfiguration configuration, HttpClient httpClient)
    {
        _configuration = configuration;
        _httpClient = httpClient;

        var builder = new ConfigurationBuilder()
          .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
          .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
          .AddEnvironmentVariables();

        _configuration = builder.Build();


        _salesforceClientId = _configuration["AppSettings:SalesforceClientId"];
        _salesforceClientSecret = _configuration["AppSettings:SalesforceClientSecret"];
        _salesforceUserName = _configuration["AppSettings:SalesforceUserName"];
        _salesforcePassword = _configuration["AppSettings:SalesforcePassword"];
        _salesforceBaseUrl = _configuration["AppSettings:SalesforceBaseUrl"];

    }


    #endregion


    #region Methods

    /// <summary>
    /// Created httpclient and make api call
    /// </summary>
    /// <param name="endpoint">The endpoint after the base url</param>
    /// <param name="contactJsonString">the input jsonstring</param>
    /// <returns>returns the data content</returns>
    public async Task<string> SalesforceAPICallHandler(string endpoint, string contactJsonString)
    {
        try
        {
            var salesforceToken = await GetSalesforceToken();
            if (salesforceToken != null)
            {
                string url = $"{_salesforceBaseUrl}{endpoint}";
                Uri uri = new Uri(url);
                HttpContent content = new StringContent(contactJsonString, Encoding.UTF8, "application/json");
                var responseContent = await MakeSalesforceAPICall(uri, salesforceToken, content, true);
                return responseContent;
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }


    /// <summary>
    /// Get the SalesforceAPItoken
    /// </summary>
    /// <returns></returns>
    public async Task<string> GetSalesforceToken()
    {
        try
        {

        
        var url = $"{_salesforceBaseUrl}oauth2/token?grant_type=password&client_id={_salesforceClientId}&client_secret={_salesforceClientSecret}&username={_salesforceUserName}&password={_salesforcePassword}";

        var response = await _httpClient.PostAsync(url, null);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        JObject json = JObject.Parse(responseContent);
        string accessToken = json["access_token"].ToString();

        return accessToken;

        }
        catch (Exception ex)
        {
            return null;
        }
    }

    /// <summary>
    /// Common method for making POST api calls to salesforce
    /// </summary>
    /// <param name="url"></param>
    /// <param name="salesforceToken"></param>
    /// <param name="httpContent"></param>
    /// <param name="isAuthenticationRequired"></param>
    /// <returns></returns>
    private async Task<string> MakeSalesforceAPICall(Uri url, string salesforceToken, HttpContent httpContent = null, bool isAuthenticationRequired = false)
    {
        string responseContent = string.Empty;
        if (isAuthenticationRequired)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", salesforceToken);
        }

        var response = await _httpClient.PostAsync(url, httpContent);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            //detect an internal redirect and make request to the redirected uri
            var finalRequestUri = response.RequestMessage.RequestUri;
            if (finalRequestUri != url)
            {
                response = await _httpClient.PostAsync(finalRequestUri, httpContent);
                response.EnsureSuccessStatusCode();
                responseContent = await response.Content.ReadAsStringAsync();
            }
        }
        return responseContent;
    }

    #endregion

}

