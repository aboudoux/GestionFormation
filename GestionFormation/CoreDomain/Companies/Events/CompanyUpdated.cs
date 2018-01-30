﻿using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Companies.Events
{
    public class CompanyUpdated : DomainEvent
    {
        public string Name { get; }
        public string Address { get; }
        public string ZipCode { get; }
        public string City { get; }

        public CompanyUpdated(Guid aggregateId, int sequence, string name, string address, string zipCode, string city) : base(aggregateId, sequence)
        {
            Name = name;
            Address = address;
            ZipCode = zipCode;
            City = city;
        }

        protected override string Description => "Société modifiée";
    }
}