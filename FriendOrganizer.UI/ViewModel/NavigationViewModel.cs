using FriendOrganizer.Model;
using FriendOrganizer.UI.Data;
using FriendOrganizer.UI.Event;
using Prism.Events;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace FriendOrganizer.UI.ViewModel
{
    public class NavigationViewModel : ViewModelBase, INavigationViewModel
    {
        private ILookupDataService _lookupDataService;
        private IEventAggregator _eventAggregator;

        public ObservableCollection<NavigationItemViewModel> Friends { get; set; }

        public NavigationViewModel(ILookupDataService lookupDataService, IEventAggregator eventAggregator)
        {
            _lookupDataService = lookupDataService;
            _eventAggregator = eventAggregator;
            Friends = new ObservableCollection<NavigationItemViewModel>();
            _eventAggregator.GetEvent<AfterFriendSavedEvent>().Subscribe(AfterFriendSaved);
        }

        private void AfterFriendSaved(AfterFriendSavedEventArgs obj)
        {
            var lookupItem = Friends.Single(l => l.Id == obj.Id);
            lookupItem.DisplayMember = obj.DisplayMember;
        }

        public async Task LoadAsync()
        {
            var lookup = await _lookupDataService.GetFriendLookupAsync();
            Friends.Clear();
            foreach(var item in lookup)
            {
                Friends.Add(new NavigationItemViewModel(item.Id,item.DisplayMember));
            }
        }

        private NavigationItemViewModel selectedFriend ;

        public NavigationItemViewModel SelectedFriend 
        {
            get { return selectedFriend; }
            set
            {
                selectedFriend = value;
                OnPropertyChanged();
                if(selectedFriend != null)
                {
                    _eventAggregator.GetEvent<OpenFriendDetailViewEvent>()
                        .Publish(selectedFriend.Id);
                }
            }
        }

    }
}
