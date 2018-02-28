using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GestionFormation.App.Core;
using GestionFormation.Applications.Contacts;
using GestionFormation.CoreDomain.Contacts.Queries;

namespace GestionFormation.App.Views.EditableLists
{
    public class ContactListVm : EditableListVm<EditableContact, EditableContact, CreateContactWindowVm>
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
            await Task.Run(() => ApplicationService.Command<CreateContact>().Execute(item.GetSocieteId(), item.Lastname, item.Firstname, item.Email, item.Telephone) );
        }

        protected override async Task UpdateAsync(EditableContact item)
        {
            await Task.Run(() => ApplicationService.Command<UpdateContact>().Execute(item.GetId(), item.GetSocieteId(), item.Lastname, item.Firstname, item.Email, item.Telephone));
        }

        protected override async Task DeleteAsync(EditableContact item)
        {
            await Task.Run(() => ApplicationService.Command<DeleteContact>().Execute(item.GetId()));
        }

        public override string Title => "Liste des contacts";     
    }

    public class EditableContact : EditableItem
    {
        private readonly Guid _societeId;

        public EditableContact()
        {            
        }

        public EditableContact(Guid societeId)
        {
            _societeId = societeId;
        }

        public EditableContact(Guid societeId, Guid objectId) : base(objectId)
        {
            _societeId = societeId;
        }

        public EditableContact(IContactResult result) : base(result.Id)
        {
            Lastname = result.Lastname;
            Firstname = result.Firstname;
            Email = result.Email;
            Telephone = result.Telephone;
            Company = result.CompanyName;
            _societeId = result.CompanyId;            
        }

        [DisplayName("Nom")]
        public string Lastname { get; set; }
        [DisplayName("Prenom")]
        public string Firstname { get; set; }
        [DisplayName("Email")]
        public string Email { get; set; }
        [DisplayName("Téléphone")]
        public string Telephone { get; set; }
        [DisplayName("Société")]
        public string Company { get; set; }
        public Guid GetSocieteId() => _societeId;      
    }
}