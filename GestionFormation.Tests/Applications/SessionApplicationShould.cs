using System;
using System.Linq;
using FluentAssertions;
using GestionFormation.Applications.Locations;
using GestionFormation.Applications.Sessions;
using GestionFormation.Applications.Trainers;
using GestionFormation.Applications.Trainings;
using GestionFormation.CoreDomain.Locations.Events;
using GestionFormation.CoreDomain.Locations.Exceptions;
using GestionFormation.CoreDomain.Sessions.Events;
using GestionFormation.CoreDomain.Trainers.Events;
using GestionFormation.CoreDomain.Trainers.Exceptions;
using GestionFormation.CoreDomain.Trainings.Events;
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
            var mockHandler = new MockHandler<TrainerAssigned, SessionPlanned>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();
            eventStore.Save(new TrainerCreated(formateurId, 1, "BOUDOUX", "Aurelien", "test@test.com"));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            new PlanSession(eventBus).Execute(formationId, new DateTime(2018, 1, 1), 5, 10, null, formateurId);

            // then
            mockHandler.AllEvents.Should()
                .Contain(new TrainerAssigned(Guid.Empty, 0, new DateTime(2018, 1, 1), 5)).And
                .Contain(new SessionPlanned(Guid.Empty, 0, formationId, new DateTime(2018, 1, 1), 5, 10, null, formateurId));
        }

        [TestMethod]
        public void be_planned_if_lieu_available_for_session()
        {
            // given
            var formationId = Guid.NewGuid();
            
            var lieuId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();
            var mockHandler = new MockHandler<LocationAssigned, SessionPlanned>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();
            eventStore.Save(new LocationCreated(lieuId, 1, "Paris", "test", 5));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            new PlanSession(eventBus).Execute(formationId, new DateTime(2018, 1, 1), 5, 10, lieuId, null);

            // then
            mockHandler.AllEvents.Should()
                .Contain(new LocationAssigned(Guid.Empty, 0, new DateTime(2018, 1, 1), 5)).And
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
            action.ShouldThrow<TrainerNotExistsException>();
        }

        [TestMethod]
        public void throw_error_if_formateur_not_available_on_planNewSession()
        {
            // given
            var formationId = Guid.NewGuid();
            var formateurId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();

            var eventStore = new FakeEventStore();
            eventStore.Save(new TrainerCreated(formateurId, 1, "BOUDOUX", "Aurelien", "test@test.com"));
            eventStore.Save(new TrainerAssigned(formateurId, 2, new DateTime(2017, 12, 20), 10));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            Action action = () => new PlanSession(eventBus).Execute(formationId, new DateTime(2017, 12, 21), 5, 1, Guid.NewGuid(), formateurId);

            // then
            action.ShouldThrow<TrainerAlreadyAssignedException>();
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
            var mockHandler = new MockHandler<TrainerAssigned, TrainerUnassigned, SessionUpdated>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();
            eventStore.Save(new TrainerCreated(formateurId1, 1, "BOUDOUX", "Aurelien", "test@test.com"));
            eventStore.Save(new TrainerCreated(formateurId2, 1, "Creutzfeldt", "Jacob", "test@test.com"));
            eventStore.Save(new SessionPlanned(sessionId, 1, formationId, new DateTime(2018, 1, 1), 5, 10, lieuId, formateurId1));
            eventStore.Save(new TrainerAssigned(formateurId1, 2, new DateTime(2018, 1, 1), 5));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            new UpdateSession(eventBus).Execute(sessionId, formationId, new DateTime(2018, 1, 1), 5, 10, lieuId, formateurId2);

            // then
            mockHandler.AllEvents.Should()
                .Contain(new TrainerAssigned(Guid.Empty, 0, new DateTime(2018, 1, 1), 5)).And
                .Contain(new TrainerUnassigned(Guid.Empty, 0, new DateTime(2018, 1, 1), 5)).And
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
            var mockHandler = new MockHandler<TrainerAssigned, TrainerUnassigned, SessionUpdated>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();
            eventStore.Save(new TrainerCreated(formateurId1, 1, "BOUDOUX", "Aurelien", "test@test.com"));
            eventStore.Save(new TrainerCreated(formateurId2, 1, "Creutzfeldt", "Jacob", "test@test.com"));
            eventStore.Save(new SessionPlanned(sessionId, 1, formationId, new DateTime(2018, 1, 1), 5, 10, lieuId, null));
            eventStore.Save(new TrainerAssigned(formateurId1, 2, new DateTime(2018, 1, 1), 5));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            new UpdateSession(eventBus).Execute(sessionId, formationId, new DateTime(2018, 1, 1), 5, 10, lieuId, formateurId2);

            // then
            mockHandler.AllEvents.Should()
                .Contain(new TrainerAssigned(Guid.Empty, 0, new DateTime(2018, 1, 1), 5)).And                
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
            var mockHandler = new MockHandler<TrainerAssigned, TrainerUnassigned, SessionUpdated>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();
            eventStore.Save(new TrainerCreated(formateurId1, 1, "BOUDOUX", "Aurelien", "test@test.com"));
            eventStore.Save(new TrainerCreated(formateurId2, 1, "Creutzfeldt", "Jacob", "test@test.com"));
            eventStore.Save(new SessionPlanned(sessionId, 1, formationId, new DateTime(2018, 1, 1), 5, 10, lieuId, formateurId1));
            eventStore.Save(new TrainerAssigned(formateurId1, 2, new DateTime(2018, 1, 1), 5));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            new UpdateSession(eventBus).Execute(sessionId, formationId, new DateTime(2018, 1, 1), 5, 10, lieuId, null);

            // then
            mockHandler.AllEvents.Should()
                .Contain(new TrainerUnassigned(Guid.Empty, 0, new DateTime(2018, 1, 1), 5)).And
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
            var mockHandler = new MockHandler<TrainerAssigned, TrainerUnassigned, SessionUpdated>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();
            eventStore.Save(new TrainerCreated(formateurId1, 1, "BOUDOUX", "Aurelien", "test@test.com"));
            eventStore.Save(new TrainerCreated(formateurId2, 1, "Creutzfeldt", "Jacob", "test@test.com"));
            eventStore.Save(new SessionPlanned(sessionId, 1, formationId, new DateTime(2018, 1, 1), 5, 10, lieuId, formateurId1));
            eventStore.Save(new TrainerAssigned(formateurId1, 2, new DateTime(2017, 12, 21), 5));
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
            eventStore.Save(new TrainerCreated(formateurId1, 1, "BOUDOUX", "Aurelien", "test@test.com"));
            eventStore.Save(new TrainerCreated(formateurId2, 1, "Creutzfeldt", "Jacob", "test@test.com"));
            eventStore.Save(new SessionPlanned(sessionId, 1, formationId, new DateTime(2017, 12, 21), 5, 10, lieuId, formateurId1));
            eventStore.Save(new TrainerAssigned(formateurId1, 2, new DateTime(2017, 12, 21), 5));
            eventStore.Save(new TrainerAssigned(formateurId2, 2, new DateTime(2017, 12, 20), 10));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            Action action = ()=>new UpdateSession(eventBus).Execute(sessionId, formationId, new DateTime(2017, 12, 21), 5, 10, lieuId, formateurId2);

            action.ShouldThrow<TrainerAlreadyAssignedException>();
        }

        [TestMethod]
        public void unassign_formateur_when_session_deleted()
        {
            // given
            var formationId = Guid.NewGuid();
            var formateurId1 = Guid.NewGuid();            

            var sessionId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();
            var mockHandler = new MockHandler<TrainerUnassigned, SessionDeleted>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();
            eventStore.Save(new TrainerCreated(formateurId1, 1, "BOUDOUX", "Aurelien", "test@test.com"));            
            eventStore.Save(new SessionPlanned(sessionId, 1, formationId, new DateTime(2017, 12, 21), 5, 10, null, formateurId1));
            eventStore.Save(new TrainerAssigned(formateurId1, 2, new DateTime(2017, 12, 21), 5));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            new RemoveSession(eventBus).Execute(sessionId);

            // then
            mockHandler.AllEvents.Should()
                .Contain(new TrainerUnassigned(Guid.Empty, 0, new DateTime(2017, 12, 21), 5)).And
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
            var mockHandler = new MockHandler<TrainerReassigned, SessionUpdated>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();
            eventStore.Save(new TrainerCreated(formateurId1, 1, "BOUDOUX", "Aurelien", "test@test.com"));
            eventStore.Save(new SessionPlanned(sessionId, 1, formationId, new DateTime(2018, 1, 1), 5, 10, lieuId, formateurId1));
            eventStore.Save(new TrainerAssigned(formateurId1, 2, new DateTime(2018, 1, 1), 5));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            new UpdateSession(eventBus).Execute(sessionId, formationId, new DateTime(2018, 1, 2), 4, 10, lieuId, formateurId1);

            // then
            mockHandler.AllEvents.Should()
                .Contain(new TrainerReassigned(Guid.Empty, 0, new DateTime(2018, 1, 1), 5, new DateTime(2018, 1, 2), 4)).And                
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
            var mockHandler = new MockHandler<TrainerUnassigned, SessionDeleted, TrainingDeleted>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();
            eventStore.Save(new TrainerCreated(formateurId1, 1, "BOUDOUX", "Aurelien", "test@test.com"));
            eventStore.Save(new TrainerCreated(formateurId2, 1, "Creutzfeldt", "Jacob", "test@test.com"));            
            eventStore.Save(new SessionPlanned(sessionId1, 1, formationId, new DateTime(2017, 10, 21), 2, 2, lieuId, formateurId1));
            eventStore.Save(new SessionPlanned(sessionId2, 2, formationId, new DateTime(2017, 11, 21), 2, 2, lieuId, formateurId1));
            eventStore.Save(new SessionPlanned(sessionId3, 3, formationId, new DateTime(2017, 12, 21), 2, 2, lieuId, formateurId1));
            eventStore.Save(new SessionPlanned(sessionId4, 1, formationId, new DateTime(2017, 10, 21), 2, 2, lieuId, formateurId2));
            eventStore.Save(new SessionPlanned(sessionId5, 2, formationId, new DateTime(2017, 11, 21), 2, 2, lieuId, formateurId2));
            eventStore.Save(new SessionPlanned(sessionId6, 3, formationId, new DateTime(2017, 12, 21), 2, 2, lieuId, formateurId2));
            eventStore.Save(new TrainerAssigned(formateurId1, 1, new DateTime(2017, 10, 21), 2));
            eventStore.Save(new TrainerAssigned(formateurId1, 2, new DateTime(2017, 11, 21), 2));
            eventStore.Save(new TrainerAssigned(formateurId1, 3, new DateTime(2017, 12, 21), 2));
            eventStore.Save(new TrainerAssigned(formateurId2, 4, new DateTime(2017, 10, 21), 2));
            eventStore.Save(new TrainerAssigned(formateurId2, 5, new DateTime(2017, 11, 21), 2));
            eventStore.Save(new TrainerAssigned(formateurId2, 6, new DateTime(2017, 12, 21), 2));
            eventStore.Save(new TrainingCreated(formationId, 1, "Formation de test",1));
            var eventBus = new EventBus(dispatcher, eventStore);

            var sessionQueries = new FakeSessionQueries();
            sessionQueries.AddSession(formationId, sessionId1, new DateTime(2017, 10, 21), 2,null, formateurId1);
            sessionQueries.AddSession(formationId, sessionId2, new DateTime(2017, 11, 21), 2,null, formateurId1);
            sessionQueries.AddSession(formationId, sessionId3, new DateTime(2017, 12, 21), 2,null, formateurId1);
            sessionQueries.AddSession(formationId, sessionId4, new DateTime(2017, 10, 21), 2,null, formateurId2);
            sessionQueries.AddSession(formationId, sessionId5, new DateTime(2017, 11, 21), 2,null, formateurId2);
            sessionQueries.AddSession(formationId, sessionId6, new DateTime(2017, 12, 21), 2,null, formateurId2);

            // when
            new DeleteTraining(eventBus, sessionQueries).Execute(formationId);
            
            // then
            mockHandler.AllEvents.OfType<SessionDeleted>().Should().HaveCount(6);
            mockHandler.AllEvents.OfType<TrainerUnassigned>().Should().HaveCount(6);
            mockHandler.AllEvents.OfType<TrainingDeleted>().Should().HaveCount(1);
        }

        //----------
        [TestMethod]
        public void plan_new_session_with_no_lieu()
        {
            var formationId = Guid.NewGuid();
            var formateurId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();
            var eventStore = new FakeEventStore();
            eventStore.Save(new TrainerCreated(formateurId, 1, "BOUDOUX", "Aurelien", "test@test.com"));
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
            eventStore.Save(new TrainerCreated(formateurId, 1, "BOUDOUX", "Aurelien", "test@test.com"));
            var eventBus = new EventBus(dispatcher, eventStore);

            Action action = () => new PlanSession(eventBus).Execute(formationId, new DateTime(2017, 12, 21), 5, 10, Guid.NewGuid(), formateurId);
            action.ShouldThrow<LocationNotExistsException>();
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
            eventStore.Save(new TrainerCreated(formateurId, 1, "BOUDOUX", "Aurelien", "test@test.com"));
            eventStore.Save(new LocationCreated(lieuId, 1, "Paris", "test", 5));
            eventStore.Save(new LocationAssigned(lieuId, 2, new DateTime(2017, 12, 20), 10));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            Action action = () => new PlanSession(eventBus).Execute(formationId, new DateTime(2017, 12, 21), 5, 1, lieuId, formateurId);

            // then
            action.ShouldThrow<LocationAlreadyAssignedException>();
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
            var mockHandler = new MockHandler<LocationAssigned, LocationUnassigned, SessionUpdated>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();
            eventStore.Save(new LocationCreated(lieuId1, 1, "Lyon", "test", 5));
            eventStore.Save(new LocationCreated(lieuId2, 1, "Paris", "test", 5));
            eventStore.Save(new SessionPlanned(sessionId, 1, formationId, new DateTime(2018, 1, 1), 5, 10, lieuId1, formateurId));
            eventStore.Save(new LocationAssigned(lieuId1, 2, new DateTime(2018, 1, 1), 5));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            new UpdateSession(eventBus).Execute(sessionId, formationId,new DateTime(2018, 1, 1), 5, 10, lieuId2, formateurId);

            // then
            mockHandler.AllEvents.Should()                
                .Contain(new LocationUnassigned(Guid.Empty, 0, new DateTime(2018, 1, 1), 5)).And
                .Contain(new LocationAssigned(Guid.Empty, 0, new DateTime(2018, 1, 1), 5)).And
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
            var mockHandler = new MockHandler<LocationAssigned, LocationUnassigned, SessionUpdated>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();
            eventStore.Save(new LocationCreated(lieuId1, 1, "Lyon", "test", 5));
            eventStore.Save(new LocationCreated(lieuId2, 1, "Paris", "test", 5));
            eventStore.Save(new SessionPlanned(sessionId, 1, formationId, new DateTime(2018, 1, 1), 5, 10, null, formateurId));
            eventStore.Save(new LocationAssigned(lieuId1, 2, new DateTime(2018, 1, 1), 5));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            new UpdateSession(eventBus).Execute(sessionId, formationId,new DateTime(2018, 1, 1), 5, 10, lieuId2, formateurId);

            // then
            mockHandler.AllEvents.Should()
                .Contain(new LocationAssigned(Guid.Empty, 0, new DateTime(2018, 1, 1), 5)).And
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
            var mockHandler = new MockHandler<LocationAssigned, LocationUnassigned, SessionUpdated>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();
            eventStore.Save(new LocationCreated(lieuId1, 1, "Lyon", "test", 5));
            eventStore.Save(new LocationCreated(lieuId2, 1, "Paris", "test", 5));
            eventStore.Save(new SessionPlanned(sessionId, 1, formationId, new DateTime(2018, 1, 1), 5, 10, lieuId1, formateurId));
            eventStore.Save(new LocationAssigned(lieuId1, 2, new DateTime(2018, 1, 1), 5));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            new UpdateSession(eventBus).Execute(sessionId, formationId,new DateTime(2018, 1, 1), 5, 10, null, formateurId);

            // then
            mockHandler.AllEvents.Should()
                .Contain(new LocationUnassigned(Guid.Empty, 0, new DateTime(2018, 1, 1), 5)).And
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
            var mockHandler = new MockHandler<LocationAssigned, LocationUnassigned, SessionUpdated>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();
            eventStore.Save(new LocationCreated(lieuId1, 1, "Lyon", "test", 5));
            eventStore.Save(new LocationCreated(lieuId2, 1, "Paris", "test", 5));
            eventStore.Save(new SessionPlanned(sessionId, 1, formationId, new DateTime(2018, 1, 1), 5, 10, lieuId1, formateurId));
            eventStore.Save(new LocationAssigned(lieuId1, 2, new DateTime(2018, 1, 1), 5));
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
            eventStore.Save(new LocationCreated(lieuId1, 1, "Lyon", "test", 5));
            eventStore.Save(new LocationCreated(lieuId2, 1, "Paris", "test", 5));
            eventStore.Save(new SessionPlanned(sessionId, 1, formationId, new DateTime(2018, 1, 1), 5, 10, lieuId1, formateurId));
            eventStore.Save(new LocationAssigned(lieuId1, 2, new DateTime(2017, 12, 21), 5));
            eventStore.Save(new LocationAssigned(lieuId2, 2, new DateTime(2017, 12, 20), 10));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            Action action = () => new UpdateSession(eventBus).Execute(sessionId, formationId, new DateTime(2017, 12, 21), 5, 10, lieuId2, formateurId);

            action.ShouldThrow<LocationAlreadyAssignedException>();
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
            var mockHandler = new MockHandler<TrainerUnassigned, LocationUnassigned, SessionDeleted>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();
            eventStore.Save(new TrainerCreated(formateurId1, 1, "BOUDOUX", "Aurelien", "test@test.com"));
            eventStore.Save(new TrainerAssigned(formateurId1, 2, new DateTime(2017, 12, 21), 5));

            eventStore.Save(new LocationCreated(lieuId1, 1, "Lyon", "test", 5));
            eventStore.Save(new LocationAssigned(lieuId1, 2, new DateTime(2017, 12, 21), 5));

            eventStore.Save(new SessionPlanned(sessionId, 1, formationId, new DateTime(2017, 12, 21), 5, 10, lieuId1, formateurId1));
            
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            new RemoveSession(eventBus).Execute(sessionId);

            // then
            mockHandler.AllEvents.Should()
                .Contain(new TrainerUnassigned(Guid.Empty, 0, new DateTime(2017, 12, 21), 5)).And
                .Contain(new LocationUnassigned(Guid.Empty, 0, new DateTime(2017, 12, 21), 5)).And
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
            var mockHandler = new MockHandler<LocationReassigned, SessionUpdated>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();

            eventStore.Save(new LocationCreated(lieuId, 1, "Lyon", "test", 5));
            eventStore.Save(new LocationAssigned(lieuId, 2, new DateTime(2018, 1, 1), 5));

            eventStore.Save(new SessionPlanned(sessionId, 1, formationId, new DateTime(2018, 1, 1), 5, 10, lieuId, null));
            
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            new UpdateSession(eventBus).Execute(sessionId, formationId, new DateTime(2018, 1, 2), 4, 10, lieuId, null);

            // then
            mockHandler.AllEvents.Should()
                .Contain(new LocationReassigned(Guid.Empty, 0, new DateTime(2018, 1, 1), 5, new DateTime(2018, 1, 2), 4)).And
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
            var mockHandler = new MockHandler<LocationUnassigned, SessionDeleted, TrainingDeleted>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();
            eventStore.Save(new LocationCreated(lieuId1, 1, "Paris", "test", 5));
            eventStore.Save(new LocationCreated(lieuId2, 1, "Lyon", "test", 3));
            eventStore.Save(new SessionPlanned(sessionId1, 1, formationId, new DateTime(2017, 10, 21), 2, 2, lieuId1, null));
            eventStore.Save(new SessionPlanned(sessionId2, 2, formationId, new DateTime(2017, 11, 21), 2, 2, lieuId1, null));
            eventStore.Save(new SessionPlanned(sessionId3, 3, formationId, new DateTime(2017, 12, 21), 2, 2, lieuId1, null));
            eventStore.Save(new SessionPlanned(sessionId4, 1, formationId, new DateTime(2017, 10, 21), 2, 2, lieuId2, null));
            eventStore.Save(new SessionPlanned(sessionId5, 2, formationId, new DateTime(2017, 11, 21), 2, 2, lieuId2, null));
            eventStore.Save(new SessionPlanned(sessionId6, 3, formationId, new DateTime(2017, 12, 21), 2, 2, lieuId2, null));
            eventStore.Save(new LocationAssigned(lieuId1, 1, new DateTime(2017, 10, 21), 2));
            eventStore.Save(new LocationAssigned(lieuId1, 2, new DateTime(2017, 11, 21), 2));
            eventStore.Save(new LocationAssigned(lieuId1, 3, new DateTime(2017, 12, 21), 2));
            eventStore.Save(new LocationAssigned(lieuId2, 4, new DateTime(2017, 10, 21), 2));
            eventStore.Save(new LocationAssigned(lieuId2, 5, new DateTime(2017, 11, 21), 2));
            eventStore.Save(new LocationAssigned(lieuId2, 6, new DateTime(2017, 12, 21), 2));
            eventStore.Save(new TrainingCreated(formationId, 1, "Formation de test", 1));
            var eventBus = new EventBus(dispatcher, eventStore);

            var sessionQueries = new FakeSessionQueries();
            sessionQueries.AddSession(formationId, sessionId1, new DateTime(2017, 10, 21), 2, lieuId1, null);
            sessionQueries.AddSession(formationId, sessionId2, new DateTime(2017, 11, 21), 2, lieuId1, null);
            sessionQueries.AddSession(formationId, sessionId3, new DateTime(2017, 12, 21), 2, lieuId1, null);
            sessionQueries.AddSession(formationId, sessionId4, new DateTime(2017, 10, 21), 2, lieuId2, null);
            sessionQueries.AddSession(formationId, sessionId5, new DateTime(2017, 11, 21), 2, lieuId2, null);
            sessionQueries.AddSession(formationId, sessionId6, new DateTime(2017, 12, 21), 2, lieuId2, null);

            // when
            new DeleteTraining(eventBus, sessionQueries).Execute(formationId);

            // then
            mockHandler.AllEvents.OfType<SessionDeleted>().Should().HaveCount(6);
            mockHandler.AllEvents.OfType<LocationUnassigned>().Should().HaveCount(6);
            mockHandler.AllEvents.OfType<TrainingDeleted>().Should().HaveCount(1);
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

             var formateur = new CreateTrainer(eventBus).Execute("BOUDOUX", "Aurelien", "test@test.com");
             var lieu = new CreateLocation(eventBus, new FakeLocationQueries()).Execute("Paris", "test", 5);

             new PlanSession(eventBus).Execute(formationId, new DateTime(2017, 12, 20), 3, 3, lieu.AggregateId, formateur.AggregateId);
             projection.Planned.TrainingId.Should().Be(formationId);
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
