using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using AKBMatome.Data;
using AKBMatome.Navigation;
using AKBMatome.Services;
using Helpers;
using Microsoft.Phone.Controls;
using SimpleMvvmToolkit;

namespace AKBMatome.ViewModels
{
    public class GroupListPageViewModel : ViewModelBase<ChannelListPageViewModel>
    {
        #region Initialization and Cleanup
        /******************************
         * Initialization and Cleanup *
         ******************************/

        public GroupListPageViewModel() { }

        public GroupListPageViewModel(PhoneApplicationFrame app, INavigator navigator, Services.IAKBMatomeService service, FeedDataContext dataContext)
        {
            RegisterToReceiveMessages(Constants.MessageTokens.GroupListInitializeCompleted, OnInitializeCompleted);
            this.app = app;
            this.navigator = navigator;
            this.service = service;
            this.dataContext = dataContext;
            LoadAllFeedGroups();
        }

        #endregion

        #region Notifications
        /*****************
         * Notifications *
         *****************/

        public event EventHandler<NotificationEventArgs<Exception>> ErrorNotice;

        private void OnInitializeCompleted(object sender, NotificationEventArgs e)
        {
            LoadFeedGroups();
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

        private ObservableCollection<FeedGroup> _FeedGroups;
        public ObservableCollection<FeedGroup> FeedGroups
        {
            get { return _FeedGroups; }
            set
            {
                if (_FeedGroups == value) return;
                _FeedGroups = value;
                NotifyPropertyChanged(m => FeedGroups);
            }
        }

        #endregion

        #region Commands
        /************
         * Commands *
         ************/

        public ICommand ListSelectionChangedCommand
        {
            get
            {
                return new DelegateCommand<LongListSelector>((e) =>
                {
                    if (e.SelectedItem != null)
                    {
                        NavigationService.Navigate(new Uri("/Views/GroupDetailPage.xaml", UriKind.Relative), e.SelectedItem);
                        e.SelectedItem = null;
                    }
                }
                );
            }
        }

        #endregion

        #region Methods
        /***********
         * Methods *
         ***********/

        private void LoadAllFeedGroups()
        {
            IsBusy = true;
            service.GetAllFeedGroups(dataContext, GetAllFeedGroupsCompleted, true);
        }

        private void LoadFeedGroups()
        {
            lock (dataContext)
            {
                var groups =
                    from theGroup in dataContext.FeedGroups
                    where theGroup.Class == 1 && theGroup.Status == 1
                    orderby theGroup.Title ascending
                    select theGroup;
                FeedGroups = new ObservableCollection<FeedGroup>(groups);
            };
        }

        #endregion

        #region Completion Callbacks
        /************************
         * Completion Callbacks *
         ************************/

        private void GetAllFeedGroupsCompleted(Exception error)
        {
            if (!IsBusy)
            {
                return;
            }

            IsBusy = false;

            if (error != null)
            {
                NotifyError(Localization.AppResources.MainPage_Error_FailedToGetAllFeedGroups, error);
                return;
            }

            SendMessage(Constants.MessageTokens.GroupListInitializeCompleted, new NotificationEventArgs());
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
