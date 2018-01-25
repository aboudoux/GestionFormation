using System;
using GestionFormation.Applications.Lieux.Exceptions;
using GestionFormation.CoreDomain.Lieux;
using GestionFormation.CoreDomain.Lieux.Queries;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Lieux
{
    public class CreateLieu : ActionCommand
    {
        private readonly ILieuQueries _lieuQueries;

        public CreateLieu(EventBus eventBus, ILieuQueries lieuQueries) : base(eventBus)
        {
            _lieuQueries = lieuQueries ?? throw new ArgumentNullException(nameof(lieuQueries));
        }

        public Lieu Execute(string nom, string addresse, int places)
        {
            if(_lieuQueries.GetLieu(nom).HasValue)
                throw new LieuAlreadyExistsException(nom);

            var lieu = Lieu.Create(nom, addresse, places);
            PublishUncommitedEvents(lieu);
            return lieu;
        }
    }
}
