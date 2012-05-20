using System;
using AKBMatome.Data;

namespace AKBMatome.Services
{
    public interface IAKBMatomeService
    {
        // Feed
        void GetAllFeedGroupsAndChannels(FeedDataContext dataContext, Action<Exception> callback, bool update);
        void GetFeedGroups(int[] groupIds, Action<FeedGroup[], Exception> callback);
        void GetFeedChannels(int[] groupIds, int[] channelIds, Action<FeedChannel[], Exception> callback);
        void GetFeedItems(FeedDataContext dataContext, int[] groupIds, int[] channelIds, int page, Action<AKBMatomeService.GetFeedItemsResult, Exception> callback);
    
        // Notification
        void RegisterNotificationChannel(Action<AKBMatomeService.RegisterNotificationChannelResult, Exception> callback);
        void UnregisterNotificationChannel(string uuid, Action<AKBMatomeService.UnregisterNotificationChannelResult, Exception> callback);
        void UpdateNotificationChannel(string uuid, string langCode, int[] channelIds, bool resetUnreads, Action<AKBMatomeService.UpdateNotificationChannelResult, Exception> callback);
    }
}
