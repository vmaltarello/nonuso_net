using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Nonuso.Application.IServices;
using Nonuso.Domain.Entities;
using Nonuso.Domain.IRepos;
using Nonuso.Infrastructure.Secret;
using Nonuso.Messages.Api;
using System.Net.Http.Headers;
using System.Text;

namespace Nonuso.Infrastructure.Notification.Services
{
    internal class NotificationService : INotificationService
    {
        readonly HttpClient _httpClient;
        readonly IConfiguration _configuration;
        readonly string _appId;
        readonly string _oneSignalApiURL = "https://onesignal.com/api/v1/notifications";
        readonly (string RestApiKey, string AppId) _oneSignalSettings;

        public NotificationService(HttpClient httpClient,
            ISecretManager secretManager,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;

            _oneSignalSettings = secretManager.GetOneSignalSettings();

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _oneSignalSettings.RestApiKey);
            _appId = _oneSignalSettings.AppId;
        }

        public async Task SendConfirmEmailAsync(User user, string link)
        {
            var obj = new
            {
                app_id = _appId,
                template_id = _configuration["OneSignal:Template:ConfirmEmailTemplateId"],
                include_unsubscribed = true,
                target_channel = "email",
                email_subject = "Conferma la tua email",
                include_email_tokens = new string[] { user.Email! },
                custom_data = new
                {
                    action_link = link,
                    username = user.UserName,
                }
            };

            var json = JsonConvert.SerializeObject(obj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            _ = await _httpClient.PostAsync(new Uri(_oneSignalApiURL), content);
        }

        public async Task SendRequestResetPasswordEmailAsync(User user, string link)
        {
            var obj = new
            {
                app_id = _appId,
                template_id = _configuration["OneSignal:Template:RequestResetPasswordEmailTemplateId"],
                include_unsubscribed = true,
                target_channel = "email",
                email_subject = "Richiesta reset password",
                include_email_tokens = new string[] { user.Email! },
                custom_data = new
                {
                    action_link = link,
                    username = user.UserName,
                }
            };

            var json = JsonConvert.SerializeObject(obj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            _ = await _httpClient.PostAsync(new Uri(_oneSignalApiURL), content);
        }

        public async Task SendPushNotificationAsync(PusNotificationParamModel model)
        {
            var payload = new
            {
                app_id = _appId,
                include_aliases = new { external_id = new string[] { model.UserId.ToString() } },
                headings = new { en = model.UserName },
                contents = new { en = model.Content },
                target_channel = "push",
                ios_badgeType = "Increase", // Only for iOS
                ios_badgeCount = 1,
                app_url = $"nonuso.app://chat?conversationId={model.ConversationId}"
            };

            var json = JsonConvert.SerializeObject(payload, Formatting.Indented);
            var contentToSend = new StringContent(json, Encoding.UTF8, "application/json");

            _ = await _httpClient.PostAsync(new Uri(_oneSignalApiURL), contentToSend);
        }
    }
}
