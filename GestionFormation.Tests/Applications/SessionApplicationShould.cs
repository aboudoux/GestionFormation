using System;
using System.Linq;
using FluentAssertions;
using GestionFormation.Applications.Formateurs;
using GestionFormation.Applications.Formations;
using GestionFormation.Applications.Lieux;
using GestionFormation.Applications.Sessions;
using GestionFormation.CoreDomain.Formateurs.Events;
using GestionFormation.CoreDomain.Formateurs.Exceptions;
using GestionFormation.CoreDomain.Formations.Events;
using GestionFormation.CoreDomain.Lieux.Events;
using GestionFormation.CoreDomain.Lieux.Exceptions;
using GestionFormation.CoreDomain.Sessions.Events;
using GestionFormation.Kernel;
using GestionFormation.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GestionFormation.Tests.Applications
{
    [TestClass]
    [TestCategory("UnitTests")]
    public class SessionApplicationShould
    {
        [TestMethod]
        public void be_planned_if_formateur_available_for_session()
        {
            // given
            var formationId = Guid.NewGuid();
            var formateurId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();
            var mockHandler = new MockHandler<FormateurAssigned, SessionPlanned>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();
            eventStore.Save(new FormateurCreated(formateurId, 1, "BOUDOUX", "Aurelien", "test@test.com"));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            new PlanSession(eventBus).Execute(formationId, new DateTime(2018, 1, 1), 5, 10, null, formateurId);

            // then
            mockHandler.AllEvents.Should()
                .Contain(new FormateurAssigned(Guid.Empty, 0, new DateTime(2018, 1, 1), 5)).And
                .Contain(new SessionPlanned(Guid.Empty, 0, formationId, new DateTime(2018, 1, 1), 5, 10, null, formateurId));
        }

        [TestMethod]
        public void be_planned_if_lieu_available_for_session()
        {
            // given
            var formationId = Guid.NewGuid();
            
            var lieuId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();
            var mockHandler = new MockHandler<LieuAssigned, SessionPlanned>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();
            eventStore.Save(new LieuCreated(lieuId, 1, "Paris", "test", 5));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            new PlanSession(eventBus).Execute(formationId, new DateTime(2018, 1, 1), 5, 10, lieuId, null);

            // then
            mockHandler.AllEvents.Should()
                .Contain(new LieuAssigned(Guid.Empty, 0, new DateTime(2018, 1, 1), 5)).And
                .Contain(new SessionPlanned(Guid.Empty, 0, formationId, new DateTime(2018, 1, 1), 5, 10, lieuId, null));
        }

        [TestMethod]
        public void plan_new_session_with_no_formateur()
        {
            var formationId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();
            var eventStore = new FakeEventStore();
            var eventBus = new EventBus(dispatcher, eventStore);

            var mockHandler = new MockHandler<SessionPlanned>();
            dispatcher.Register(mockHandler);

            new PlanSession(eventBus).Execute(formationId, new DateTime(2018, 1, 1), 5, 10, null, null);

            mockHandler.AllEvents.Should().HaveCount(1);
            mockHandler.AllEvents.Should().Contain(new SessionPlanned(Guid.Empty, 0, formationId, new DateTime(2018, 1, 1), 5, 10, null, null));
        }

        [TestMethod]
        public void throw_error_if_plan_session_with_formateur_that_not_exists()
        {
            var formationId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();
            var eventStore = new FakeEventStore();
            var eventBus = new EventBus(dispatcher, eventStore);            

            Action action = () => new PlanSession(eventBus).Execute(formationId, new DateTime(2017, 12, 21), 5, 10, Guid.NewGuid(), formationId);
            action.ShouldThrow<FormateurNotExistsException>();
        }

        [TestMethod]
        public void throw_error_if_formateur_not_available_on_planNewSession()
        {
            // given
            var formationId = Guid.NewGuid();
            var formateurId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();

            var eventStore = new FakeEventStore();
            eventStore.Save(new FormateurCreated(formateurId, 1, "BOUDOUX", "Aurelien", "test@test.com"));
            eventStore.Save(new FormateurAssigned(formateurId, 2, new DateTime(2017, 12, 20), 10));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            Action action = () => new PlanSession(eventBus).Execute(formationId, new DateTime(2017, 12, 21), 5, 1, Guid.NewGuid(), formateurId);

            // then
            action.ShouldThrow<FormateurAlreadyAssignedException>();
        }

        [TestMethod]
        public void reassigne_formateurs_when_update_session_with_source_and_destination()
        {
            // given
            var formationId = Guid.NewGuid();
            
            var formateurId1 = Guid.NewGuid();
            var formateurId2 = Guid.NewGuid();
            var lieuId = Guid.NewGuid();

            var sessionId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();
            var mockHandler = new MockHandler<FormateurAssigned, FormateurUnassigned, SessionUpdated>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();
            eventStore.Save(new FormateurCreated(formateurId1, 1, "BOUDOUX", "Aurelien", "test@test.com"));
            eventStore.Save(new FormateurCreated(formateurId2, 1, "Creutzfeldt", "Jacob", "test@test.com"));
            eventStore.Save(new SessionPlanned(sessionId, 1, formationId, new DateTime(2018, 1, 1), 5, 10, lieuId, formateurId1));
            eventStore.Save(new FormateurAssigned(formateurId1, 2, new DateTime(2018, 1, 1), 5));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            new UpdateSession(eventBus).Execute(sessionId, formationId, new DateTime(2018, 1, 1), 5, 10, lieuId, formateurId2);

            // then
            mockHandler.AllEvents.Should()
                .Contain(new FormateurAssigned(Guid.Empty, 0, new DateTime(2018, 1, 1), 5)).And
                .Contain(new FormateurUnassigned(Guid.Empty, 0, new DateTime(2018, 1, 1), 5)).And
                .Contain(new SessionUpdated(Guid.Empty, 0, new DateTime(2018, 1, 1), 5, 10, lieuId, formateurId2, formationId));
        }

        [TestMethod]
        public void reassigne_formateurs_when_update_session_without_source_and_withdestination()
        {
            // given
            var formationId = Guid.NewGuid();

            var formateurId1 = Guid.NewGuid();
            var formateurId2 = Guid.NewGuid();
            var lieuId = Guid.NewGuid();

            var sessionId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();
            var mockHandler = new MockHandler<FormateurAssigned, FormateurUnassigned, SessionUpdated>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();
            eventStore.Save(new FormateurCreated(formateurId1, 1, "BOUDOUX", "Aurelien", "test@test.com"));
            eventStore.Save(new FormateurCreated(formateurId2, 1, "Creutzfeldt", "Jacob", "test@test.com"));
            eventStore.Save(new SessionPlanned(sessionId, 1, formationId, new DateTime(2018, 1, 1), 5, 10, lieuId, null));
            eventStore.Save(new FormateurAssigned(formateurId1, 2, new DateTime(2018, 1, 1), 5));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            new UpdateSession(eventBus).Execute(sessionId, formationId, new DateTime(2018, 1, 1), 5, 10, lieuId, formateurId2);

            // then
            mockHandler.AllEvents.Should()
                .Contain(new FormateurAssigned(Guid.Empty, 0, new DateTime(2018, 1, 1), 5)).And                
                .Contain(new SessionUpdated(Guid.Empty, 0, new DateTime(2018, 1, 1), 5, 10, lieuId, formateurId2, formationId));
        }

        [TestMethod]
        public void reassigne_formateurs_when_update_session_with_source_and_withoutdestination()
        {
            // given
            var formationId = Guid.NewGuid();

            var formateurId1 = Guid.NewGuid();
            var formateurId2 = Guid.NewGuid();
            var lieuId = Guid.NewGuid();

            var sessionId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();
            var mockHandler = new MockHandler<FormateurAssigned, FormateurUnassigned, SessionUpdated>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();
            eventStore.Save(new FormateurCreated(formateurId1, 1, "BOUDOUX", "Aurelien", "test@test.com"));
            eventStore.Save(new FormateurCreated(formateurId2, 1, "Creutzfeldt", "Jacob", "test@test.com"));
            eventStore.Save(new SessionPlanned(sessionId, 1, formationId, new DateTime(2018, 1, 1), 5, 10, lieuId, formateurId1));
            eventStore.Save(new FormateurAssigned(formateurId1, 2, new DateTime(2018, 1, 1), 5));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            new UpdateSession(eventBus).Execute(sessionId, formationId, new DateTime(2018, 1, 1), 5, 10, lieuId, null);

            // then
            mockHandler.AllEvents.Should()
                .Contain(new FormateurUnassigned(Guid.Empty, 0, new DateTime(2018, 1, 1), 5)).And
                .Contain(new SessionUpdated(Guid.Empty, 0, new DateTime(2018, 1, 1), 5, 10, lieuId, null, formationId));
        }

        [TestMethod]
        public void dont_reassigne_formateurs_when_update_session_with_same_source_and_destination()
        {
            // given
            var formationId = Guid.NewGuid();

            var formateurId1 = Guid.NewGuid();
            var formateurId2 = Guid.NewGuid();
            var lieuId = Guid.NewGuid();

            var sessionId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();
            var mockHandler = new MockHandler<FormateurAssigned, FormateurUnassigned, SessionUpdated>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();
            eventStore.Save(new FormateurCreated(formateurId1, 1, "BOUDOUX", "Aurelien", "test@test.com"));
            eventStore.Save(new FormateurCreated(formateurId2, 1, "Creutzfeldt", "Jacob", "test@test.com"));
            eventStore.Save(new SessionPlanned(sessionId, 1, formationId, new DateTime(2018, 1, 1), 5, 10, lieuId, formateurId1));
            eventStore.Save(new FormateurAssigned(formateurId1, 2, new DateTime(2017, 12, 21), 5));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            new UpdateSession(eventBus).Execute(sessionId, formationId, new DateTime(2018, 1, 1), 5, 10, lieuId, formateurId1);

            // then
            mockHandler.AllEvents.Should()
                .Contain(new SessionUpdated(Guid.Empty, 0, new DateTime(2018, 1, 1), 5, 10, lieuId, formateurId1, formationId));
        }

        [TestMethod]
        public void throw_error_if_update_formateur_that_is_not_available()
        {
            // given
            var formationId = Guid.NewGuid();

            var formateurId1 = Guid.NewGuid();
            var formateurId2 = Guid.NewGuid();

            var lieuId = Guid.NewGuid();

            var sessionId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();

            var eventStore = new FakeEventStore();
            eventStore.Save(new FormateurCreated(formateurId1, 1, "BOUDOUX", "Aurelien", "test@test.com"));
            eventStore.Save(new FormateurCreated(formateurId2, 1, "Creutzfeldt", "Jacob", "test@test.com"));
            eventStore.Save(new SessionPlanned(sessionId, 1, formationId, new DateTime(2017, 12, 21), 5, 10, lieuId, formateurId1));
            eventStore.Save(new FormateurAssigned(formateurId1, 2, new DateTime(2017, 12, 21), 5));
            eventStore.Save(new FormateurAssigned(formateurId2, 2, new DateTime(2017, 12, 20), 10));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            Action action = ()=>new UpdateSession(eventBus).Execute(sessionId, formationId, new DateTime(2017, 12, 21), 5, 10, lieuId, formateurId2);

            action.ShouldThrow<FormateurAlreadyAssignedException>();
        }

        [TestMethod]
        public void unassign_formateur_when_session_deleted()
        {
            // given
            var formationId = Guid.NewGuid();
            var formateurId1 = Guid.NewGuid();            

            var sessionId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();
            var mockHandler = new MockHandler<FormateurUnassigned, SessionDeleted>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();
            eventStore.Save(new FormateurCreated(formateurId1, 1, "BOUDOUX", "Aurelien", "test@test.com"));            
            eventStore.Save(new SessionPlanned(sessionId, 1, formationId, new DateTime(2017, 12, 21), 5, 10, null, formateurId1));
            eventStore.Save(new FormateurAssigned(formateurId1, 2, new DateTime(2017, 12, 21), 5));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            new RemoveSession(eventBus).Execute(sessionId);

            // then
            mockHandler.AllEvents.Should()
                .Contain(new FormateurUnassigned(Guid.Empty, 0, new DateTime(2017, 12, 21), 5)).And
                .Contain(new SessionDeleted(Guid.Empty, 0));
        }    

        [TestMethod]
        public void reassign_formateur_if_update_session_period()
        {
            // given
            var formationId = Guid.NewGuid();
            var formateurId1 = Guid.NewGuid();
            var lieuId = Guid.NewGuid();

            var sessionId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();
            var mockHandler = new MockHandler<FormateurReassigned, SessionUpdated>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();
            eventStore.Save(new FormateurCreated(formateurId1, 1, "BOUDOUX", "Aurelien", "test@test.com"));
            eventStore.Save(new SessionPlanned(sessionId, 1, formationId, new DateTime(2018, 1, 1), 5, 10, lieuId, formateurId1));
            eventStore.Save(new FormateurAssigned(formateurId1, 2, new DateTime(2018, 1, 1), 5));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            new UpdateSession(eventBus).Execute(sessionId, formationId, new DateTime(2018, 1, 2), 4, 10, lieuId, formateurId1);

            // then
            mockHandler.AllEvents.Should()
                .Contain(new FormateurReassigned(Guid.Empty, 0, new DateTime(2018, 1, 1), 5, new DateTime(2018, 1, 2), 4)).And                
                .Contain(new SessionUpdated(Guid.Empty, 0, new DateTime(2018, 1, 2), 4, 10, lieuId, formateurId1, formationId));
        }

        [TestMethod]
        public void unassign_all_formateur_when_formation_is_deleted()
        {
            // given
            var formationId = Guid.NewGuid();            

            var formateurId1 = Guid.NewGuid();
            var formateurId2 = Guid.NewGuid();

            var sessionId1 = Guid.NewGuid();
            var sessionId2 = Guid.NewGuid();
            var sessionId3 = Guid.NewGuid();
            var sessionId4 = Guid.NewGuid();
            var sessionId5 = Guid.NewGuid();
            var sessionId6 = Guid.NewGuid();

            var lieuId = Guid.NewGuid();
            
            var dispatcher = new EventDispatcher();
            var mockHandler = new MockHandler<FormateurUnassigned, SessionDeleted, FormationDeleted>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();
            eventStore.Save(new FormateurCreated(formateurId1, 1, "BOUDOUX", "Aurelien", "test@test.com"));
            eventStore.Save(new FormateurCreated(formateurId2, 1, "Creutzfeldt", "Jacob", "test@test.com"));            
            eventStore.Save(new SessionPlanned(sessionId1, 1, formationId, new DateTime(2017, 10, 21), 2, 2, lieuId, formateurId1));
            eventStore.Save(new SessionPlanned(sessionId2, 2, formationId, new DateTime(2017, 11, 21), 2, 2, lieuId, formateurId1));
            eventStore.Save(new SessionPlanned(sessionId3, 3, formationId, new DateTime(2017, 12, 21), 2, 2, lieuId, formateurId1));
            eventStore.Save(new SessionPlanned(sessionId4, 1, formationId, new DateTime(2017, 10, 21), 2, 2, lieuId, formateurId2));
            eventStore.Save(new SessionPlanned(sessionId5, 2, formationId, new DateTime(2017, 11, 21), 2, 2, lieuId, formateurId2));
            eventStore.Save(new SessionPlanned(sessionId6, 3, formationId, new DateTime(2017, 12, 21), 2, 2, lieuId, formateurId2));
            eventStore.Save(new FormateurAssigned(formateurId1, 1, new DateTime(2017, 10, 21), 2));
            eventStore.Save(new FormateurAssigned(formateurId1, 2, new DateTime(2017, 11, 21), 2));
            eventStore.Save(new FormateurAssigned(formateurId1, 3, new DateTime(2017, 12, 21), 2));
            eventStore.Save(new FormateurAssigned(formateurId2, 4, new DateTime(2017, 10, 21), 2));
            eventStore.Save(new FormateurAssigned(formateurId2, 5, new DateTime(2017, 11, 21), 2));
            eventStore.Save(new FormateurAssigned(formateurId2, 6, new DateTime(2017, 12, 21), 2));
            eventStore.Save(new FormationCreated(formationId, 1, "Formation de test",1));
            var eventBus = new EventBus(dispatcher, eventStore);

            var sessionQueries = new FakeSessionQueries();
            sessionQueries.AddSession(formationId, sessionId1, new DateTime(2017, 10, 21), 2,null, formateurId1);
            sessionQueries.AddSession(formationId, sessionId2, new DateTime(2017, 11, 21), 2,null, formateurId1);
            sessionQueries.AddSession(formationId, sessionId3, new DateTime(2017, 12, 21), 2,null, formateurId1);
            sessionQueries.AddSession(formationId, sessionId4, new DateTime(2017, 10, 21), 2,null, formateurId2);
            sessionQueries.AddSession(formationId, sessionId5, new DateTime(2017, 11, 21), 2,null, formateurId2);
            sessionQueries.AddSession(formationId, sessionId6, new DateTime(2017, 12, 21), 2,null, formateurId2);

            // when
            new DeleteFormation(eventBus, sessionQueries).Execute(formationId);
            
            // then
            mockHandler.AllEvents.OfType<SessionDeleted>().Should().HaveCount(6);
            mockHandler.AllEvents.OfType<FormateurUnassigned>().Should().HaveCount(6);
            mockHandler.AllEvents.OfType<FormationDeleted>().Should().HaveCount(1);
        }

        //----------
        [TestMethod]
        public void plan_new_session_with_no_lieu()
        {
            var formationId = Guid.NewGuid();
            var formateurId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();
            var eventStore = new FakeEventStore();
            eventStore.Save(new FormateurCreated(formateurId, 1, "BOUDOUX", "Aurelien", "test@test.com"));
            var eventBus = new EventBus(dispatcher, eventStore);

            var mockHandler = new MockHandler<SessionPlanned>();
            dispatcher.Register(mockHandler);

            new PlanSession(eventBus).Execute(formationId, new DateTime(2018, 1, 1), 5, 10, null, formateurId);

            mockHandler.AllEvents.Should().HaveCount(1);
            mockHandler.AllEvents.Should().Contain(new SessionPlanned(Guid.Empty, 0, formationId, new DateTime(2018, 1, 1), 5, 10, null, formateurId));
        }

        [TestMethod]
        public void throw_error_if_plan_session_with_lieu_that_not_exists()
        {
            var formationId = Guid.NewGuid();
            var formateurId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();
            var eventStore = new FakeEventStore();
            eventStore.Save(new FormateurCreated(formateurId, 1, "BOUDOUX", "Aurelien", "test@test.com"));
            var eventBus = new EventBus(dispatcher, eventStore);

            Action action = () => new PlanSession(eventBus).Execute(formationId, new DateTime(2017, 12, 21), 5, 10, Guid.NewGuid(), formateurId);
            action.ShouldThrow<LieuNotExistsException>();
        }

        [TestMethod]
        public void throw_error_if_lieu_not_available_on_planNewSession()
        {
            // given
            var formationId = Guid.NewGuid();
            var formateurId = Guid.NewGuid();
            var lieuId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();
            var eventStore = new FakeEventStore();
            eventStore.Save(new FormateurCreated(formateurId, 1, "BOUDOUX", "Aurelien", "test@test.com"));
            eventStore.Save(new LieuCreated(lieuId, 1, "Paris", "test", 5));
            eventStore.Save(new LieuAssigned(lieuId, 2, new DateTime(2017, 12, 20), 10));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            Action action = () => new PlanSession(eventBus).Execute(formationId, new DateTime(2017, 12, 21), 5, 1, lieuId, formateurId);

            // then
            action.ShouldThrow<LieuAlreadyAssignedException>();
        }

        [TestMethod]
        public void reassigne_lieux_when_update_session_with_source_and_destination()
        {
            // given
            var formationId = Guid.NewGuid();

            var lieuId1 = Guid.NewGuid();
            var lieuId2 = Guid.NewGuid();
            var formateurId = Guid.NewGuid();

            var sessionId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();
            var mockHandler = new MockHandler<LieuAssigned, LieuUnassigned, SessionUpdated>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();
            eventStore.Save(new LieuCreated(lieuId1, 1, "Lyon", "test", 5));
            eventStore.Save(new LieuCreated(lieuId2, 1, "Paris", "test", 5));
            eventStore.Save(new SessionPlanned(sessionId, 1, formationId, new DateTime(2018, 1, 1), 5, 10, lieuId1, formateurId));
            eventStore.Save(new LieuAssigned(lieuId1, 2, new DateTime(2018, 1, 1), 5));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            new UpdateSession(eventBus).Execute(sessionId, formationId,new DateTime(2018, 1, 1), 5, 10, lieuId2, formateurId);

            // then
            mockHandler.AllEvents.Should()                
                .Contain(new LieuUnassigned(Guid.Empty, 0, new DateTime(2018, 1, 1), 5)).And
                .Contain(new LieuAssigned(Guid.Empty, 0, new DateTime(2018, 1, 1), 5)).And
                .Contain(new SessionUpdated(Guid.Empty, 0, new DateTime(2018, 1, 1), 5, 10, lieuId2, formateurId, formationId));
        }

        [TestMethod]
        public void reassigne_lieux_when_update_session_without_source_and_withdestination()
        {
            // given
            var formationId = Guid.NewGuid();

            var lieuId1 = Guid.NewGuid();
            var lieuId2 = Guid.NewGuid();
            var formateurId = Guid.NewGuid();

            var sessionId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();
            var mockHandler = new MockHandler<LieuAssigned, LieuUnassigned, SessionUpdated>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();
            eventStore.Save(new LieuCreated(lieuId1, 1, "Lyon", "test", 5));
            eventStore.Save(new LieuCreated(lieuId2, 1, "Paris", "test", 5));
            eventStore.Save(new SessionPlanned(sessionId, 1, formationId, new DateTime(2018, 1, 1), 5, 10, null, formateurId));
            eventStore.Save(new LieuAssigned(lieuId1, 2, new DateTime(2018, 1, 1), 5));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            new UpdateSession(eventBus).Execute(sessionId, formationId,new DateTime(2018, 1, 1), 5, 10, lieuId2, formateurId);

            // then
            mockHandler.AllEvents.Should()
                .Contain(new LieuAssigned(Guid.Empty, 0, new DateTime(2018, 1, 1), 5)).And
                .Contain(new SessionUpdated(Guid.Empty, 0, new DateTime(2018, 1, 1), 5, 10, lieuId2, formateurId, formationId));
        }

        [TestMethod]
        public void reassigne_lieux_when_update_session_with_source_and_withoutdestination()
        {
            // given
            var formationId = Guid.NewGuid();

            var lieuId1 = Guid.NewGuid();
            var lieuId2 = Guid.NewGuid();
            var formateurId = Guid.NewGuid();

            var sessionId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();
            var mockHandler = new MockHandler<LieuAssigned, LieuUnassigned, SessionUpdated>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();
            eventStore.Save(new LieuCreated(lieuId1, 1, "Lyon", "test", 5));
            eventStore.Save(new LieuCreated(lieuId2, 1, "Paris", "test", 5));
            eventStore.Save(new SessionPlanned(sessionId, 1, formationId, new DateTime(2018, 1, 1), 5, 10, lieuId1, formateurId));
            eventStore.Save(new LieuAssigned(lieuId1, 2, new DateTime(2018, 1, 1), 5));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            new UpdateSession(eventBus).Execute(sessionId, formationId,new DateTime(2018, 1, 1), 5, 10, null, formateurId);

            // then
            mockHandler.AllEvents.Should()
                .Contain(new LieuUnassigned(Guid.Empty, 0, new DateTime(2018, 1, 1), 5)).And
                .Contain(new SessionUpdated(Guid.Empty, 0, new DateTime(2018, 1, 1), 5, 10, null, formateurId, formationId));
        }

        [TestMethod]
        public void dont_reassigne_lieux_when_update_session_with_same_source_and_destination()
        {
            // given
            var formationId = Guid.NewGuid();

            var lieuId1 = Guid.NewGuid();
            var lieuId2 = Guid.NewGuid();
            var formateurId = Guid.NewGuid();

            var sessionId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();
            var mockHandler = new MockHandler<LieuAssigned, LieuUnassigned, SessionUpdated>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();
            eventStore.Save(new LieuCreated(lieuId1, 1, "Lyon", "test", 5));
            eventStore.Save(new LieuCreated(lieuId2, 1, "Paris", "test", 5));
            eventStore.Save(new SessionPlanned(sessionId, 1, formationId, new DateTime(2018, 1, 1), 5, 10, lieuId1, formateurId));
            eventStore.Save(new LieuAssigned(lieuId1, 2, new DateTime(2018, 1, 1), 5));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            new UpdateSession(eventBus).Execute(sessionId, formationId, new DateTime(2018, 1, 1), 5, 10, lieuId1, formateurId);

            // then
            mockHandler.AllEvents.Should()
                .Contain(new SessionUpdated(Guid.Empty, 0, new DateTime(2018, 1, 1), 5, 10, lieuId1, formateurId, formationId));          
        }

        [TestMethod]
        public void throw_error_if_update_lieu_that_is_not_available()
        {
            // given
            var formationId = Guid.NewGuid();

            var lieuId1 = Guid.NewGuid();
            var lieuId2 = Guid.NewGuid();

            var formateurId = Guid.NewGuid();

            var sessionId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();

            var eventStore = new FakeEventStore();
            eventStore.Save(new LieuCreated(lieuId1, 1, "Lyon", "test", 5));
            eventStore.Save(new LieuCreated(lieuId2, 1, "Paris", "test", 5));
            eventStore.Save(new SessionPlanned(sessionId, 1, formationId, new DateTime(2018, 1, 1), 5, 10, lieuId1, formateurId));
            eventStore.Save(new LieuAssigned(lieuId1, 2, new DateTime(2017, 12, 21), 5));
            eventStore.Save(new LieuAssigned(lieuId2, 2, new DateTime(2017, 12, 20), 10));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            Action action = () => new UpdateSession(eventBus).Execute(sessionId, formationId, new DateTime(2017, 12, 21), 5, 10, lieuId2, formateurId);

            action.ShouldThrow<LieuAlreadyAssignedException>();
        }

        [TestMethod]
        public void unassign_lieu_when_session_deleted()
        {
            // given
            var formationId = Guid.NewGuid();
            var formateurId1 = Guid.NewGuid();
            var lieuId1 = Guid.NewGuid();

            var sessionId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();
            var mockHandler = new MockHandler<FormateurUnassigned, LieuUnassigned, SessionDeleted>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();
            eventStore.Save(new FormateurCreated(formateurId1, 1, "BOUDOUX", "Aurelien", "test@test.com"));
            eventStore.Save(new FormateurAssigned(formateurId1, 2, new DateTime(2017, 12, 21), 5));

            eventStore.Save(new LieuCreated(lieuId1, 1, "Lyon", "test", 5));
            eventStore.Save(new LieuAssigned(lieuId1, 2, new DateTime(2017, 12, 21), 5));

            eventStore.Save(new SessionPlanned(sessionId, 1, formationId, new DateTime(2017, 12, 21), 5, 10, lieuId1, formateurId1));
            
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            new RemoveSession(eventBus).Execute(sessionId);

            // then
            mockHandler.AllEvents.Should()
                .Contain(new FormateurUnassigned(Guid.Empty, 0, new DateTime(2017, 12, 21), 5)).And
                .Contain(new LieuUnassigned(Guid.Empty, 0, new DateTime(2017, 12, 21), 5)).And
                .Contain(new SessionDeleted(Guid.Empty, 0));
        }

        [TestMethod]
        public void reassign_lieu_if_update_session_period()
        {
            // given
            var formationId = Guid.NewGuid();
            var lieuId = Guid.NewGuid();

            var sessionId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();
            var mockHandler = new MockHandler<LieuReassigned, SessionUpdated>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();

            eventStore.Save(new LieuCreated(lieuId, 1, "Lyon", "test", 5));
            eventStore.Save(new LieuAssigned(lieuId, 2, new DateTime(2018, 1, 1), 5));

            eventStore.Save(new SessionPlanned(sessionId, 1, formationId, new DateTime(2018, 1, 1), 5, 10, lieuId, null));
            
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            new UpdateSession(eventBus).Execute(sessionId, formationId, new DateTime(2018, 1, 2), 4, 10, lieuId, null);

            // then
            mockHandler.AllEvents.Should()
                .Contain(new LieuReassigned(Guid.Empty, 0, new DateTime(2018, 1, 1), 5, new DateTime(2018, 1, 2), 4)).And
                .Contain(new SessionUpdated(Guid.Empty, 0, new DateTime(2018, 1, 2), 4, 10, lieuId, null, formationId));
        }

        [TestMethod]
        public void unassign_all_lieu_when_formation_is_deleted()
        {
            // given
            var formationId = Guid.NewGuid();

            var lieuId1 = Guid.NewGuid();
            var lieuId2 = Guid.NewGuid();

            var sessionId1 = Guid.NewGuid();
            var sessionId2 = Guid.NewGuid();
            var sessionId3 = Guid.NewGuid();
            var sessionId4 = Guid.NewGuid();
            var sessionId5 = Guid.NewGuid();
            var sessionId6 = Guid.NewGuid();

            //var formateurId = Guid.NewGuid();

            var dispatcher = new EventDispatcher();
            var mockHandler = new MockHandler<LieuUnassigned, SessionDeleted, FormationDeleted>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();
            eventStore.Save(new LieuCreated(lieuId1, 1, "Paris", "test", 5));
            eventStore.Save(new LieuCreated(lieuId2, 1, "Lyon", "test", 3));
            eventStore.Save(new SessionPlanned(sessionId1, 1, formationId, new DateTime(2017, 10, 21), 2, 2, lieuId1, null));
            eventStore.Save(new SessionPlanned(sessionId2, 2, formationId, new DateTime(2017, 11, 21), 2, 2, lieuId1, null));
            eventStore.Save(new SessionPlanned(sessionId3, 3, formationId, new DateTime(2017, 12, 21), 2, 2, lieuId1, null));
            eventStore.Save(new SessionPlanned(sessionId4, 1, formationId, new DateTime(2017, 10, 21), 2, 2, lieuId2, null));
            eventStore.Save(new SessionPlanned(sessionId5, 2, formationId, new DateTime(2017, 11, 21), 2, 2, lieuId2, null));
            eventStore.Save(new SessionPlanned(sessionId6, 3, formationId, new DateTime(2017, 12, 21), 2, 2, lieuId2, null));
            eventStore.Save(new LieuAssigned(lieuId1, 1, new DateTime(2017, 10, 21), 2));
            eventStore.Save(new LieuAssigned(lieuId1, 2, new DateTime(2017, 11, 21), 2));
            eventStore.Save(new LieuAssigned(lieuId1, 3, new DateTime(2017, 12, 21), 2));
            eventStore.Save(new LieuAssigned(lieuId2, 4, new DateTime(2017, 10, 21), 2));
            eventStore.Save(new LieuAssigned(lieuId2, 5, new DateTime(2017, 11, 21), 2));
            eventStore.Save(new LieuAssigned(lieuId2, 6, new DateTime(2017, 12, 21), 2));
            eventStore.Save(new FormationCreated(formationId, 1, "Formation de test", 1));
            var eventBus = new EventBus(dispatcher, eventStore);

            var sessionQueries = new FakeSessionQueries();
            sessionQueries.AddSession(formationId, sessionId1, new DateTime(2017, 10, 21), 2, lieuId1, null);
            sessionQueries.AddSession(formationId, sessionId2, new DateTime(2017, 11, 21), 2, lieuId1, null);
            sessionQueries.AddSession(formationId, sessionId3, new DateTime(2017, 12, 21), 2, lieuId1, null);
            sessionQueries.AddSession(formationId, sessionId4, new DateTime(2017, 10, 21), 2, lieuId2, null);
            sessionQueries.AddSession(formationId, sessionId5, new DateTime(2017, 11, 21), 2, lieuId2, null);
            sessionQueries.AddSession(formationId, sessionId6, new DateTime(2017, 12, 21), 2, lieuId2, null);

            // when
            new DeleteFormation(eventBus, sessionQueries).Execute(formationId);

            // then
            mockHandler.AllEvents.OfType<SessionDeleted>().Should().HaveCount(6);
            mockHandler.AllEvents.OfType<LieuUnassigned>().Should().HaveCount(6);
            mockHandler.AllEvents.OfType<FormationDeleted>().Should().HaveCount(1);
        }
        //---------

        [TestMethod]
         public void write_projection_with_formation_id()
         {
             var dispatcher = new EventDispatcher();
             var projection = new SessionTestProjection();
             dispatcher.Register(projection);
             var eventBus = new EventBus(dispatcher, new FakeEventStore());

             var formationId = Guid.NewGuid();

             var formateur = new CreateFormateur(eventBus).Execute("BOUDOUX", "Aurelien", "test@test.com");
             var lieu = new CreateLieu(eventBus, new FakeLieuQueries()).Execute("Paris", "test", 5);

             new PlanSession(eventBus).Execute(formationId, new DateTime(2017, 12, 20), 3, 3, lieu.AggregateId, formateur.AggregateId);
             projection.Planned.FormationId.Should().Be(formationId);
         }

        private class SessionTestProjection : IEventHandler<SessionPlanned>
        {
            public SessionPlanned Planned { get; private set; }

            public void Handle(SessionPlanned @event)
            {
                Planned = @event;
            }
        }     
    }
}
