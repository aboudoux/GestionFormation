using System;
using System.Collections.Generic;
using System.Text;

namespace GestionFormation.CoreDomain.Contacts.Queries
{
    public interface IContactQueries
    {
        IEnumerable<IContactResult> GetAll(Guid companyId);
        IEnumerable<IContactResult> GetAll();
        IContactResult GetAgreementContact(Guid agreementId);
    }
}
