using System;
using System.Collections.Generic;

namespace GestionFormation.CoreDomain.Companies.Queries
{
    public interface ICompanyQueries
    {
        IEnumerable<ICompanyResult> GetAll();
        Guid? GetIdIfExists(string companyName);
        bool Exists(Guid companyId);
    }
}
