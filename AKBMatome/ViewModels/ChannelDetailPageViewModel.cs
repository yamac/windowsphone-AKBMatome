using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using AKBMatome.Data;
using AKBMatome.Navigation;
using Microsoft.Phone.Controls;
using SimpleMvvmToolkit;

namespace AKBMatome.ViewModels
{
    public class ChannelDetailPageViewModel : ViewModelBase<ChannelListPageViewModel>
    {
        #region Initialization and Cleanup
        /******************************
         * Initialization and Cleanup *
         ******************************/

        public ChannelDetailPageViewModel() { }

        public ChannelDetailPageViewModel(INavigator navigator, FeedDataContext dataContext)
        {
            System.Diagnostics.Debug.WriteLine("ChannelDetailPageViewModel");
            this.navigator = navigator;
            this.dataContext = dataContext;
            LoadFeedChannel(((FeedChannel)NavigationService.NavigationArgs).Id);
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

        private ListPicker AccentColorPicker;

        public Collection<NamedSolidColorBrush> AccentColors
        {
            get
            {
                return Constants.Media.AccentColors;
            }
        }

        private FeedChannel _TheFeedChannel;
        public FeedChannel TheFeedChannel
        {
            get { return _TheFeedChannel; }
            set
            {
                if (_TheFeedChannel == value) return;
                _TheFeedChannel = value;
                NotifyPropertyChanged(m => TheFeedChannel);
            }
        }

        #endregion

        #region Commands
        /************
         * Commands *
         ************/

        public ICommand BackKeyPressCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    RejectChanges();
                }
                );
            }
        }


        public ICommand AccentColorPickerInitializedCommand
        {
            get
            {
                return new DelegateCommand<ListPicker>((e) =>
                {
                    AccentColorPicker = e;
                    if (TheFeedChannel != null)
                    {
                        int i = 0;
                        var en = e.Items.GetEnumerator();
                        while (en.MoveNext())
                        {
                            var color = (NamedSolidColorBrush)en.Current;
                            var colorInt = int.Parse(color.Brush.Color.ToString().TrimStart('#'), System.Globalization.NumberStyles.HexNumber);
                            if (colorInt == TheFeedChannel.AccentColor)
                            {
                                e.SelectedIndex = i;
                            }
                            i++;
                        }
                    }
                }
                );
            }
        }

        public ICommand AccentColorSelectionChangedCommand
        {
            get
            {
                return new DelegateCommand<ListPicker>((e) =>
                {
                    var color = (NamedSolidColorBrush)e.SelectedItem;
                    var colorInt = int.Parse(color.Brush.Color.ToString().TrimStart('#'), System.Globalization.NumberStyles.HexNumber);
                    TheFeedChannel.AccentColor = colorInt;
                }
                );
            }
        }

        public ICommand OkCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    SubmitChanges();
                    NavigationService.GoBack();
                }
                );
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    RejectChanges();
                    NavigationService.GoBack();
                }
                );
            }
        }

        #endregion

        #region Methods
        /***********
         * Methods *
         ***********/

        private void LoadFeedChannel(int id)
        {
            TheFeedChannel = dataContext.FeedChannels.Single(channel => channel.Id == id);
        }

        private void SubmitChanges()
        {
            dataContext.SubmitChanges();
            SendMessage(Constants.MessageTokens.FeedChannelsUpdated, new NotificationEventArgs());
        }

        private void RejectChanges()
        {
            dataContext.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, TheFeedChannel);
            SendMessage(Constants.MessageTokens.FeedChannelsUpdated, new NotificationEventArgs());
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
