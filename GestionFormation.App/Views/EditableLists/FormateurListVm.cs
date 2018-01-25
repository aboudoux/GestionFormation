using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestionFormation.App.Core;
using GestionFormation.Applications.Formateurs;
using GestionFormation.CoreDomain.Formateurs.Queries;

namespace GestionFormation.App.Views.EditableLists
{
    public class FormateurListVm : EditableListVm<EditableFormateur>
    {
        private readonly IFormateurQueries _formateurQueries;

        public FormateurListVm(IApplicationService applicationService, IFormateurQueries formateurQueries) : base(applicationService)
        {
            _formateurQueries = formateurQueries ?? throw new ArgumentNullException(nameof(formateurQueries));
        }

        protected override async Task<IReadOnlyList<EditableFormateur>> LoadAsync()
        {
            return await Task.Run(() => _formateurQueries.GetAll().Select(a=>new EditableFormateur(a)).ToList());
        }

        protected override async Task CreateAsync(EditableFormateur item)
        {            
            await Task.Run(()=> ApplicationService.Command<CreateFormateur>().Execute(item.Nom, item.Prenom, item.Email));
        }

        protected override async Task UpdateAsync(EditableFormateur item)
        {
            await Task.Run(() => ApplicationService.Command<UpdateFormateur>().Execute(item.GetId(), item.Nom, item.Prenom, item.Email) );            
        }

        protected override async Task DeleteAsync(EditableFormateur item)
        {
            await Task.Run(() => ApplicationService.Command<DeleteFormateur>().Execute(item.GetId()));            
        }

        public override string Title => "Liste des formateurs";
    }

    public class EditableFormateur : EditableItem
    {
        public EditableFormateur()
        {
            
        }
        public EditableFormateur(IFormateurResult result) : base(result.Id)
        {
            Nom = result.Nom;
            Prenom = result.Prenom;
            Email = result.Email;
        }

        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
    }
}