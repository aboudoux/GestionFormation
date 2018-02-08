using System;
using System.Collections.Generic;
using System.Linq;
using GestionFormation.EventStore;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Companies.Queries
{
    public class CompanyQueries : ICompanyQueries, IRuntimeDependency
    {
        public IEnumerable<ICompanyResult> GetAll()
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                return context.Companies.Where(a=>a.Removed == false).ToList().Select(a => new CompanyResult(a));
            }
        }

        public bool Exists(string companyName)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var lowerCompanyName = companyName.ToLower();
                return context.Companies.Any(a => a.Name.ToLower() == lowerCompanyName && a.Removed == false);
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