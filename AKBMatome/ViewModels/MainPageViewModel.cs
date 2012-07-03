using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using AKBMatome.Data;
using AKBMatome.Navigation;
using AKBMatome.Services;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SimpleMvvmToolkit;

namespace AKBMatome.ViewModels
{
    public class MainPageViewModel : ViewModelBase<MainPageViewModel>
    {
        #region Initialization and Cleanup
        /******************************
         * Initialization and Cleanup *
         ******************************/

        public MainPageViewModel() { }

        public MainPageViewModel(PhoneApplicationFrame app, INavigator navigator, Services.IAKBMatomeService service, FeedDataContext dataContext)
        {
            RegisterToReceiveMessages(Constants.MessageTokens.InitializeCompleted, OnInitializeCompleted);
            RegisterToReceiveMessages(Constants.MessageTokens.FeedGroupsUpdated, OnFeedGroupsOrChannelsUpdated);
            RegisterToReceiveMessages(Constants.MessageTokens.FeedChannelsUpdated, OnFeedGroupsOrChannelsUpdated);
            RegisterToReceiveMessages(Constants.MessageTokens.NotificationUpdated, OnNotificationUpdated);
            this.app = app;
            this.navigator = navigator;
            this.service = service;
            this.dataContext = dataContext;
            IsInitializing = true;
            if (!IsInDesignMode)
            {
                DateTime lastUpdate = Helpers.AppSettings.GetValueOrDefault<DateTime>(Constants.AppKey.LastUpdate, DateTime.MinValue);
                if (DateTime.Compare(lastUpdate, DateTime.Now.AddDays(-Constants.App.FeedChannelExpireDays)) < 0)
                {
                    LoadAllFeedGroupsAndChannels();
                }
                else
                {
                    SendMessage(Constants.MessageTokens.InitializeCompleted, new NotificationEventArgs());
                }
            }
        }

        #endregion

        #region Notifications
        /*****************
         * Notifications *
         *****************/

        public event EventHandler<NotificationEventArgs<Exception>> ErrorNotice;

        private void OnInitializeCompleted(object sender, NotificationEventArgs e)
        {
            /*
            System.Diagnostics.Debug.WriteLine(
                int.Parse(
                    "FF15181d",
                    System.Globalization.NumberStyles.HexNumber
                )
            );
            */
            string uuid = Helpers.AppSettings.GetValueOrDefault<string>(Constants.AppKey.NotificationUuid, null);
            if (uuid != null)
            {
                CultureInfo uicc = Thread.CurrentThread.CurrentUICulture;
                service.UpdateNotificationChannel(uuid, Helpers.AppAttributes.Version, uicc.Name, null, true, UpdateNotificationChannelCompleted);
            }

            InitPivotItems();
            LoadPivotItem(0, true);

            ShellTile shellTile = ShellTile.ActiveTiles.First();
            StandardTileData shellTileData = new StandardTileData()
            {
                Title = null,
                BackgroundImage = null,
                Count = 0,
            };
            shellTile.Update(shellTileData);

            IsInitializing = false;
        }

        private void OnFeedGroupsOrChannelsUpdated(object sender, NotificationEventArgs e)
        {
            LoadPivotItem(0, true);
            LoadPivotItem(1, true);
        }

