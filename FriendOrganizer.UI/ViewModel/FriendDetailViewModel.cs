using FriendOrganizer.Model;
using FriendOrganizer.UI.Data;
using FriendOrganizer.UI.Event;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FriendOrganizer.UI.ViewModel
{
    public class FriendDetailViewModel : ViewModelBase, IFriendDetailViewModel
    {
        private IFriendDataService _friendDataService;
        private IEventAggregator _eventAggregator;

        public FriendDetailViewModel(IFriendDataService friendDataService, IEventAggregator eventAggregator)
        {
            _friendDataService = friendDataService;
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<Event.OpenFriendDetailViewEvent>()
                .Subscribe(OnOpenFriendDetailView);
            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
        }

        private bool OnSaveCanExecute()
        {
            
            return true;
        }

        private async void OnSaveExecute()
        {
            await _friendDataService.SaveAsync(Friend);
            _eventAggregator.GetEvent<AfterFriendSavedEvent>().Publish(new AfterFriendSavedEventArgs { Id = Friend.Id, DisplayMember = $"{Friend.FirstName} {Friend.LastName}" });
        }

        private async void OnOpenFriendDetailView(int friendId)
        {
            await LoadAsync(friendId);
        }

        public async Task LoadAsync(int friendId)
        {
            Friend = await _friendDataService.GetByIdAsync(friendId);
        }

        private Friend _friend;
        public Friend Friend
        {
            get
            {
                return _friend;
            }
            private set 
            {
                _friend = value;
                OnPropertyChanged();
            }
        }
        public ICommand SaveCommand { get; }
    }
}
