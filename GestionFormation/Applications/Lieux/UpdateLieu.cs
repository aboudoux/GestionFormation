using System;
using GestionFormation.Applications.Lieux.Exceptions;
using GestionFormation.CoreDomain.Lieux;
using GestionFormation.CoreDomain.Lieux.Queries;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Lieux
{
    public class UpdateLieu : ActionCommand
    {
        private readonly ILieuQueries _lieuQueries;

        public UpdateLieu(EventBus eventBus, ILieuQueries lieuQueries) : base(eventBus)
        {
            _lieuQueries = lieuQueries ?? throw new ArgumentNullException(nameof(lieuQueries));
        }

        public void Execute(Guid lieuId, string newNom, string addresse, int places)
        {
            var foundLieuId = _lieuQueries.GetLieu(newNom);

            if (foundLieuId.HasValue && foundLieuId.Value != lieuId)
                throw new LieuAlreadyExistsException(newNom);

            var lieu = GetAggregate<Lieu>(lieuId);
            lieu.Update(newNom, addresse, places);
            PublishUncommitedEvents(lieu);
        }
    }
}