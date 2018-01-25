using System;
using GestionFormation.CoreDomain.Formations;
using GestionFormation.CoreDomain.Formations.Exceptions;
using GestionFormation.CoreDomain.Formations.Queries;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Formations
{
    public class UpdateFormation : ActionCommand
    {
        private readonly IFormationQueries _queries;

        public UpdateFormation(EventBus eventBus, IFormationQueries queries) : base(eventBus)
        {
            _queries = queries ?? throw new ArgumentNullException(nameof(queries));
        }

        public void Execute(Guid formationId, string newName, int places)
        {
            var foundFormation = _queries.GetFormation(newName);
            if (foundFormation.HasValue && foundFormation.Value != formationId)
                throw new FormationAlreadyExistsException(newName);

            var formation = GetAggregate<Formation>(formationId);
            formation.Update(newName, places);
            PublishUncommitedEvents(formation);
        }
    }
}