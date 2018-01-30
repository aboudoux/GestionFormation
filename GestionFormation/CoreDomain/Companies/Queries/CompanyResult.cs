using System;
using GestionFormation.CoreDomain.Companies.Projections;

namespace GestionFormation.CoreDomain.Companies.Queries
{
    public class CompanyResult : ICompanyResult
    {
        public CompanyResult(CompanySqlEntity entity)
        {
            CompanyId = entity.CompanyId;
            Name = entity.Name;
            Address = entity.Address;
            ZipCode = entity.ZipCode;
            City = entity.City;
        }
        public Guid CompanyId { get; }
        public string Name { get; }
        public string Address { get; }
        public string ZipCode { get; }
        public string City { get; }
    }
}