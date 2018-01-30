using System;

namespace GestionFormation.CoreDomain.Companies.Queries
{
    public interface ICompanyResult
    {
        Guid CompanyId { get; }
        string Name { get; }
        string Address { get; }
        string ZipCode { get; }
        string City { get; }
    }
}