using System;
using System.Collections.Generic;
using System.Linq;
using GestionFormation.EventStore;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Companies.Queries
{
    public class CompanySqlQueries : ICompanyQueries, IRuntimeDependency
    {
        public IEnumerable<ICompanyResult> GetAll()
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                return context.Companies.ToList().Select(a => new CompanyResult(a));
            }
        }

        public bool Exists(Guid companyId)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                return context.Companies.Any(a => a.CompanyId == companyId);
            }
        }
    }
}