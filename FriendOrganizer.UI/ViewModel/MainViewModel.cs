using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.View.Services;
using Prism.Events;
using System;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private IEventAggregator _eventAggregator;
        private IFriendDetailViewModel _friendDetailViewModel;
        private Func<IFriendDetailViewModel> _friendDetailViewModelCreator;
        private IMessageDialogService _messageDialogService;

        public INavigationViewModel NavigationViewModel { get; }

        public MainViewModel(INavigationViewModel navigationViewModel, Func<IFriendDetailViewModel> friendDetailViewModelCreator, IEventAggregator eventAggregator, IMessageDialogService messageDialogService)
        {
            _friendDetailViewModelCreator = friendDetailViewModelCreator;
            _eventAggregator = eventAggregator;
            _messageDialogService = messageDialogService;

            _eventAggregator.GetEvent<OpenFriendDetailViewEvent>().Subscribe(OnOpenFriendDetailViewAsync);

            NavigationViewModel = navigationViewModel;
        }

        public IFriendDetailViewModel FriendDetailViewModel
        {
            get { return _friendDetailViewModel; }
            private set
            {
                _friendDetailViewModel = value;
                OnPropertyChanged();
            }
        }

        public async Task LoadAsync()
        {
            await NavigationViewModel.LoadAsync();
        }

        private async void OnOpenFriendDetailViewAsync(int friendId)
        {

            if (FriendDetailViewModel != null && FriendDetailViewModel.HasChanges)
            {
                var result = _messageDialogService.ShowOkCancelDialog("You have made changes. Navigate away?", "Question");

                if (result == MessageDialogResult.Cancel)
                {
                    return;
                }
            }

            FriendDetailViewModel = _friendDetailViewModelCreator();

            await FriendDetailViewModel.LoadAsync(friendId);
        }
    }
}