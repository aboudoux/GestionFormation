using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GestionFormation.App.Core;
using GestionFormation.Applications.Locations;
using GestionFormation.CoreDomain.Locations.Queries;

namespace GestionFormation.App.Views.EditableLists
{
    public class LocationListVm : EditableListVm<EditableLocation>
    {
        private readonly ILocationQueries _locationQueries;

        public LocationListVm(IApplicationService applicationService, ILocationQueries locationQueries) : base(applicationService)
        {
            _locationQueries = locationQueries ?? throw new ArgumentNullException(nameof(locationQueries));
        }

        protected override async Task<IReadOnlyList<EditableLocation>> LoadAsync()
        {
            return await Task.Run(() => _locationQueries.GetAll().Select(a=>new EditableLocation(a)).ToList());            
        }

        protected override async Task CreateAsync(EditableLocation item)
        {
            await Task.Run(() => ApplicationService.Command<CreateLocation>().Execute(item.Name, item.Address, item.Seats));
        }

        protected override async Task UpdateAsync(EditableLocation item)
        {
            await Task.Run(() => ApplicationService.Command<UpdateLocation>().Execute(item.GetId(), item.Name, item.Address, item.Seats));
        }

        protected override async Task DeleteAsync(EditableLocation item)
        {
            await Task.Run(() => ApplicationService.Command<DeleteLocation>().Execute(item.GetId()));
        }

        public override string Title => "liste des lieux";
    }

    public class EditableLocation : EditableItem
    {

        public EditableLocation()
        {            
        }

        public EditableLocation(ILocationResult result) : base(result.LocationId)
        {
            Name = result.Name;
            Address = result.Address;
            Seats = result.Seats;
        }

        [DisplayName("Nom")]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        [DisplayName("Adresse")]
        public string Address { get; set; }
        [DisplayName("Places")]
        public int Seats { get; set; }
    }
}