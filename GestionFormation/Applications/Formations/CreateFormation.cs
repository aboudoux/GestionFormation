using System;
using GestionFormation.CoreDomain.Formations;
using GestionFormation.CoreDomain.Formations.Exceptions;
using GestionFormation.CoreDomain.Formations.Queries;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Formations
{
    public class CreateFormation : ActionCommand
    {
        private readonly IFormationQueries _queries;

        public CreateFormation(EventBus eventBus, IFormationQueries queries) : base(eventBus)
        {
            _queries = queries ?? throw new ArgumentNullException(nameof(queries));
        }
        public Formation Execute(string nom, int places)
        {            
            if (_queries.GetFormation(nom).HasValue)
                throw new FormationAlreadyExistsException(nom);
            
            var formation = Formation.Create(nom, places);
            PublishUncommitedEvents(formation);
            return formation;
        }
    }
}
