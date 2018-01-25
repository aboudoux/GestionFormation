using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestionFormation.App.Core;
using GestionFormation.Applications.Societes;
using GestionFormation.CoreDomain.Societes.Queries;

namespace GestionFormation.App.Views.EditableLists
{
    public class SocieteListVm : EditableListVm<EditableSociete>
    {
        private readonly ISocieteQueries _societeQueries;

        public override string Title => "Liste des sociétés";

        public SocieteListVm(IApplicationService applicationService, ISocieteQueries societeQueries) : base(applicationService)
        {
            _societeQueries = societeQueries ?? throw new ArgumentNullException(nameof(societeQueries));
        }

        protected override async Task<IReadOnlyList<EditableSociete>> LoadAsync()
        {
            return await Task.Run(() => _societeQueries.GetAll().Select(a => new EditableSociete(a)).ToList());
        }

        protected override async Task CreateAsync(EditableSociete item)
        {
            await Task.Run(()=> ApplicationService.Command<CreateSociete>().Execute(item.Nom, item.Adresse, item.CodePostal, item.Ville));
        }

        protected override async Task UpdateAsync(EditableSociete item)
        {
            await Task.Run(()=> ApplicationService.Command<UpdateSociete>().Execute(item.GetId(), item.Nom, item.Adresse, item.CodePostal, item.Ville));
        }

        protected override async Task DeleteAsync(EditableSociete item)
        {
            await Task.Run(()=>ApplicationService.Command<DeleteSociete>().Execute(item.GetId()));
        }        
    }

    public class EditableSociete : EditableItem
    {
        public EditableSociete()
        {
            
        }

        public EditableSociete(ISocieteResult result) : base(result.SocieteId)
        {
            Nom = result.Nom;
            Adresse = result.Adresse;
            CodePostal = result.Codepostal;
            Ville = result.Ville;
        }        
        

        public string Nom { get; set; }
        public string Adresse { get; set; }
        public string CodePostal { get; set; }
        public string Ville { get; set; }
    }
}