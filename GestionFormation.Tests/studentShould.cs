using System;
using System.Collections.Generic;
using FluentAssertions;
using GestionFormation.CoreDomain.Students;
using GestionFormation.CoreDomain.Students.Events;
using GestionFormation.Kernel;
using GestionFormation.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GestionFormation.Tests
{
    [TestClass]
    [TestCategory("UnitTests")]
    public class StudentShould
    {
        [TestMethod]
        public void raise_stagiaireCreated_on_create_new_stagiaire()
        {
            var stagiaire = Student.Create("BOUDOUX", "Aurelien");
            stagiaire.UncommitedEvents.GetStream().Should().Contain(new StudentCreated(Guid.Empty, 0, "BOUDOUX", "Aurelien"));
        }

        [TestMethod]
        public void raise_stagiaireUpdated_on_update_stagiaire()
        {
            var history = new History();
            history.Add(new StudentCreated(Guid.NewGuid(), 1, "BOUDOUX", "Aurélien"));
            
            var stagiaire = new Student(history);
            stagiaire.Update("BOUDOUX", "Aurélien");
            stagiaire.UncommitedEvents.GetStream().Should().Contain(new StudentUpdated(Guid.Empty, 0, "BOUDOUX", "Aurélien"));
        }        

        [TestMethod]
        public void raise_stagiaireDeleted_on_delete_stagiaire()
        {
            var history = new History();
            history.Add(new StudentCreated(Guid.NewGuid(), 1, "BOUDOUX", "Aurélien"));

            var stagiaire = new Student(history);
            stagiaire.Delete();
            stagiaire.UncommitedEvents.GetStream().Should().Contain(new StudentDeleted(Guid.Empty, 0));
        }

        [TestMethod]
        public void dont_raise_stagiaireDeleted_if_stagiaire_already_deleted()
        {
            var history = new History();
            history.Add(new StudentCreated(Guid.NewGuid(), 1, "BOUDOUX", "Aurélien"));
            history.Add(new StudentDeleted(Guid.Empty, 1));

            var stagiaire = new Student(history);
            stagiaire.Delete();
            stagiaire.UncommitedEvents.GetStream().Should().BeEmpty();
        }

        [TestMethod]
        public void dont_raise_multiple_update_if_last_update_is_same_data()
        {
            var history = new History();
            history.Add(new StudentCreated(Guid.NewGuid(), 1, "BOUDOUX", "Aurélien"));
            history.Add(new StudentUpdated(Guid.NewGuid(), 2, "BOUDOUX", "Aurelien"));

            var stagiaire = new Student(history);
            stagiaire.Update("BOUDOUX", "Aurelien");
            stagiaire.UncommitedEvents.GetStream().Should().BeEmpty();
        }

        [TestMethod]
        public void dont_raise_multiple_update_if_multiple_update_call_with_same_data()
        {
            var history = new History();
            history.Add(new StudentCreated(Guid.NewGuid(), 1, "BOUDOUX", "Aurélien"));            

            var stagiaire = new Student(history);
            stagiaire.Update("BOUDOUX", "Aurelien");
            stagiaire.Update("BOUDOUX", "Aurelien");
            stagiaire.UncommitedEvents.GetStream().Should().HaveCount(1);

        }

        [TestMethod]
        public void raise_multiple_update_if_data_change()
        {
            var history = new History();
            history.Add(new StudentCreated(Guid.NewGuid(), 1, "BOUDOUX", "Aurelien"));

            var stagiaire = new Student(history);
            stagiaire.Update("BOUDOUX", "Aurelien1");
            stagiaire.Update("BOUDOUX", "Aurelien1");
            stagiaire.Update("BOUDOUX", "Aurelien2");
            stagiaire.Update("BOUDOUX", "Aurelien3");            
            stagiaire.UncommitedEvents.GetStream().Should().HaveCount(3);
        }

        [TestMethod]
        public void add_stagiaire_in_projection_when_user_created()
        {
            var dispatcher = new EventDispatcher();
            var projection = new FakeStagiaireProjection();
            dispatcher.Register(projection);
            
            var eventBus = new EventBus(dispatcher, new FakeEventStore());

            var stagiaire = Student.Create("BOUDOUX", "Aurelien");
             eventBus.Publish(stagiaire.UncommitedEvents);

            projection.Tables.Should().HaveCount(1);
        }

        private class FakeStagiaireProjection : IEventHandler<StudentCreated>, IEventHandler<StudentUpdated>, IEventHandler<StudentDeleted>
        {
            public List<StagiaireTable> Tables = new List<StagiaireTable>();

            public void Handle(StudentCreated @event)
            {
                Tables.Add(new StagiaireTable(){ Nom = @event.Lastname, Prenom = @event.Firstname});
            }

            public void Handle(StudentUpdated @event)
            {
                
            }

            public void Handle(StudentDeleted @event)
            {
                throw new System.NotImplementedException();
            }
        }

       /* public class FakeEventStore : IEventStore
        {
            public void Save(IDomainEvent @event)
            {
                
            }

            public int GetLastSequence(Guid aggregateId)
            {
                return 0;
            }

            public IReadOnlyList<IDomainEvent> GetEvents(Guid aggregateId)
            {
                throw new NotImplementedException();
            }
        }*/

        public class StagiaireTable
        {
            public string Nom { get; set; }
            public string Prenom { get; set; }
            public string Societe { get; set; }
        }
    }
}
