using System;
using GestionFormation.CoreDomain.Companies.Events;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Companies
{
    public class Company : AggregateRootUpdatableAndDeletable<CompanyUpdated, CompanyDeleted>
    {
        public Company(History history) : base(history)
        {
        }

        public static Company Create(string name, string address, string zipcode, string city)
        {
            var societe = new Company(History.Empty);
            societe.AggregateId = Guid.NewGuid();
            societe.UncommitedEvents.Add(new CompanyCreated(societe.AggregateId, 1, name, address, zipcode, city ));
            return societe;
        }

        public void Update(string name, string address, string zipcode, string city)
        {
            Update(new CompanyUpdated(AggregateId, GetNextSequence(), name, address, zipcode, city));
        }

        public void Delete()
        {
            Delete(new CompanyDeleted(AggregateId, GetNextSequence()));
        }
    }
}
