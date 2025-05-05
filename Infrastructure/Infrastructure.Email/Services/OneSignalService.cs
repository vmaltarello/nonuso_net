using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Nonuso.Application.IServices;
using System.Net.Http.Headers;
using System.Text;
namespace Nonuso.Infrastructure.Notification.Services
{
    internal class OneSignalService : IOneSignalService
    {
        readonly HttpClient _httpClient;
        readonly IConfiguration _configuration;

        public OneSignalService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _configuration["OneSignal:RestApiKey"]);

        }

        public async Task SendConfirmEmailAsync(Domain.Entities.User user, string tokenConfirmEmail)
        {
            var obj = new
            {
                app_id = _configuration["OneSignal:AppId"],
                template_id = _configuration["OneSignal:Template:ConfirmEmailTemplateId"],
                include_unsubscribed = true,
                target_channel = "email",
                email_subject = "Conferma la tua email",
                include_email_tokens = new string[] { user.Email! },
                custom_data = new 
                {
                    action_link = tokenConfirmEmail,
                    username = user.UserName,
                }
            };

            var json = JsonConvert.SerializeObject(obj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(new Uri("https://onesignal.com/api/v1/notifications"), content);
        }
    }
}
