using System;
using GestionFormation.Kernel;

namespace GestionFormation.Applications
{
    public class AggregateNotFoundException : DomainException
    {
        public AggregateNotFoundException(Type aggregatType, Guid aggregatId) : base($"Impossible de charger l'agrégat de type {aggregatType.Name} ayant l'id {aggregatId}")
        {
            
        }
    }
}