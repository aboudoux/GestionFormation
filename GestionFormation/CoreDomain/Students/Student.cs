using System;
using GestionFormation.CoreDomain.Students.Events;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Students
{
    public class Student : AggregateRootUpdatableAndDeletable<StudentUpdated, StudentDeleted>
    {
        
        public Student(History history) : base(history)
        {
        }        

        public static Student Create(string lastname, string firstname)
        {   
            var stagiaire = new Student(History.Empty);
            stagiaire.AggregateId = Guid.NewGuid();
            stagiaire.UncommitedEvents.Add(new StudentCreated(stagiaire.AggregateId, 1, lastname, firstname));
            return stagiaire;

        }

        public void Update(string lastname, string firstname)
        {
            Update(new StudentUpdated(AggregateId, GetNextSequence(), lastname, firstname));
        }

        public void Delete()
        {
            Delete(new StudentDeleted(AggregateId, GetNextSequence()));          
        }
    }
}