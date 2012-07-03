using Microsoft.Phone.Controls;
using SimpleMvvmToolkit;
using AKBMatome.Services;
using AKBMatome.ViewModels;
using AKBMatome.Data;

namespace AKBMatome.Locators
{
    public class ViewModelLocator
    {
        private PhoneApplicationFrame TheApp
        {
            get
            {
                return App.Current.RootVisual as PhoneApplicationFrame;
            }
        }

        private INavigator _TheNavigator;
        private INavigator TheNavigator
        {
            get
            {
                if (_TheNavigator == null)
                {
                    _TheNavigator = new Navigator();
                }
                return _TheNavigator;
            }
        }

        private static IAKBMatomeService _TheAKBMatomeService;
        private IAKBMatomeService TheAKBMatomeService
        {
            get
            {
                if (_TheAKBMatomeService == null)
                {
                    _TheAKBMatomeService = new AKBMatomeService();
                }
                return _TheAKBMatomeService;
            }
        }

        private static FeedDataContext _TheFeedDataContext;
        private FeedDataContext TheFeedDataContext
        {
            get
            {
                if (_TheFeedDataContext == null)
                {
                    _TheFeedDataContext = new FeedDataContext();
                }
                return _TheFeedDataContext;
            }
        }

        public MainPageViewModel MainPageViewModel
        {
            get
            {
                return new MainPageViewModel(TheApp, TheNavigator, TheAKBMatomeService, TheFeedDataContext);
            }
        }

        public WebPageViewModel WebPageViewModel
        {
            get
            {
                return new WebPageViewModel();
            }
        }

        public GroupListPageViewModel GroupListPageViewModel
        {
            get
            {
                return new GroupListPageViewModel(TheApp, TheNavigator, TheAKBMatomeService, TheFeedDataContext);
            }
        }

        public GroupDetailPageViewModel GroupDetailPageViewModel
        {
            get
            {
                return new GroupDetailPageViewModel(TheNavigator, TheFeedDataContext);
            }
        }

        public ChannelListPageViewModel ChannelListPageViewModel
        {
            get
            {
                return new ChannelListPageViewModel(TheApp, TheNavigator, TheAKBMatomeService, TheFeedDataContext);
            }
        }

        public ChannelDetailPageViewModel ChannelDetailPageViewModel
        {
            get
            {
                return new ChannelDetailPageViewModel(TheNavigator, TheFeedDataContext);
            }
        }

        public PreferencesPageViewModel PreferencesPageViewModel
        {
            get
            {
                return new PreferencesPageViewModel(TheApp, TheNavigator, TheAKBMatomeService, TheFeedDataContext);
            }
        }
    }
}