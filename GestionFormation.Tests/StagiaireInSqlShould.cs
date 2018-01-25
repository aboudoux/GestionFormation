using FluentAssertions;
using GestionFormation.CoreDomain.Stagiaires;
using GestionFormation.CoreDomain.Stagiaires.Projections;
using GestionFormation.EventStore;
using GestionFormation.Kernel;
using GestionFormation.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GestionFormation.Tests
{
    [TestClass]
    [TestCategory("IntegrationTests")]
    public class StagiaireInSqlShould
    {
        [TestMethod]
        public void be_reidrated_from_database()
        {
            var eventStore = new SqlEventStore(new DomainEventJsonEventSerializer(), new FakeEventStamping());
            var eventBus = new EventBus(new EventDispatcher(), eventStore);
            var createdStagiaire = Stagiaire.Create("BOUDOUX", "Aurelien");
            eventBus.Publish(createdStagiaire.UncommitedEvents);

            var stagiaireId = createdStagiaire.AggregateId;
            var events = eventStore.GetEvents(stagiaireId);
            var stagiaire = new Stagiaire(new History(events));
            stagiaire.Update("BOUDOUX", "Aurélien");
            eventBus.Publish(stagiaire.UncommitedEvents);

            var stagiaire2 = new Stagiaire(new History(eventStore.GetEvents(stagiaireId)));
            stagiaire2.Update("BOUDOUX", "Aurélien");
            stagiaire2.UncommitedEvents.GetStream().Should().BeEmpty();
        }

        [TestMethod]
        public void write_result_in_projection()
        {
            var eventStore = new SqlEventStore(new DomainEventJsonEventSerializer(), new FakeEventStamping());
            var dispatcher = new EventDispatcher();
            dispatcher.Register(new StagiaireSqlProjection());

            var eventBus = new EventBus(dispatcher, eventStore);

            var stagiaire = Stagiaire.Create("BOUDOUX", "Aurelien");
            eventBus.Publish(stagiaire.UncommitedEvents);
        }
    }
}