using System;
using System.Collections.Generic;
using FluentAssertions;
using GestionFormation.CoreDomain.Stagiaires;
using GestionFormation.CoreDomain.Stagiaires.Events;
using GestionFormation.Kernel;
using GestionFormation.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GestionFormation.Tests
{
    [TestClass]
    [TestCategory("UnitTests")]
    public class StagiaireShould
    {
        [TestMethod]
        public void raise_stagiaireCreated_on_create_new_stagiaire()
        {
            var stagiaire = Stagiaire.Create("BOUDOUX", "Aurelien");
            stagiaire.UncommitedEvents.GetStream().Should().Contain(new StagiaireCreated(Guid.Empty, 0, "BOUDOUX", "Aurelien"));
        }

        [TestMethod]
        public void raise_stagiaireUpdated_on_update_stagiaire()
        {
            var history = new History();
            history.Add(new StagiaireCreated(Guid.NewGuid(), 1, "BOUDOUX", "Aurélien"));
            
            var stagiaire = new Stagiaire(history);
            stagiaire.Update("BOUDOUX", "Aurélien");
            stagiaire.UncommitedEvents.GetStream().Should().Contain(new StagiaireUpdated(Guid.Empty, 0, "BOUDOUX", "Aurélien"));
        }        

        [TestMethod]
        public void raise_stagiaireDeleted_on_delete_stagiaire()
        {
            var history = new History();
            history.Add(new StagiaireCreated(Guid.NewGuid(), 1, "BOUDOUX", "Aurélien"));

            var stagiaire = new Stagiaire(history);
            stagiaire.Delete();
            stagiaire.UncommitedEvents.GetStream().Should().Contain(new StagiaireDeleted(Guid.Empty, 0));
        }

        [TestMethod]
        public void dont_raise_stagiaireDeleted_if_stagiaire_already_deleted()
        {
            var history = new History();
            history.Add(new StagiaireCreated(Guid.NewGuid(), 1, "BOUDOUX", "Aurélien"));
            history.Add(new StagiaireDeleted(Guid.Empty, 1));

            var stagiaire = new Stagiaire(history);
            stagiaire.Delete();
            stagiaire.UncommitedEvents.GetStream().Should().BeEmpty();
        }

        [TestMethod]
        public void dont_raise_multiple_update_if_last_update_is_same_data()
        {
            var history = new History();
            history.Add(new StagiaireCreated(Guid.NewGuid(), 1, "BOUDOUX", "Aurélien"));
            history.Add(new StagiaireUpdated(Guid.NewGuid(), 2, "BOUDOUX", "Aurelien"));

            var stagiaire = new Stagiaire(history);
            stagiaire.Update("BOUDOUX", "Aurelien");
            stagiaire.UncommitedEvents.GetStream().Should().BeEmpty();
        }

        [TestMethod]
        public void dont_raise_multiple_update_if_multiple_update_call_with_same_data()
        {
            var history = new History();
            history.Add(new StagiaireCreated(Guid.NewGuid(), 1, "BOUDOUX", "Aurélien"));            

            var stagiaire = new Stagiaire(history);
            stagiaire.Update("BOUDOUX", "Aurelien");
            stagiaire.Update("BOUDOUX", "Aurelien");
            stagiaire.UncommitedEvents.GetStream().Should().HaveCount(1);

        }

        [TestMethod]
        public void raise_multiple_update_if_data_change()
        {
            var history = new History();
            history.Add(new StagiaireCreated(Guid.NewGuid(), 1, "BOUDOUX", "Aurelien"));

            var stagiaire = new Stagiaire(history);
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

            var stagiaire = Stagiaire.Create("BOUDOUX", "Aurelien");
             eventBus.Publish(stagiaire.UncommitedEvents);

            projection.Tables.Should().HaveCount(1);
        }

        private class FakeStagiaireProjection : IEventHandler<StagiaireCreated>, IEventHandler<StagiaireUpdated>, IEventHandler<StagiaireDeleted>
        {
            public List<StagiaireTable> Tables = new List<StagiaireTable>();

            public void Handle(StagiaireCreated @event)
            {
                Tables.Add(new StagiaireTable(){ Nom = @event.Nom, Prenom = @event.Prenom});
            }

            public void Handle(StagiaireUpdated @event)
            {
                
            }

            public void Handle(StagiaireDeleted @event)
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
