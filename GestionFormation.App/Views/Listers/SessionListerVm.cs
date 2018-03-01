using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GestionFormation.App.Core;
using GestionFormation.App.Views.Listers.Bases;
using GestionFormation.CoreDomain.Sessions.Queries;

namespace GestionFormation.App.Views.Listers
{
    public class SessionListerVm : ListerWindowVm<SessionListerItem>
    {
        private readonly ISessionQueries _sessionQueries;
        public override string Title => "Liste des sessions";
        public SessionListerVm(IApplicationService applicationService, ISessionQueries sessionQueries) : base(applicationService)
        {
            _sessionQueries = sessionQueries;
        }

        protected override async Task<IEnumerable<SessionListerItem>> LoadAsync()
        {
            return await Task.Run(() => _sessionQueries.GetAllCompleteSession().Select(a => new SessionListerItem(a)));
        }
    }

    public class SessionListerItem
    {
        public SessionListerItem(ICompleteSessionResult result)
        {
            TrainingName = result.Training;
            Start = result.SessionStart;
            Duration = result.Duration;
            TrainerName = result.Trainer.ToString();
            Location = result.Location;            
        }

        [DisplayName("Formation")]
        public string TrainingName { get; }
        [DisplayName("Debut")]
        public DateTime Start { get; }
        [DisplayName("Durée")]
        public int Duration { get; }
        [DisplayName("Formateur")]
        public string TrainerName { get; }
        [DisplayName("Lieu")]
        public string Location { get; }
    }
}