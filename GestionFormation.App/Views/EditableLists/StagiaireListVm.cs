using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestionFormation.App.Core;
using GestionFormation.Applications.Stagiaires;
using GestionFormation.CoreDomain.Stagiaires.Queries;

namespace GestionFormation.App.Views.EditableLists
{
    public class StagiaireListVm : EditableListVm<EditableStagiaire>
    {
        private readonly IStagiaireQueries _stagiaireQueries;

        public override string Title => "Liste des stagiaires";

        public StagiaireListVm(IApplicationService applicationService, IStagiaireQueries stagiaireQueries) : base(applicationService)
        {
            _stagiaireQueries = stagiaireQueries ?? throw new ArgumentNullException(nameof(stagiaireQueries));
        }

        protected override async Task<IReadOnlyList<EditableStagiaire>> LoadAsync()
        {
            return await Task.Run(()=>_stagiaireQueries.GetAll().Select(a=>new EditableStagiaire(a)).ToList());
        }

        protected override async Task CreateAsync(EditableStagiaire item)
        {
            await Task.Run(()=> ApplicationService.Command<CreateStagiaire>().Execute(item.Nom, item.Prenom));
        }

        protected override async Task UpdateAsync(EditableStagiaire item)
        {
            await Task.Run(()=>ApplicationService.Command<UpdateStagiaire>().Execute(item.GetId(), item.Nom, item.Prenom));
        }

        protected override async Task DeleteAsync(EditableStagiaire item)
        {
            await Task.Run(() => ApplicationService.Command<DeleteStagiaire>().Execute(item.GetId()));
        }        
    }

    public class EditableStagiaire : EditableItem
    {
        public EditableStagiaire()
        {
            
        }
        public EditableStagiaire(IStagiaireResult result) : base(result.Id)
        {
            Nom = result.Nom;
            Prenom = result.Prenom;
        }

        public string Nom { get; set; }
        public string Prenom { get; set; }
    }
}