using System.Collections.Generic;
using System.Collections.ObjectModel;
using AKBMatome.Data;

namespace AKBMatome.SampleData
{
    public class ChannelListSampleData
    {
        public class FeedChannelsInFeedGroup : ObservableCollection<FeedChannel>
        {
            public FeedChannelsInFeedGroup(FeedGroup group)
            {
                Key = group;
            }

            public FeedGroup Key { get; set; }
        }

        private ObservableCollection<FeedChannelsInFeedGroup> _FeedChannels;
        public ObservableCollection<FeedChannelsInFeedGroup> FeedChannels
        {
            get
            {
                if (_FeedChannels == null)
                {
                    _FeedChannels = new ObservableCollection<FeedChannelsInFeedGroup>();
                    List<FeedGroup> groups = new List<FeedGroup>
                    {
                        new FeedGroup
                        {
                            Id = 1,
                            Title = "Group1 Title",
                            Class = 1,
                            AccentColor = int.Parse("FFF09609", System.Globalization.NumberStyles.HexNumber)
                        },
                        new FeedGroup
                        {
                            Id = 2,
                            Title = "Group2 Title",
                            Class = 1,
                            AccentColor = int.Parse("FFF09609", System.Globalization.NumberStyles.HexNumber)
                        }
                    };
                    foreach (var group in groups)
                    {
                        FeedChannelsInFeedGroup groupedChannels = new FeedChannelsInFeedGroup(group);
                        groupedChannels.Add(
                            new FeedChannel
                            {
                                FeedGroupId = 1,
                                FeedGroup = group,
                                AuthorName = "Author1 foofoofoofoofoo",
                                Title = "Title1 foofoofoofoofoo",
                                Priority = 100,
                                AccentColor = int.Parse("FFF09609", System.Globalization.NumberStyles.HexNumber)
                            }
                        );
                        groupedChannels.Add(
                            new FeedChannel
                            {
                                FeedGroupId = 1,
                                FeedGroup = group,
                                AuthorName = "Author2 foofoofoofoofoo",
                                Title = "Title2 foofoofoofoofoo",
                                Priority = 100,
                                AccentColor = int.Parse("FFF09609", System.Globalization.NumberStyles.HexNumber)
                            }
                        );
                        groupedChannels.Add(
                            new FeedChannel
                            {
                                FeedGroupId = 1,
                                FeedGroup = group,
                                AuthorName = "Author3 foofoofoofoofoo",
                                Title = "Title3 foofoofoofoofoo",
                                Priority = 100,
                                AccentColor = int.Parse("FFF09609", System.Globalization.NumberStyles.HexNumber)
                            }
                        );
                        _FeedChannels.Add(groupedChannels);
                    }
                }
                return _FeedChannels;
            }
        }
    }
}
