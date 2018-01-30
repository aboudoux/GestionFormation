using System;
using GestionFormation.CoreDomain.Contacts;
using GestionFormation.CoreDomain.Societes.Queries;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Contacts
{
    public class CreateContact : ActionCommand
    {
        private readonly ISocieteQueries _societeQueries;

        public CreateContact(EventBus eventBus, ISocieteQueries societeQueries) : base(eventBus)
        {
            _societeQueries = societeQueries ?? throw new ArgumentNullException(nameof(societeQueries));
        }

        public Contact Execute(Guid societeId, string nom, string prenom, string email, string telephone)
        {
            if(!_societeQueries.Exists(societeId))
                throw new Exception("Impossible de créer le contact car la société à laquele vous voulez le rattacher n'existe pas");

            var contact = Contact.Create(societeId, nom, prenom, email, telephone);
            PublishUncommitedEvents(contact);
            return contact;
        }
    }
}
