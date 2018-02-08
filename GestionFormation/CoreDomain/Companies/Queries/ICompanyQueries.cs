using System;
using System.Collections.Generic;

namespace GestionFormation.CoreDomain.Companies.Queries
{
    public interface ICompanyQueries
    {
        IEnumerable<ICompanyResult> GetAll();
        bool Exists(string companyName);
        bool Exists(Guid companyId);
    }
}
