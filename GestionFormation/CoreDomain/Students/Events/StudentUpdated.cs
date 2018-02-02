using System;

namespace GestionFormation.CoreDomain.Students.Events
{
    public class StudentUpdated : StudentCreated
    {
        public StudentUpdated(Guid aggregateId, int sequence, string lastname, string firstname) : base(aggregateId, sequence, lastname, firstname)
        {          
        }
    }
}