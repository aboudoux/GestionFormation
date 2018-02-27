using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GestionFormation.App.Core;
using GestionFormation.EventStore;
using GestionFormation.EventStore.Queries;
using GestionFormation.Kernel;

namespace GestionFormation.App.Views.Admins.History
{
    public class HistoryWindowVm : ViewModelBase, ILoadableVm
    {
        private readonly IEventQueries _eventQueries;
        private readonly IEventSerializer _eventSerializer;
        private bool _isLoading;
        private ObservableCollection<EventItem> _items;
        private EventItem _selectedItem;
        private object _selectedItemData;

        public HistoryWindowVm(IEventQueries eventQueries, IEventSerializer eventSerializer)
        {
            _eventQueries = eventQueries ?? throw new ArgumentNullException(nameof(eventQueries));
            _eventSerializer = eventSerializer ?? throw new ArgumentNullException(nameof(eventSerializer));

            LoadCommand = new RelayCommandAsync(ExecuteLoadAsync);
        }

        public string Title => "Historique";


        public bool IsLoading
        {
            get => _isLoading;
            set { Set(()=>IsLoading, ref _isLoading, value); }
        }

        public ObservableCollection<EventItem> Items
        {
            get => _items;
            set { Set(()=>Items, ref _items, value); }
        }

        public EventItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                Set(()=>SelectedItem, ref _selectedItem, value);                
            }
        }

        public object SelectedItemData
        {
            get => _selectedItemData;
            set { Set(()=>SelectedItemData, ref _selectedItemData, value); }
        }

        public RelayCommandAsync LoadCommand { get; }
        private async Task ExecuteLoadAsync()
        {
            try
            {
                IsLoading = true;
                var data = await Task.Run(() => _eventQueries.GetAll().Select(a=>new EventItem(a, _eventSerializer)));
                Items = new ObservableCollection<EventItem>(data);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }

    public class EventItem
    {
        public EventItem(IEventResult result, IEventSerializer serializer)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));
            if (serializer == null) throw new ArgumentNullException(nameof(serializer));

            var @event = serializer.Deserialize<IDomainEvent>(result.Data);
            EventName = @event.ToString();
            UserName = result.User;
            Data = @event;
            TimeStamp = result.TimeStamp;
        }
        public string EventName { get; }
        public string UserName { get; }
        public object Data { get; }
        public DateTime TimeStamp { get; }
    }
}