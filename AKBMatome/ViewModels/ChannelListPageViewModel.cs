using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using AKBMatome.Data;
using AKBMatome.Navigation;
using SimpleMvvmToolkit;
using Microsoft.Phone.Controls;

namespace AKBMatome.ViewModels
{
    public class ChannelListPageViewModel : ViewModelBase<ChannelListPageViewModel>
    {
        #region Initialization and Cleanup
        /******************************
         * Initialization and Cleanup *
         ******************************/

        public ChannelListPageViewModel() { }

        public ChannelListPageViewModel(INavigator navigator,FeedDataContext dataContext)
        {
            System.Diagnostics.Debug.WriteLine("ChannelListPageViewModel");
            this.navigator = navigator;
            this.dataContext = dataContext;
            LoadFeedChannels();
        }

        #endregion

        #region Notifications
        /*****************
         * Notifications *
         *****************/

        public event EventHandler<NotificationEventArgs<Exception>> ErrorNotice;

        #endregion

        #region Services
        /************
         * Services *
         ************/

        INavigator navigator;
        FeedDataContext dataContext;

        #endregion

        #region Properties
        /**************
         * Properties *
         **************/

        private ObservableCollection<FeedChannel> _FeedChannels;
        public ObservableCollection<FeedChannel> FeedChannels
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

        public ICommand FeedChannelSelectionChangedCommand
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
            FeedChannel[] channels = (from channel in dataContext.FeedChannels orderby channel.Priority descending select channel).ToArray();
            FeedChannels = new ObservableCollection<FeedChannel>(channels);
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
