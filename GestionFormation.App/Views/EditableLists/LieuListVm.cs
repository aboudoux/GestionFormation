using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestionFormation.App.Core;
using GestionFormation.Applications.Lieux;
using GestionFormation.CoreDomain.Locations.Queries;

namespace GestionFormation.App.Views.EditableLists
{
    public class LieuListVm : EditableListVm<EditableLieu>
    {
        private readonly ILocationQueries _locationQueries;

        public LieuListVm(IApplicationService applicationService, ILocationQueries locationQueries) : base(applicationService)
        {
            _locationQueries = locationQueries ?? throw new ArgumentNullException(nameof(locationQueries));
        }

        protected override async Task<IReadOnlyList<EditableLieu>> LoadAsync()
        {
            return await Task.Run(() => _locationQueries.GetAll().Select(a=>new EditableLieu(a)).ToList());            
        }

        protected override async Task CreateAsync(EditableLieu item)
        {
            await Task.Run(() => ApplicationService.Command<CreateLieu>().Execute(item.Nom, item.Addresse, item.Places));
        }

        protected override async Task UpdateAsync(EditableLieu item)
        {
            await Task.Run(() => ApplicationService.Command<UpdateLieu>().Execute(item.GetId(), item.Nom, item.Addresse, item.Places));
        }

        protected override async Task DeleteAsync(EditableLieu item)
        {
            await Task.Run(() => ApplicationService.Command<DeleteLieu>().Execute(item.GetId()));
        }

        public override string Title => "liste des lieux";
    }

    public class EditableLieu : EditableItem
    {

        public EditableLieu()
        {            
        }

        public EditableLieu(ILocationResult result) : base(result.LocationId)
        {
            Nom = result.Name;
            Addresse = result.Address;
            Places = result.Seats;
        }


        public string Nom { get; set; }
        public string Addresse { get; set; }
        public int Places { get; set; }
    }
}