using System;
using GestionFormation.CoreDomain.Contacts.Queries;

namespace GestionFormation.App.Views.Seats
{
    public class ContactItem
    {
        public ContactItem(IContactResult contactResult)
        {
            Id = contactResult.Id;
            Nom = contactResult.Lastname;
            Prenom = contactResult.Firstname;
            Telephone = contactResult.Telephone;
            Email = contactResult.Email;
        }
        public Guid Id { get; }
        public string Nom { get; }
        public string Prenom { get; }
        public string Telephone { get; }
        public string Email { get; }

        public override string ToString()
        {
            return Nom + " " + Prenom;
        }
    }
}