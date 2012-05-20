using System.Collections.ObjectModel;
using AKBMatome.Data;

namespace AKBMatome.SampleData
{
    public class ChannelListPageSampleData
    {
        private ObservableCollection<FeedChannel> _FeedChannels = new ObservableCollection<FeedChannel>
        {
            new FeedChannel
            {
                AuthorName = "Author Author",
                Title = "Title Title Title Title Title Title Title Title Title Title",
                Priority = 100,
                AccentColor = int.Parse("FFF09609", System.Globalization.NumberStyles.HexNumber)
            },
            new FeedChannel
            {
                AuthorName = "Author Author",
                Title = "Title Title Title Title Title Title Title Title Title Title",
                Priority = 0,
                AccentColor = int.Parse("FFF09609", System.Globalization.NumberStyles.HexNumber)
            },
            new FeedChannel
            {
                AuthorName = "Author Author",
                Title = "Title Title Title Title Title Title Title Title Title Title",
                Priority = 100,
                AccentColor = int.Parse("FFF09609", System.Globalization.NumberStyles.HexNumber)
            },
            new FeedChannel
            {
                AuthorName = "Author Author",
                Title = "Title Title Title Title Title Title Title Title Title Title",
                Priority = 100,
                AccentColor = int.Parse("FFF09609", System.Globalization.NumberStyles.HexNumber)
            },
        };
        public ObservableCollection<FeedChannel> FeedChannels
        {
            get { return _FeedChannels; }
        }
    }
}
