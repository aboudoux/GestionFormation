using System;
using System.Collections.Generic;
using System.Text;

namespace GestionFormation.CoreDomain.Contacts.Queries
{
    public interface IContactQueries
    {
        IEnumerable<IContactResult> GetAll();
        IContactResult GetConventionContact(Guid conventionId);
    }
}
