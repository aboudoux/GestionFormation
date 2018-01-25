using GestionFormation.CoreDomain.Contacts;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Contacts
{
    public class CreateContact : ActionCommand
    {
        public CreateContact(EventBus eventBus) : base(eventBus)
        {
        }

        public Contact Execute(string nom, string prenom, string email, string telephone)
        {
            var contact = Contact.Create(nom, prenom, email, telephone);
            PublishUncommitedEvents(contact);
            return contact;
        }
    }
}
