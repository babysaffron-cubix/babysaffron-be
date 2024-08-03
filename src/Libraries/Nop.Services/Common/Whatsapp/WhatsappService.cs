using System;
using Microsoft.Extensions.Configuration;

namespace Nop.Services.Common.Whatsapp;

public class WhatsappService : IWhatsappService
{

    #region Fields
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly string _userId;
    private readonly string _password;
    private const string _messageType = "Text";
    private const string _authSchema = "plain";
    private const string _method = "SendMessage";
    private const string _v = "1.1";
    private const string _format = "json";
    private const string _message = "Dear {0},\nYour Order No. {1} has been Accepted. Total Approved Quantity is {2} Grams. Outstanding Amount of this order is Rs. {3}.";
    private const string _isTemplate = "true";
    private const string _header = "Your Order has been Accepted";
    private const string _footer = "Thank You!";
    private readonly string _whatappUrl;






    #endregion


    #region Ctor

    public WhatsappService(IConfiguration configuration, HttpClient httpClient)
    {
        _configuration = configuration;
        _httpClient = httpClient;

        var builder = new ConfigurationBuilder()
          .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
          .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
          .AddEnvironmentVariables();

        _configuration = builder.Build();

        _userId = _configuration["AppSettings:WhatsappUserId"];
        _password = _configuration["AppSettings:WhatsappPassword"];
        _whatappUrl = _configuration["AppSettings:WhatsappUrl"];

    }
    #endregion


    #region Methods

    public async Task<string> SendOrderConfirmationOnWhatsapp(string sendToNumber, WhatsappEmailRequest whatsappEmailRequest)
    {
        using (var httpClient = new HttpClient())
        {
            string whatappMessage = String.Format(_message, whatsappEmailRequest.Name, whatsappEmailRequest.OrderId, whatsappEmailRequest.Weight, whatsappEmailRequest.OrderAmount);

            var formData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("send_to", sendToNumber),
                new KeyValuePair<string, string>("msg_type", _messageType),
                new KeyValuePair<string, string>("userid", _userId),
                new KeyValuePair<string, string>("auth_scheme", _authSchema),
                new KeyValuePair<string, string>("password", _password),
                new KeyValuePair<string, string>("method", _method),
                new KeyValuePair<string, string>("v", _v),
                new KeyValuePair<string, string>("format", _format),
                new KeyValuePair<string, string>("msg", whatappMessage),
                new KeyValuePair<string, string>("isTemplate", _isTemplate),
                new KeyValuePair<string, string>("header", _header),
                new KeyValuePair<string, string>("footer", _footer),

            };

            // Encode form data
            var content = new FormUrlEncodedContent(formData);

            // Send POST request
            HttpResponseMessage response = await httpClient.PostAsync(_whatappUrl, content);

            // Check if successful
            if (response.IsSuccessStatusCode)
            {
                // Read response content
                string responseContent = await response.Content.ReadAsStringAsync();
                return responseContent;
            }
            else
            {
                return "Error when trying to send whatsapp message";
            }
        }
    }

  
    #endregion

}

