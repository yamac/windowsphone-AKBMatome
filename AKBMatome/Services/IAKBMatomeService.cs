using System;
using AKBMatome.Data;

namespace AKBMatome.Services
{
    public interface IAKBMatomeService
    {
        // Feed
        void GetAllFeedGroups(FeedDataContext dataContext, Action<Exception> callback, bool update);
        void GetAllFeedGroupsAndChannels(FeedDataContext dataContext, Action<Exception> callback, bool update);
        void GetFeedItems(FeedDataContext dataContext, int classId, int[] channelIds, int page, Action<AKBMatomeService.GetFeedItemsResult, Exception> callback);
    
        // Notification
        void RegisterNotificationChannel(string version, string langCode, Action<AKBMatomeService.RegisterNotificationChannelResult, Exception> callback);
        void UnregisterNotificationChannel(string uuid, Action<AKBMatomeService.UnregisterNotificationChannelResult, Exception> callback);
        void UpdateNotificationChannel(string uuid, string version, string langCode, int[] channelIds, bool resetUnreads, Action<AKBMatomeService.UpdateNotificationChannelResult, Exception> callback);
    }
}
