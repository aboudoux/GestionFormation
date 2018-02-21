using System;
using System.Collections.Generic;
using System.Linq;
using GestionFormation.CoreDomain.Companies.Queries;

namespace GestionFormation.Tests.Fakes
{
    public class FakeCompanyQueries : ICompanyQueries
    {
        private readonly List<ICompanyResult> _companies = new List<ICompanyResult>();

        public void Add(string name, string address = null, string zipCode = null, string city = null, Guid? companyId = null)
        {
            _companies.Add(new Result(companyId ?? Guid.NewGuid(), name, address, zipCode, city));
        }

        public IEnumerable<ICompanyResult> GetAll()
        {
            return _companies;
        }

        public Guid? GetIdIfExists(string companyName)
        {
            return _companies.FirstOrDefault(a => string.Equals(a.Name, companyName, StringComparison.CurrentCultureIgnoreCase))?.CompanyId;
        }

        public bool Exists(Guid companyId)
        {
            return _companies.Any(a => a.CompanyId == companyId);
        }

        private class Result : ICompanyResult
        {
            public Result(Guid companyId, string name, string address, string zipCode, string city)
            {
                CompanyId = companyId;
                Name = name;
                Address = address;
                ZipCode = zipCode;
                City = city;
            }

            public Guid CompanyId { get; }
            public string Name { get; }
            public string Address { get; }
            public string ZipCode { get; }
            public string City { get; }
        }
    }
}