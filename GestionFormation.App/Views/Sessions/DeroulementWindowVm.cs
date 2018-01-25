using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GestionFormation.App.Core;
using GestionFormation.CoreDomain.Places.Queries;

namespace GestionFormation.App.Views.Sessions
{
    public class DeroulementWindowVm : PopupWindowVm
    {
        private readonly IPlacesQueries _placesQueries;
        private readonly Guid _sessionId;
        private ObservableCollection<IPlaceValidatedResult> _places;
        private IPlaceValidatedResult _selectedPlace;

        public DeroulementWindowVm(IPlacesQueries placesQueries, Guid sessionId)
        {
            _placesQueries = placesQueries ?? throw new ArgumentNullException(nameof(placesQueries));
            _sessionId = sessionId;
            RefreshCommand = new RelayCommandAsync(ExecuteRefreshAsync);
        }

        public ObservableCollection<IPlaceValidatedResult> Places
        {
            get => _places;
            set { Set(()=>Places, ref _places, value); }
        }

        public IPlaceValidatedResult SelectedPlace
        {
            get => _selectedPlace;
            set { Set(()=>SelectedPlace, ref _selectedPlace, value); }
        }

        private async Task ExecuteRefreshAsync()
        {
            Places = new ObservableCollection<IPlaceValidatedResult>( await Task.Run(()=>_placesQueries.GetValidatedPlaces(_sessionId)));
        }

        public override async Task Init()
        {
            await RefreshCommand.ExecuteAsync();
        }

        public RelayCommandAsync RefreshCommand { get; }
    }
}