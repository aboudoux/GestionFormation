using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestionFormation.App.Core;
using GestionFormation.Applications.Contacts;
using GestionFormation.CoreDomain.Contacts.Queries;

namespace GestionFormation.App.Views.EditableLists
{
    public class ContactListVm : EditableListVm<EditableContact>
    {
        private readonly IContactQueries _contactQueries;

        public ContactListVm(IApplicationService applicationService, IContactQueries contactQueries) : base(applicationService)
        {
            _contactQueries = contactQueries ?? throw new ArgumentNullException(nameof(contactQueries));
        }

        protected override async Task<IReadOnlyList<EditableContact>> LoadAsync()
        {
            return await Task.Run(() => _contactQueries.GetAll().Select(a => new EditableContact(a)).ToList());
        }

        protected override async Task CreateAsync(EditableContact item)
        {
            await Task.Run(() => ApplicationService.Command<CreateContact>().Execute(item.Nom, item.Prenom, item.Email, item.Telephone) );
        }

        protected override async Task UpdateAsync(EditableContact item)
        {
            await Task.Run(() => ApplicationService.Command<UpdateContact>().Execute(item.GetId(), item.Nom, item.Prenom, item.Email, item.Telephone));
        }

        protected override async Task DeleteAsync(EditableContact item)
        {
            await Task.Run(() => ApplicationService.Command<DeleteContact>().Execute(item.GetId()));
        }

        public override string Title => "Liste des contacts";
    }

    public class EditableContact : EditableItem
    {
        public EditableContact()
        {
            
        }
        public EditableContact(IContactResult result) : base(result.Id)
        {
            Nom = result.Nom;
            Prenom = result.Prenom;
            Email = result.Email;
            Telephone = result.Telephone;
        }

        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
    }
}