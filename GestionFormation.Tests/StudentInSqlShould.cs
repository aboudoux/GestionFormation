using System.Drawing;
using FluentAssertions;
using GestionFormation.CoreDomain.Students;
using GestionFormation.EventStore;
using GestionFormation.Infrastructure.Students.Projections;
using GestionFormation.Kernel;
using GestionFormation.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GestionFormation.Tests
{
    [TestClass]
    [TestCategory("IntegrationTests")]
    public class StudentInSqlShould
    {
        [TestMethod]
        public void be_reidrated_from_database()
        {
            var eventStore = new SqlEventStore(new DomainEventJsonEventSerializer(), new FakeEventStamping());
            var eventBus = new EventBus(new EventDispatcher(), eventStore);
            var createdStagiaire = Student.Create("BOUDOUX", "Aurelien");
            eventBus.Publish(createdStagiaire.UncommitedEvents);

            var studentId = createdStagiaire.AggregateId;
            var events = eventStore.GetEvents(studentId);
            var student = new Student(new History(events));
            student.Update("BOUDOUX", "Aurélien");
            eventBus.Publish(student.UncommitedEvents);

            var student1 = new Student(new History(eventStore.GetEvents(studentId)));
            student1.Update("BOUDOUX", "Aurélien");
            student1.UncommitedEvents.GetStream().Should().BeEmpty();            
        }

        [TestMethod]
        public void write_result_in_projection()
        {
            var eventStore = new SqlEventStore(new DomainEventJsonEventSerializer(), new FakeEventStamping());
            var dispatcher = new EventDispatcher();
            dispatcher.Register(new StudentSqlProjection());

            var eventBus = new EventBus(dispatcher, eventStore);

            var student = Student.Create("BOUDOUX", "Aurelien");
            eventBus.Publish(student.UncommitedEvents);
        }
    }
}