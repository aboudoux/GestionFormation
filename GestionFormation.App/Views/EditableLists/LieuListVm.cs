using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestionFormation.App.Core;
using GestionFormation.Applications.Lieux;
using GestionFormation.CoreDomain.Lieux.Queries;

namespace GestionFormation.App.Views.EditableLists
{
    public class LieuListVm : EditableListVm<EditableLieu>
    {
        private readonly ILieuQueries _lieuQueries;

        public LieuListVm(IApplicationService applicationService, ILieuQueries lieuQueries) : base(applicationService)
        {
            _lieuQueries = lieuQueries ?? throw new ArgumentNullException(nameof(lieuQueries));
        }

        protected override async Task<IReadOnlyList<EditableLieu>> LoadAsync()
        {
            return await Task.Run(() => _lieuQueries.GetAll().Select(a=>new EditableLieu(a)).ToList());            
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

        public EditableLieu(ILieuResult result) : base(result.LieuId)
        {
            Nom = result.Nom;
            Addresse = result.Addresse;
            Places = result.Places;
        }


        public string Nom { get; set; }
        public string Addresse { get; set; }
        public int Places { get; set; }
    }
}