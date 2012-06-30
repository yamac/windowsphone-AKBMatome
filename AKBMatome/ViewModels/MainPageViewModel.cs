using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using AKBMatome.Data;
using AKBMatome.Navigation;
using AKBMatome.Services;
using Microsoft.Phone.Controls;
using SimpleMvvmToolkit;
using Microsoft.Phone.Shell;
using System.Linq;
using System.Globalization;
using System.Threading;
using System.Windows.Media;

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
            RegisterToReceiveMessages(Constants.MessageTokens.FeedChannelsUpdated, OnFeedChannelsUpdated);
            RegisterToReceiveMessages(Constants.MessageTokens.NotificationUpdated, OnNotificationUpdated);
            this.app = app;
            this.navigator = navigator;
            this.service = service;
            this.dataContext = dataContext;
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

            ObservableCollection<MainPagePivotItemViewModel> items = new ObservableCollection<MainPagePivotItemViewModel>();
            MainPagePivotItemViewModel vm;
            vm = new MainPagePivotItemViewModel(app, navigator, service, dataContext, MainPagePivotItemViewModel.PivotItemType.OshiMem);
            vm.ErrorNotice += OnErrorNotice;
            items.Add(vm);
            vm = new MainPagePivotItemViewModel(app, navigator, service, dataContext, MainPagePivotItemViewModel.PivotItemType.OtherMember);
            vm.ErrorNotice += OnErrorNotice;
            items.Add(vm);
            PivotItems = items;
            initialized = true;
            Helpers.AppSettings.AddOrUpdateValue(Constants.AppKey.LastUpdate, DateTime.Now);
            PivotItems[0].LoadFeedItems(true, false);
            PivotItemSelectedIndex = 0;

            ShellTile shellTile = ShellTile.ActiveTiles.First();
            StandardTileData shellTileData = new StandardTileData()
            {
                Title = null,
                BackgroundImage = null,
                Count = 0,
            };
            shellTile.Update(shellTileData);
        }

        private void OnFeedChannelsUpdated(object sender, NotificationEventArgs e)
        {
            PivotItems[0].LoadFeedItems(true, false);
            PivotItems[1].LoadFeedItems(true, false);
        }

        private void OnNotificationUpdated(object sender, NotificationEventArgs e)
        {
            int[] channelIds = (from channel in dataContext.FeedChannels where channel.Priority == 100 select channel.Id).ToArray();
            string uuid = Helpers.AppSettings.GetValueOrDefault<string>(Constants.AppKey.NotificationUuid, null);
            if (uuid != null)
            {
                CultureInfo uicc = Thread.CurrentThread.CurrentUICulture;
                service.UpdateNotificationChannel(uuid, Helpers.AppAttributes.Version, uicc.Name, channelIds, true, UpdateNotificationChannelCompleted);
            }
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

        private bool initialized = false;

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

        private ObservableCollection<MainPagePivotItemViewModel> _PivotItems = new ObservableCollection<MainPagePivotItemViewModel>();
        public ObservableCollection<MainPagePivotItemViewModel> PivotItems
        {
            get { return _PivotItems; }
            set
            {
                if (_PivotItems == value) return;
                _PivotItems = value;
                NotifyPropertyChanged(m => PivotItems);
            }
        }

        private int _PivotItemSelectedIndex = -1;
        public int PivotItemSelectedIndex
        {
            get { return _PivotItemSelectedIndex; }
            set
            {
                if (_PivotItemSelectedIndex == value) return;
                _PivotItemSelectedIndex = value;
                NotifyPropertyChanged(m => PivotItemSelectedIndex);
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
                    (e.SelectedItem as MainPagePivotItemViewModel).LoadFeedItems(false, false);
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
                    if (!initialized)
                    {
                        LoadAllFeedGroupsAndChannels();
                        return;
                    }
                    (e.SelectedItem as MainPagePivotItemViewModel).LoadFeedItems(true, false);
                }
                ,
                (e) =>
                {
                    if (IsBusy)
                    {
                        return false;
                    }
                    if (!initialized)
                    {
                        return true;
                    }
                    if (e.SelectedItem == null)
                    {
                        return false;
                    }
                    return !(e.SelectedItem as MainPagePivotItemViewModel).IsBusy;
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
            service.GetAllFeedGroupsAndChannels(dataContext, GetAllFeedGroupsAndChannelsCompleted, true);
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