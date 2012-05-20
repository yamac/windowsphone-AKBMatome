using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using AKBMatome.Data;
using AKBMatome.Navigation;
using AKBMatome.Services;
using Microsoft.Phone.Controls;
using SimpleMvvmToolkit;
using Yamac.Controls;
using System.Globalization;
using System.Threading;

namespace AKBMatome.ViewModels
{
    public class MainPagePivotItemViewModel : ViewModelBase<MainPagePivotItemViewModel>
    {
        public enum PivotItemType
        {
            OshiMem = 1,
            OtherMember = 2
        }

        #region Initialization and Cleanup
        /******************************
         * Initialization and Cleanup *
         ******************************/

        public MainPagePivotItemViewModel() { }

        public MainPagePivotItemViewModel(PhoneApplicationFrame app, INavigator navigator, IAKBMatomeService service, FeedDataContext dataContext, PivotItemType itemType)
        {
            RegisterToReceiveMessages(Constants.MessageTokens.FeedChannelsUpdated, OnFeedChannelsUpdated);
            this.app = app;
            this.navigator = navigator;
            this.service = service;
            this.dataContext = dataContext;
            pivotItemType = itemType;
            switch (pivotItemType)
            {
                case PivotItemType.OshiMem:
                    Title = Localization.AppResources.MainPage_Oshimems;//"推しメン";
                    break;
                case PivotItemType.OtherMember:
                    Title = Localization.AppResources.MainPage_GeneralMembers;//"一般メンバー";
                    break;
            }
        }

        #endregion

        #region Notifications
        /*****************
         * Notifications *
         *****************/

        public event EventHandler<NotificationEventArgs<Exception>> ErrorNotice;

        private void OnFeedChannelsUpdated(object sender, NotificationEventArgs e)
        {
            read = false;
        }

        #endregion

        #region Services
        /************
         * Services *
         ************/

        PhoneApplicationFrame app;
        INavigator navigator;
        IAKBMatomeService service;
        FeedDataContext dataContext;
        PivotItemType pivotItemType;

        #endregion

        #region Properties
        /**************
         * Properties *
         **************/

        private bool read = false;
        private int page = Constants.App.BasePage;

        private bool _IsBusy = false;
        public bool IsBusy
        {
            get { return _IsBusy; }
            set
            {
                if (_IsBusy == value) return;
                _IsBusy = value;
                NotifyPropertyChanged(m => IsBusy);
            }
        }

        private bool _HasNextPage = false;
        public bool HasNextPage
        {
            get { return _HasNextPage; }
            set
            {
                if (_HasNextPage == value) return;
                _HasNextPage = value;
                NotifyPropertyChanged(m => HasNextPage);
            }
        }

        private string _Title;
        public string Title
        {
            get { return _Title; }
            set
            {
                if (_Title == value) return;
                _Title = value;
                NotifyPropertyChanged(m => Title);
            }
        }

        public bool IsOshiMem
        {
            get { return (pivotItemType == PivotItemType.OshiMem ? true : false); }
        }

        private bool _HasMember = true;
        public bool HasMember
        {
            get { return _HasMember; }
            set
            {
                if (_HasMember == value) return;
                _HasMember = value;
                NotifyPropertyChanged(m => HasMember);
            }
        }

        private ObservableCollection<FeedItem> _FeedItems = new ObservableCollection<FeedItem>();
        public ObservableCollection<FeedItem> FeedItems
        {
            get { return _FeedItems; }
            set
            {
                if (_FeedItems == value) return;
                _FeedItems = value;
                NotifyPropertyChanged(m => FeedItems);
            }
        }

        #endregion

        #region Commands
        /************
         * Commands *
         ************/

        public ICommand TheListSelectorSelectionChangedCommand
        {
            get
            {
                return new DelegateCommand<LongListSelector>((e) =>
                {
                    if (e.SelectedItem != null)
                    {
                        NavigationService.Navigate(new Uri("/Views/WebPage.xaml", UriKind.Relative), e.SelectedItem);
                        e.SelectedItem = null;
                    }
                }
                );
            }
        }

        public ICommand TheListSelectorStretchingBottomCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    LoadFeedItems(false, true);
                }
                );
            }
        }

        #endregion

        #region Methods
        /***********
         * Methods *
         ***********/

        public void LoadFeedItems(bool clear, bool next)
        {
            if (IsBusy) return;

            if (clear)
            {
                HasNextPage = true;
                FeedItems.Clear();
                page = Constants.App.BasePage;
            }
            else
            {
                if (read)
                {
                    if (!next) return;
                    if (!HasNextPage) return;
                    page++;
                } else {
                    HasNextPage = true;
                }
            }

            IsBusy = true;
            switch (pivotItemType)
            {
                case PivotItemType.OshiMem:
                    {
                        lock (dataContext)
                        {
                            int[] channelIds = (from channel in dataContext.FeedChannels where channel.Priority == 100 select channel.Id).ToArray();
                            if (channelIds.Length == 0)
                            {
                                HasMember = false;
                                IsBusy = false;
                            }
                            else
                            {
                                HasMember = true;
                                service.GetFeedItems(dataContext, null, channelIds, page, LoadFeedItemsCompleted);
                                string uuid = Helpers.AppSettings.GetValueOrDefault<string>(Constants.AppKey.NotificationUuid, null);
                                if (uuid != null)
                                {
                                    CultureInfo uicc = Thread.CurrentThread.CurrentUICulture;
                                    service.UpdateNotificationChannel(uuid, uicc.Name, channelIds, true, UpdateNotificationChannelCompleted);
                                }
                            }
                        }
                        break;
                    }
                case PivotItemType.OtherMember:
                    {
                        lock (dataContext)
                        {
                            HasMember = true;
                            int[] channelIds = (from channel in dataContext.FeedChannels where channel.Priority != 100 select channel.Id).ToArray();
                            service.GetFeedItems(dataContext, null, channelIds, page, LoadFeedItemsCompleted);
                        }
                        break;
                    }
            }
        }

        #endregion

        #region Completion Callbacks
        /************************
         * Completion Callbacks *
         ************************/

        protected void LoadFeedItemsCompleted(AKBMatomeService.GetFeedItemsResult result, Exception error)
        {
            if (!IsBusy)
            {
                return;
            }

            IsBusy = false;
            read = true;

            if (error != null)
            {
                NotifyError(Localization.AppResources.MainPage_Error_FailedToGetAllFeedGroupsAndChannels, error);
                return;
            }

            foreach (var item in result.FeedItems)
            {
                FeedItems.Add(item);
            }
            HasNextPage = result.HasNext;
        }

        protected void UpdateNotificationChannelCompleted(AKBMatomeService.UpdateNotificationChannelResult result, Exception error)
        {
        }

        #endregion

        #region Helpers
        /***********
         * Helpers *
         ***********/

        private void NotifyError(string message, Exception error)
        {
            Notify(ErrorNotice, new NotificationEventArgs<Exception>(message, error));
        }

        #endregion
    }
}