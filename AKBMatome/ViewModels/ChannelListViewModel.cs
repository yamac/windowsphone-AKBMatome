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
    public class ChannelListViewModel : ViewModelBase<ChannelListViewModel>
    {
        #region Initialization and Cleanup
        /******************************
         * Initialization and Cleanup *
         ******************************/

        public ChannelListViewModel() { }

        public ChannelListViewModel(PhoneApplicationFrame app, INavigator navigator, Services.IAKBMatomeService service, FeedDataContext dataContext)
        {
            RegisterToReceiveMessages(Constants.MessageTokens.FeedGroupsUpdated, OnFeedGroupsUpdated);
            this.app = app;
            this.navigator = navigator;
            this.service = service;
            this.dataContext = dataContext;
            LoadFeedChannels();
        }

        #endregion

        #region Notifications
        /*****************
         * Notifications *
         *****************/

        public event EventHandler<NotificationEventArgs<Exception>> ErrorNotice;

        private void OnFeedGroupsUpdated(object sender, NotificationEventArgs e)
        {
            LoadFeedChannels();
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

        private ObservableCollection<PublicGrouping<FeedGroup, FeedChannel>> _FeedChannels;
        public ObservableCollection<PublicGrouping<FeedGroup, FeedChannel>> FeedChannels
        {
            get { return _FeedChannels; }
            set
            {
                if (_FeedChannels == value) return;
                _FeedChannels = value;
                NotifyPropertyChanged(m => FeedChannels);
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
                        NavigationService.Navigate(new Uri("/Views/ChannelDetailPage.xaml", UriKind.Relative), e.SelectedItem);
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

        private void LoadFeedChannels()
        {
            lock (dataContext)
            {
                var groupedChannels =
                    from channel in dataContext.FeedChannels
                    where channel.FeedGroup.Class == 1 && channel.FeedGroup.Subscribe == 100 && channel.FeedGroup.Status == 1 && channel.Status == 1
                    orderby channel.Priority descending, channel.AuthorName ascending
                    group channel by channel.FeedGroup into grouping
                    select new PublicGrouping<FeedGroup, FeedChannel>(grouping);
                FeedChannels = new ObservableCollection<PublicGrouping<FeedGroup, FeedChannel>>(groupedChannels);
            };
        }

        #endregion

        #region Completion Callbacks
        /************************
         * Completion Callbacks *
         ************************/

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