        private void OnNotificationUpdated(object sender, NotificationEventArgs e)
        {
            lock (dataContext)
            {
                int[] channelIds =
                    (
                        from channel in dataContext.FeedChannels
                        where channel.Priority == 100 && channel.FeedGroup.Class == 1 && channel.FeedGroup.Subscribe == 100
                        select channel.Id
                    )
                    .ToArray();
                string uuid = Helpers.AppSettings.GetValueOrDefault<string>(Constants.AppKey.NotificationUuid, null);
                if (uuid != null)
                {
                    CultureInfo uicc = Thread.CurrentThread.CurrentUICulture;
                    service.UpdateNotificationChannel(uuid, Helpers.AppAttributes.Version, uicc.Name, channelIds, true, UpdateNotificationChannelCompleted);
                }
            };
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

        #endregion

        #region Properties
        /**************
         * Properties *
         **************/

        private bool _IsInitializing = true;
        public bool IsInitializing
        {
            get { return _IsInitializing; }
            set
            {
                if (_IsInitializing == value) return;
                _IsInitializing = value;
                NotifyPropertyChanged(m => IsInitializing);
            }
        }

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

        private ChannelsUpdatesListViewModel _OshimemChannelsUpdatesListViewModel = null;
        public ChannelsUpdatesListViewModel OshimemChannelsUpdatesListViewModel
        {
            get { return _OshimemChannelsUpdatesListViewModel; }
            set
            {
                if (_OshimemChannelsUpdatesListViewModel == value) return;
                _OshimemChannelsUpdatesListViewModel = value;
                NotifyPropertyChanged(m => OshimemChannelsUpdatesListViewModel);
            }
        }

        private ChannelsUpdatesListViewModel _MemberChannelsUpdatesListViewModel = null;
        public ChannelsUpdatesListViewModel MemberChannelsUpdatesListViewModel
        {
            get { return _MemberChannelsUpdatesListViewModel; }
            set
            {
                if (_MemberChannelsUpdatesListViewModel == value) return;
                _MemberChannelsUpdatesListViewModel = value;
                NotifyPropertyChanged(m => MemberChannelsUpdatesListViewModel);
            }
        }

        private ChannelsUpdatesListViewModel _MatomeChannelsUpdatesListViewModel = null;
        public ChannelsUpdatesListViewModel MatomeChannelsUpdatesListViewModel
        {
            get { return _MatomeChannelsUpdatesListViewModel; }
            set
            {
                if (_MatomeChannelsUpdatesListViewModel == value) return;
                _MatomeChannelsUpdatesListViewModel = value;
                NotifyPropertyChanged(m => MatomeChannelsUpdatesListViewModel);
            }
        }

        #endregion

        #region Commands
        /************
         * Commands *
         ************/

        public ICommand PivotSelectionChangedCommand
        {
            get
            {
                return new DelegateCommand<Pivot>(
                (e) =>
                {
                    LoadPivotItem(e.SelectedIndex, false);
                }
                );
            }
        }

        public ICommand RefreshCommand
        {
            get
            {
                return new DelegateCommand<Pivot>(
                (e) =>
                {
                    LoadPivotItem(e.SelectedIndex, true);
                }
                ,
                (e) =>
                {
                    if (IsBusy)
                    {
                        return false;
                    }
                    if (e.SelectedItem == null)
                    {
                        return false;
                    }
                    switch (e.SelectedIndex)
                    {
                        case 0:
                            {
                                return !OshimemChannelsUpdatesListViewModel.IsBusy;
                            }
                        case 1:
                            {
                                return !MemberChannelsUpdatesListViewModel.IsBusy;
                            }
                        case 2:
                            {
                                return !MatomeChannelsUpdatesListViewModel.IsBusy;
                            }
                    }
                    return false;
                }
                );
            }
        }

        public ICommand GotoPreferencesPageCommand
        {
            get
            {
                return new DelegateCommand(
                () =>
                {
                    NavigationService.Navigate(new Uri("/Views/PreferencesPage.xaml", UriKind.Relative));
                }
                ,
                () =>
                {
                    return !IsBusy;
                }
                );
            }
        }

        #endregion

        #region Methods
        /***********
         * Methods *
         ***********/

        public void LoadAllFeedGroupsAndChannels()
        {
            IsBusy = true;
            service.GetAllFeedGroupsAndChannels(dataContext, GetAllFeedGroupsAndChannelsCompleted, false);
        }

        private void InitPivotItems()
        {
            OshimemChannelsUpdatesListViewModel =
                new ChannelsUpdatesListViewModel
                (
                    app, navigator, service, dataContext,
                    true,
                    Localization.AppResources.MainPage_OshimemChannelsUpdatesList_Title
                );
            OshimemChannelsUpdatesListViewModel.ErrorNotice += OnErrorNotice;

            MemberChannelsUpdatesListViewModel =
                new ChannelsUpdatesListViewModel
                (
                    app, navigator, service, dataContext,
                    false,
                    Localization.AppResources.MainPage_MemberChannelsUpdatesList_Title
                );
            MemberChannelsUpdatesListViewModel.ErrorNotice += OnErrorNotice;

            MatomeChannelsUpdatesListViewModel =
                new ChannelsUpdatesListViewModel
                (
                    app, navigator, service, dataContext,
                    false,
                    Localization.AppResources.MainPage_MatomeChannelsUpdatesList_Title
                );
            MatomeChannelsUpdatesListViewModel.ErrorNotice += OnErrorNotice;
        }

        private void LoadPivotItem(int id, bool force)
        {
            switch (id)
            {
                case 0:
                    {
                        if (force || OshimemChannelsUpdatesListViewModel.FeedItems.Count == 0)
                        {
                            lock (dataContext)
                            {
                                int[] channelIds =
                                    (
                                        from channel in dataContext.FeedChannels
                                        where channel.Priority == 100 && channel.FeedGroup.Class == 1 && channel.FeedGroup.Subscribe == 100
                                        select channel.Id
                                    )
                                    .ToArray();
                                OshimemChannelsUpdatesListViewModel.SetChannelIdsAndClass(channelIds, 1);
                            };
                            OshimemChannelsUpdatesListViewModel.LoadFeedItems(true, false);
                        }
                        break;
                    }
                case 1:
                    {
                        if (force || MemberChannelsUpdatesListViewModel.FeedItems.Count == 0)
                        {
                            lock (dataContext)
                            {
                                int[] channelIds =
                                    (
                                        from channel in dataContext.FeedChannels
                                        where channel.Priority != 100 && channel.FeedGroup.Class == 1 && channel.FeedGroup.Subscribe == 100
                                        select channel.Id
                                    )
                                    .ToArray();
                                MemberChannelsUpdatesListViewModel.SetChannelIdsAndClass(channelIds, 1);
                            };
                            MemberChannelsUpdatesListViewModel.LoadFeedItems(true, false);
                    }
                        break;
                    }
                case 2:
                    {
                        if (force || MatomeChannelsUpdatesListViewModel.FeedItems.Count == 0)
                        {
                            MatomeChannelsUpdatesListViewModel.SetChannelIdsAndClass(null, 2);
                            MatomeChannelsUpdatesListViewModel.LoadFeedItems(true, false);
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

        void UpdateNotificationChannelCompleted(AKBMatomeService.UpdateNotificationChannelResult result, Exception error)
        {
        }

        private void GetAllFeedGroupsAndChannelsCompleted(Exception error)
        {
            if (!IsBusy)
            {
                return;
            }

            IsBusy = false;

            if (error != null)
            {
                NotifyError(
                    Localization.AppResources.MainPage_Error_FailedToGetAllFeedGroupsAndChannels, error);
                return;
            }

            SendMessage(Constants.MessageTokens.InitializeCompleted, new NotificationEventArgs());
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

        private void OnErrorNotice(object sender, NotificationEventArgs<Exception> e)
        {
            Notify(ErrorNotice, e);
        }

        #endregion
    }
}