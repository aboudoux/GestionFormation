using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestionFormation.App.Core;
using GestionFormation.Applications.Trainers;
using GestionFormation.CoreDomain.Trainers.Queries;

namespace GestionFormation.App.Views.EditableLists
{
    public class FormateurListVm : EditableListVm<EditableFormateur>
    {
        private readonly ITrainerQueries _trainerQueries;

        public FormateurListVm(IApplicationService applicationService, ITrainerQueries trainerQueries) : base(applicationService)
        {
            _trainerQueries = trainerQueries ?? throw new ArgumentNullException(nameof(trainerQueries));
        }

        protected override async Task<IReadOnlyList<EditableFormateur>> LoadAsync()
        {
            return await Task.Run(() => _trainerQueries.GetAll().Select(a=>new EditableFormateur(a)).ToList());
        }

        protected override async Task CreateAsync(EditableFormateur item)
        {            
            await Task.Run(()=> ApplicationService.Command<CreateTrainer>().Execute(item.Nom, item.Prenom, item.Email));
        }

        protected override async Task UpdateAsync(EditableFormateur item)
        {
            await Task.Run(() => ApplicationService.Command<UpdateTrainer>().Execute(item.GetId(), item.Nom, item.Prenom, item.Email) );            
        }

        protected override async Task DeleteAsync(EditableFormateur item)
        {
            await Task.Run(() => ApplicationService.Command<DeleteTrainer>().Execute(item.GetId()));            
        }

        public override string Title => "Liste des formateurs";
    }

    public class EditableFormateur : EditableItem
    {
        public EditableFormateur()
        {
            
        }
        public EditableFormateur(ITrainerResult result) : base(result.Id)
        {
            Nom = result.Lastname;
            Prenom = result.Firstname;
            Email = result.Email;
        }

        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
    }
}