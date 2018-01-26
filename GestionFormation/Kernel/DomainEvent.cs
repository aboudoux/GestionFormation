using System;
using System.Linq;

namespace GestionFormation.Kernel
{
    public abstract class DomainEvent : IDomainEvent
    {
        protected DomainEvent(Guid aggregateId, int sequence)
        {
            AggregateId = aggregateId;
            Sequence = sequence;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != GetType())
                return false;

            return GetType().GetProperties()
                .Where(a=>a.Name != nameof(AggregateId) && a.Name != nameof(Sequence))
                .All(a =>
                {
                    var left = a.GetValue(this);
                    var right = a.GetValue(obj);
                    if(left != null && right != null)
                        return a.GetValue(this).Equals(a.GetValue(obj));
                    return left == null && right == null;
                });
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public Guid AggregateId { get; }
        public int Sequence { get; }

        protected abstract string Description { get; }

        public override string ToString()
        {
            return Description;
        }
    }
}