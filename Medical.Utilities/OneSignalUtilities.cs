using OneSignal.RestAPIv3.Client;
using OneSignal.RestAPIv3.Client.Resources;
using OneSignal.RestAPIv3.Client.Resources.Notifications;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Utilities
{
    public class OneSignalUtilities
    {
        /// <summary>
        /// PUSH NOTIFICATION THÔNG QUA PLAYERID
        /// </summary>
        /// <param name="request"></param>
        /// <param name="appId"></param>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        public static async Task<string> OneSignalPushNotification(CreateNotificationModel request, Guid appId, string apiKey)
        {
            OneSignalClient client = new OneSignalClient(apiKey);
            var opt = new NotificationCreateOptions()
            {
                AppId = appId,
                IncludePlayerIds = request.PlayerIds,
                SendAfter = DateTime.Now.AddMilliseconds(10)
            };
            opt.Headings.Add(LanguageCodes.English, request.Title);
            opt.Contents.Add(LanguageCodes.English, request.Content);
            try
            {
                NotificationCreateResult result = await client.Notifications.CreateAsync(opt);
                return result.Id;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }



    }

    public class CreateNotificationModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public List<string> PlayerIds { get; set; }
    }

}
