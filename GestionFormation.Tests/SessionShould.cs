using System;
using System.Globalization;
using FluentAssertions;
using GestionFormation.Applications.Sessions;
using GestionFormation.CoreDomain.Seats.Events;
using GestionFormation.CoreDomain.Sessions;
using GestionFormation.CoreDomain.Sessions.Events;
using GestionFormation.CoreDomain.Sessions.Exceptions;
using GestionFormation.Kernel;
using GestionFormation.Tests.Fakes;
using GestionFormation.Tests.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GestionFormation.Tests
{
    [TestClass]
    [TestCategory("UnitTests")]
    public class SessionShould
    {
        [TestMethod]
        public void raise_sessionPlanned_when_add_session_to_formation()
        {
            var formationId = Guid.NewGuid();
            var formateurId = Guid.NewGuid();
            var lieuId = Guid.NewGuid();
            var session = Session.Plan(formationId, new DateTime(2018, 1, 1), 2, 5, lieuId, formateurId);
            session.UncommitedEvents.GetStream().Should().Contain(new SessionPlanned(Guid.Empty, 0,formationId, new DateTime(2018, 1, 1), 2, 5, lieuId, formateurId));
        }

        [TestMethod]
        public void raise_session_updated_on_update_session()
        {
            var formationId = Guid.NewGuid();
            var formateurId = Guid.NewGuid();
            var lieuId = Guid.NewGuid();

            var session = Session.Plan(formationId, new DateTime(2018, 1, 1), 2, 5, Guid.NewGuid(), formateurId);
            session.Update(formationId,new DateTime(2018, 1, 2), 4, 3, lieuId, formateurId);
            session.UncommitedEvents.GetStream().Should().Contain(new SessionUpdated(Guid.Empty, 0, new DateTime(2018, 1, 2), 4, 3, lieuId, formateurId, formationId));
        }

        [TestMethod]
        public void not_raise_sessionUpdated_if_session_alrady_updated_with_same_data()
        {
            var context = TestSession.Create();
            context.Builder.AddEvent(new SessionUpdated(Guid.Empty, 0, new DateTime(2018, 1, 1), 4, 3, context.LieuId, context.FormateurId, context.FormationId));
            var session = context.Builder.Create();

            session.Update(context.FormationId, new DateTime(2018, 1, 1), 4, 3, context.LieuId, context.FormateurId);
            session.UncommitedEvents.GetStream().Should().BeEmpty();
        }

        [TestMethod]
        public void raise_sessionDeleted_when_delete_session()
        {
            var context = TestSession.Create();
            var session = context.Builder.Create();
            session.Delete();

            session.UncommitedEvents.GetStream().Should().Contain(new SessionDeleted(Guid.NewGuid(), 1));
        }

        [TestMethod]
        public void not_raise_sessionDeleted_when_session_already_deleted()
        {
            var context = TestSession.Create();
            context.Builder.AddEvent(new SessionDeleted(Guid.NewGuid(), 1));

            var session = context.Builder.Create();
            session.Delete();

            session.UncommitedEvents.GetStream().Should().BeEmpty();
        }

        [TestMethod]
        public void raise_sessionCanceled_when_cancel()
        {
            var context = TestSession.Create();
            var session = context.Builder.Create();            

            session.Cancel("because !");
            session.UncommitedEvents.GetStream().Should().Contain(new SessionCanceled(Guid.Empty, 0, "because !"));
        }

        [TestMethod]
        public void not_raise_sessionCanceled_if_session_deleted()
        {
            var context = TestSession.Create();
            context.Builder.AddEvent(new SessionDeleted(Guid.Empty, 2));
            var session = context.Builder.Create();
            session.Cancel("OK");
            session.UncommitedEvents.GetStream().Should().BeEmpty();
        }

        
        [TestMethod]
        public void raise_placeReserved_if_place_available()
        {
            var context = TestSession.Create();
            var session = context.Builder.Create();

            var stagiaireId = Guid.NewGuid();
            var societeId = Guid.NewGuid();
            var place = session.BookSeat(stagiaireId, societeId);

            session.UncommitedEvents.GetStream().Should().Contain(new SessionSeatBooked(Guid.Empty, 0));
            place.UncommitedEvents.GetStream().Should()
                .Contain(new SeatCreated(Guid.Empty, 1, session.AggregateId, stagiaireId, societeId));
        }

        
        [TestMethod]
        public void throw_exception_if_trying_to_reserve_a_place_when_no_places_are_available()
        {
            var context = TestSession.Create();
            var session = context.Builder.Create();
            
            session.BookSeat(Guid.NewGuid(), Guid.NewGuid());
            session.BookSeat(Guid.NewGuid(), Guid.NewGuid());
            session.BookSeat(Guid.NewGuid(), Guid.NewGuid());
            session.BookSeat(Guid.NewGuid(), Guid.NewGuid());
            session.BookSeat(Guid.NewGuid(), Guid.NewGuid());

            Action action = () => session.BookSeat(Guid.NewGuid(), Guid.NewGuid());
            action.ShouldThrow<NoMoreSeatAvailableException>();
        }

        [TestMethod]
        public void throw_error_if_update_nbrplaces_lower_than_places_already_reserved()
        {
            var context = TestSession.Create();
            var session = context.Builder.Create();
            session.BookSeat(Guid.NewGuid(), Guid.NewGuid());
            session.BookSeat(Guid.NewGuid(), Guid.NewGuid());
            session.BookSeat(Guid.NewGuid(), Guid.NewGuid());
            session.BookSeat(Guid.NewGuid(), Guid.NewGuid());

            Action action = () => session.Update(context.FormationId, DateTime.Now, 0, 3, context.LieuId, context.FormateurId);
            action.ShouldThrow<TooManySeatsAlreadyReservedException>();
        }

        [DataTestMethod]
        [DataRow("05/01/2018", 2)]
        [DataRow("06/01/2018", 1)]
        public void not_be_planned_on_weekend_day(string strDate, int durée)
        {
            var date = DateTime.ParseExact(strDate, "dd/MM/yyyy", CultureInfo.CurrentCulture);
            Action action = ()=> Session.Plan(Guid.NewGuid(), date, durée, 5, Guid.NewGuid(), Guid.NewGuid());
            action.ShouldThrow<SessionWeekEndException>();
        }

        [TestMethod]
        public void not_be_updated_on_a_weekend_day()
        {
            var context = TestSession.Create();
            var session = context.Builder.Create();
            Action action = () => session.Update(context.FormationId, new DateTime(2018,1,4), 5, 5, context.LieuId, context.FormateurId);
            action.ShouldThrow<SessionWeekEndException>();
        }

        [TestMethod]
        public void release_session_place_on_releasePlace_command()
        {
            var sessionId = Guid.NewGuid();
            var placeId = Guid.NewGuid();

            var fakeStorage = new FakeEventStore();            
            fakeStorage.Save(new SessionPlanned(sessionId, 1, Guid.NewGuid(), new DateTime(2018,1,9), 1, 5, null, null ));
            fakeStorage.Save(new SessionSeatBooked(sessionId, 2));
            fakeStorage.Save(new SeatCreated(placeId, 1, sessionId, Guid.NewGuid(), Guid.NewGuid()));

            var bus = new EventBus(new EventDispatcher(), fakeStorage );
            new ReleaseSeat(bus).Execute(sessionId, placeId, "essai");

            fakeStorage.GetEvents(sessionId).Should().Contain(new SessionSeatReleased(sessionId, 1));
            fakeStorage.GetEvents(placeId).Should().Contain(new SeatCanceled(placeId, 1, "essai"));
        }

        private class TestSession
        {
            private TestSession()
            {                
                FormationId = Guid.NewGuid();
                FormateurId = Guid.NewGuid();
                LieuId = Guid.NewGuid();
                Builder = Aggregate.Make<Session>().AddEvent(new SessionPlanned(Guid.NewGuid(), 1, FormationId, new DateTime(2018, 1, 1), 0, 5, LieuId, FormateurId));
            }

            public Aggregate.AggregateBuilder<Session> Builder { get; }
            public Guid FormationId { get; }
            public Guid FormateurId { get; }
            public Guid LieuId { get; }

            public static TestSession Create()
            {
                return new TestSession();
            }
        }
    }
}