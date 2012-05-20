using Microsoft.Phone.Controls;
using SimpleMvvmToolkit;
using AKBMatome.Services;
using AKBMatome.ViewModels;
using AKBMatome.Data;

namespace AKBMatome.Locators
{
    public class ViewModelLocator
    {
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
                PhoneApplicationFrame app = App.Current.RootVisual as PhoneApplicationFrame;
                INavigator navigator = TheNavigator;
                IAKBMatomeService service = TheAKBMatomeService;
                FeedDataContext dataContext = TheFeedDataContext;
                return new MainPageViewModel(app, navigator, service, dataContext);
            }
        }

        public WebPageViewModel WebPageViewModel
        {
            get
            {
                return new WebPageViewModel();
            }
        }

        public ChannelListPageViewModel ChannelListPageViewModel
        {
            get
            {
                INavigator navigator = TheNavigator;
                FeedDataContext dataContext = TheFeedDataContext;
                return new ChannelListPageViewModel(navigator, dataContext);
            }
        }

        public ChannelDetailPageViewModel ChannelDetailPageViewModel
        {
            get
            {
                INavigator navigator = TheNavigator;
                FeedDataContext dataContext = TheFeedDataContext;
                return new ChannelDetailPageViewModel(navigator, dataContext);
            }
        }

        public PreferencesPageViewModel PreferencesPageViewModel
        {
            get
            {
                PhoneApplicationFrame app = App.Current.RootVisual as PhoneApplicationFrame;
                INavigator navigator = TheNavigator;
                IAKBMatomeService service = TheAKBMatomeService;
                FeedDataContext dataContext = TheFeedDataContext;
                return new PreferencesPageViewModel(app, navigator, service, dataContext);
            }
        }
    }
}