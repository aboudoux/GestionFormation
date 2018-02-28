using System;
using System.Drawing;
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
        public void be_planned_if_trainner_available_for_session()
        {
            // given
            var trainingId = Guid.NewGuid();
            var trainerId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();
            var mockHandler = new MockHandler<TrainerAssigned, SessionPlanned>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();
            eventStore.Save(new TrainerCreated(trainerId, 1, "BOUDOUX", "Aurelien", "test@test.com"));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            new PlanSession(eventBus).Execute(trainingId, new DateTime(2018, 1, 1), 5, 10, null, trainerId);

            // then
            mockHandler.AllEvents.Should()
                .Contain(new TrainerAssigned(Guid.Empty, 0, new DateTime(2018, 1, 1), 5)).And
                .Contain(new SessionPlanned(Guid.Empty, 0, trainingId, new DateTime(2018, 1, 1), 5, 10, null, trainerId));
        }

        [TestMethod]
        public void be_planned_if_location_available_for_session()
        {
            // given
            var trainingId = Guid.NewGuid();
            
            var locationId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();
            var mockHandler = new MockHandler<LocationAssigned, SessionPlanned>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();
            eventStore.Save(new LocationCreated(locationId, 1, "Paris", "test", 5));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            new PlanSession(eventBus).Execute(trainingId, new DateTime(2018, 1, 1), 5, 10, locationId, null);

            // then
            mockHandler.AllEvents.Should()
                .Contain(new LocationAssigned(Guid.Empty, 0, new DateTime(2018, 1, 1), 5)).And
                .Contain(new SessionPlanned(Guid.Empty, 0, trainingId, new DateTime(2018, 1, 1), 5, 10, locationId, null));
        }

        [TestMethod]
        public void plan_new_session_with_no_trainer()
        {
            var trainingId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();
            var eventStore = new FakeEventStore();
            var eventBus = new EventBus(dispatcher, eventStore);

            var mockHandler = new MockHandler<SessionPlanned>();
            dispatcher.Register(mockHandler);

            new PlanSession(eventBus).Execute(trainingId, new DateTime(2018, 1, 1), 5, 10, null, null);

            mockHandler.AllEvents.Should().HaveCount(1);
            mockHandler.AllEvents.Should().Contain(new SessionPlanned(Guid.Empty, 0, trainingId, new DateTime(2018, 1, 1), 5, 10, null, null));
        }

        [TestMethod]
        public void throw_error_if_plan_session_with_trainer_that_not_exists()
        {
            var trainingId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();
            var eventStore = new FakeEventStore();
            var eventBus = new EventBus(dispatcher, eventStore);            

            Action action = () => new PlanSession(eventBus).Execute(trainingId, new DateTime(2017, 12, 21), 5, 10, Guid.NewGuid(), trainingId);
            action.ShouldThrow<TrainerNotExistsException>();
        }

        [TestMethod]
        public void throw_error_if_trainer_not_available_on_planNewSession()
        {
            // given
            var trainingId = Guid.NewGuid();
            var trainerId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();

            var eventStore = new FakeEventStore();
            eventStore.Save(new TrainerCreated(trainerId, 1, "BOUDOUX", "Aurelien", "test@test.com"));
            eventStore.Save(new TrainerAssigned(trainerId, 2, new DateTime(2017, 12, 20), 10));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            Action action = () => new PlanSession(eventBus).Execute(trainingId, new DateTime(2017, 12, 21), 5, 1, Guid.NewGuid(), trainerId);

            // then
            action.ShouldThrow<TrainerAlreadyAssignedException>();
        }

        [TestMethod]
        public void reassign_trainers_when_update_session_with_source_and_destination()
        {
            // given
            var trainingId = Guid.NewGuid();
            
            var trainer1Id = Guid.NewGuid();
            var trainer2Id = Guid.NewGuid();
            var locationId = Guid.NewGuid();

            var sessionId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();
            var mockHandler = new MockHandler<TrainerAssigned, TrainerUnassigned, SessionUpdated>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();
            eventStore.Save(new TrainerCreated(trainer1Id, 1, "BOUDOUX", "Aurelien", "test@test.com"));
            eventStore.Save(new TrainerCreated(trainer2Id, 1, "Creutzfeldt", "Jacob", "test@test.com"));
            eventStore.Save(new SessionPlanned(sessionId, 1, trainingId, new DateTime(2018, 1, 1), 5, 10, locationId, trainer1Id));
            eventStore.Save(new TrainerAssigned(trainer1Id, 2, new DateTime(2018, 1, 1), 5));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            new UpdateSession(eventBus).Execute(sessionId, trainingId, new DateTime(2018, 1, 1), 5, 10, locationId, trainer2Id);

            // then
            mockHandler.AllEvents.Should()
                .Contain(new TrainerAssigned(Guid.Empty, 0, new DateTime(2018, 1, 1), 5)).And
                .Contain(new TrainerUnassigned(Guid.Empty, 0, new DateTime(2018, 1, 1), 5)).And
                .Contain(new SessionUpdated(Guid.Empty, 0, new DateTime(2018, 1, 1), 5, 10, locationId, trainer2Id, trainingId));
        }

        [TestMethod]
        public void reassign_trainers_when_update_session_without_source_and_withdestination()
        {
            // given
            var trainingId = Guid.NewGuid();

            var trainerId1 = Guid.NewGuid();
            var trainerId2 = Guid.NewGuid();
            var locationId = Guid.NewGuid();

            var sessionId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();
            var mockHandler = new MockHandler<TrainerAssigned, TrainerUnassigned, SessionUpdated>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();
            eventStore.Save(new TrainerCreated(trainerId1, 1, "BOUDOUX", "Aurelien", "test@test.com"));
            eventStore.Save(new TrainerCreated(trainerId2, 1, "Creutzfeldt", "Jacob", "test@test.com"));
            eventStore.Save(new SessionPlanned(sessionId, 1, trainingId, new DateTime(2018, 1, 1), 5, 10, locationId, null));
            eventStore.Save(new TrainerAssigned(trainerId1, 2, new DateTime(2018, 1, 1), 5));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            new UpdateSession(eventBus).Execute(sessionId, trainingId, new DateTime(2018, 1, 1), 5, 10, locationId, trainerId2);

            // then
            mockHandler.AllEvents.Should()
                .Contain(new TrainerAssigned(Guid.Empty, 0, new DateTime(2018, 1, 1), 5)).And                
                .Contain(new SessionUpdated(Guid.Empty, 0, new DateTime(2018, 1, 1), 5, 10, locationId, trainerId2, trainingId));
        }

        [TestMethod]
        public void reassign_trainers_when_update_session_with_source_and_withoutdestination()
        {
            // given
            var trainingId = Guid.NewGuid();

            var trainer1Id = Guid.NewGuid();
            var trainer2Id = Guid.NewGuid();
            var locationId = Guid.NewGuid();

            var sessionId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();
            var mockHandler = new MockHandler<TrainerAssigned, TrainerUnassigned, SessionUpdated>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();
            eventStore.Save(new TrainerCreated(trainer1Id, 1, "BOUDOUX", "Aurelien", "test@test.com"));
            eventStore.Save(new TrainerCreated(trainer2Id, 1, "Creutzfeldt", "Jacob", "test@test.com"));
            eventStore.Save(new SessionPlanned(sessionId, 1, trainingId, new DateTime(2018, 1, 1), 5, 10, locationId, trainer1Id));
            eventStore.Save(new TrainerAssigned(trainer1Id, 2, new DateTime(2018, 1, 1), 5));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            new UpdateSession(eventBus).Execute(sessionId, trainingId, new DateTime(2018, 1, 1), 5, 10, locationId, null);

            // then
            mockHandler.AllEvents.Should()
                .Contain(new TrainerUnassigned(Guid.Empty, 0, new DateTime(2018, 1, 1), 5)).And
                .Contain(new SessionUpdated(Guid.Empty, 0, new DateTime(2018, 1, 1), 5, 10, locationId, null, trainingId));
        }

        [TestMethod]
        public void dont_reassign_trainers_when_update_session_with_same_source_and_destination()
        {
            // given
            var trainingId = Guid.NewGuid();

            var trainerId1 = Guid.NewGuid();
            var trainerId2 = Guid.NewGuid();
            var lieuId = Guid.NewGuid();

            var sessionId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();
            var mockHandler = new MockHandler<TrainerAssigned, TrainerUnassigned, SessionUpdated>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();
            eventStore.Save(new TrainerCreated(trainerId1, 1, "BOUDOUX", "Aurelien", "test@test.com"));
            eventStore.Save(new TrainerCreated(trainerId2, 1, "Creutzfeldt", "Jacob", "test@test.com"));
            eventStore.Save(new SessionPlanned(sessionId, 1, trainingId, new DateTime(2018, 1, 1), 5, 10, lieuId, trainerId1));
            eventStore.Save(new TrainerAssigned(trainerId1, 2, new DateTime(2017, 12, 21), 5));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            new UpdateSession(eventBus).Execute(sessionId, trainingId, new DateTime(2018, 1, 1), 5, 10, lieuId, trainerId1);

            // then
            mockHandler.AllEvents.Should()
                .Contain(new SessionUpdated(Guid.Empty, 0, new DateTime(2018, 1, 1), 5, 10, lieuId, trainerId1, trainingId));
        }

        [TestMethod]
        public void throw_error_if_update_trainer_that_is_not_available()
        {
            // given
            var trainingId = Guid.NewGuid();

            var trainerId1 = Guid.NewGuid();
            var trainerId2 = Guid.NewGuid();

            var locationId = Guid.NewGuid();

            var sessionId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();

            var eventStore = new FakeEventStore();
            eventStore.Save(new TrainerCreated(trainerId1, 1, "BOUDOUX", "Aurelien", "test@test.com"));
            eventStore.Save(new TrainerCreated(trainerId2, 1, "Creutzfeldt", "Jacob", "test@test.com"));
            eventStore.Save(new SessionPlanned(sessionId, 1, trainingId, new DateTime(2017, 12, 21), 5, 10, locationId, trainerId1));
            eventStore.Save(new TrainerAssigned(trainerId1, 2, new DateTime(2017, 12, 21), 5));
            eventStore.Save(new TrainerAssigned(trainerId2, 2, new DateTime(2017, 12, 20), 10));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            Action action = ()=>new UpdateSession(eventBus).Execute(sessionId, trainingId, new DateTime(2017, 12, 21), 5, 10, locationId, trainerId2);

            action.ShouldThrow<TrainerAlreadyAssignedException>();
        }

        [TestMethod]
        public void unassign_trainer_when_session_deleted()
        {
            // given
            var trainingId = Guid.NewGuid();
            var trainerId = Guid.NewGuid();            

            var sessionId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();
            var mockHandler = new MockHandler<TrainerUnassigned, SessionDeleted>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();
            eventStore.Save(new TrainerCreated(trainerId, 1, "BOUDOUX", "Aurelien", "test@test.com"));            
            eventStore.Save(new SessionPlanned(sessionId, 1, trainingId, new DateTime(2017, 12, 21), 5, 10, null, trainerId));
            eventStore.Save(new TrainerAssigned(trainerId, 2, new DateTime(2017, 12, 21), 5));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            new RemoveSession(eventBus).Execute(sessionId);

            // then
            mockHandler.AllEvents.Should()
                .Contain(new TrainerUnassigned(Guid.Empty, 0, new DateTime(2017, 12, 21), 5)).And
                .Contain(new SessionDeleted(Guid.Empty, 0));
        }    

        [TestMethod]
        public void reassign_trainer_if_update_session_period()
        {
            // given
            var trainingId = Guid.NewGuid();
            var trainerId = Guid.NewGuid();
            var locationId = Guid.NewGuid();

            var sessionId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();
            var mockHandler = new MockHandler<TrainerReassigned, SessionUpdated>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();
            eventStore.Save(new TrainerCreated(trainerId, 1, "BOUDOUX", "Aurelien", "test@test.com"));
            eventStore.Save(new SessionPlanned(sessionId, 1, trainingId, new DateTime(2018, 1, 1), 5, 10, locationId, trainerId));
            eventStore.Save(new TrainerAssigned(trainerId, 2, new DateTime(2018, 1, 1), 5));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            new UpdateSession(eventBus).Execute(sessionId, trainingId, new DateTime(2018, 1, 2), 4, 10, locationId, trainerId);

            // then
            mockHandler.AllEvents.Should()
                .Contain(new TrainerReassigned(Guid.Empty, 0, new DateTime(2018, 1, 1), 5, new DateTime(2018, 1, 2), 4)).And                
                .Contain(new SessionUpdated(Guid.Empty, 0, new DateTime(2018, 1, 2), 4, 10, locationId, trainerId, trainingId));
        }

        [TestMethod]
        public void unassign_all_trainer_when_training_is_deleted()
        {
            // given
            var trainingId = Guid.NewGuid();            

            var trainerId1 = Guid.NewGuid();
            var trainerId2 = Guid.NewGuid();

            var sessionId1 = Guid.NewGuid();
            var sessionId2 = Guid.NewGuid();
            var sessionId3 = Guid.NewGuid();
            var sessionId4 = Guid.NewGuid();
            var sessionId5 = Guid.NewGuid();
            var sessionId6 = Guid.NewGuid();

            var locationId = Guid.NewGuid();
            
            var dispatcher = new EventDispatcher();
            var mockHandler = new MockHandler<TrainerUnassigned, SessionDeleted, TrainingDeleted>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();
            eventStore.Save(new TrainerCreated(trainerId1, 1, "BOUDOUX", "Aurelien", "test@test.com"));
            eventStore.Save(new TrainerCreated(trainerId2, 1, "Creutzfeldt", "Jacob", "test@test.com"));            
            eventStore.Save(new SessionPlanned(sessionId1, 1, trainingId, new DateTime(2017, 10, 21), 2, 2, locationId, trainerId1));
            eventStore.Save(new SessionPlanned(sessionId2, 2, trainingId, new DateTime(2017, 11, 21), 2, 2, locationId, trainerId1));
            eventStore.Save(new SessionPlanned(sessionId3, 3, trainingId, new DateTime(2017, 12, 21), 2, 2, locationId, trainerId1));
            eventStore.Save(new SessionPlanned(sessionId4, 1, trainingId, new DateTime(2017, 10, 21), 2, 2, locationId, trainerId2));
            eventStore.Save(new SessionPlanned(sessionId5, 2, trainingId, new DateTime(2017, 11, 21), 2, 2, locationId, trainerId2));
            eventStore.Save(new SessionPlanned(sessionId6, 3, trainingId, new DateTime(2017, 12, 21), 2, 2, locationId, trainerId2));
            eventStore.Save(new TrainerAssigned(trainerId1, 1, new DateTime(2017, 10, 21), 2));
            eventStore.Save(new TrainerAssigned(trainerId1, 2, new DateTime(2017, 11, 21), 2));
            eventStore.Save(new TrainerAssigned(trainerId1, 3, new DateTime(2017, 12, 21), 2));
            eventStore.Save(new TrainerAssigned(trainerId2, 4, new DateTime(2017, 10, 21), 2));
            eventStore.Save(new TrainerAssigned(trainerId2, 5, new DateTime(2017, 11, 21), 2));
            eventStore.Save(new TrainerAssigned(trainerId2, 6, new DateTime(2017, 12, 21), 2));
            eventStore.Save(new TrainingCreated(trainingId, 1, "Formation de test",1, Color.Empty.ToArgb()));
            var eventBus = new EventBus(dispatcher, eventStore);

            var sessionQueries = new FakeSessionQueries();
            sessionQueries.AddSession(trainingId, sessionId1, new DateTime(2017, 10, 21), 2,null, trainerId1);
            sessionQueries.AddSession(trainingId, sessionId2, new DateTime(2017, 11, 21), 2,null, trainerId1);
            sessionQueries.AddSession(trainingId, sessionId3, new DateTime(2017, 12, 21), 2,null, trainerId1);
            sessionQueries.AddSession(trainingId, sessionId4, new DateTime(2017, 10, 21), 2,null, trainerId2);
            sessionQueries.AddSession(trainingId, sessionId5, new DateTime(2017, 11, 21), 2,null, trainerId2);
            sessionQueries.AddSession(trainingId, sessionId6, new DateTime(2017, 12, 21), 2,null, trainerId2);

            // when
            new DeleteTraining(eventBus, sessionQueries).Execute(trainingId);
            
            // then
            mockHandler.AllEvents.OfType<SessionDeleted>().Should().HaveCount(6);
            mockHandler.AllEvents.OfType<TrainerUnassigned>().Should().HaveCount(6);
            mockHandler.AllEvents.OfType<TrainingDeleted>().Should().HaveCount(1);
        }

        //----------
        [TestMethod]
        public void plan_new_session_with_no_location()
        {
            var trainingId = Guid.NewGuid();
            var trainerId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();
            var eventStore = new FakeEventStore();
            eventStore.Save(new TrainerCreated(trainerId, 1, "BOUDOUX", "Aurelien", "test@test.com"));
            var eventBus = new EventBus(dispatcher, eventStore);

            var mockHandler = new MockHandler<SessionPlanned>();
            dispatcher.Register(mockHandler);

            new PlanSession(eventBus).Execute(trainingId, new DateTime(2018, 1, 1), 5, 10, null, trainerId);

            mockHandler.AllEvents.Should().HaveCount(1);
            mockHandler.AllEvents.Should().Contain(new SessionPlanned(Guid.Empty, 0, trainingId, new DateTime(2018, 1, 1), 5, 10, null, trainerId));
        }

        [TestMethod]
        public void throw_error_if_plan_session_with_location_that_not_exists()
        {
            var trainingId = Guid.NewGuid();
            var trainerId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();
            var eventStore = new FakeEventStore();
            eventStore.Save(new TrainerCreated(trainerId, 1, "BOUDOUX", "Aurelien", "test@test.com"));
            var eventBus = new EventBus(dispatcher, eventStore);

            Action action = () => new PlanSession(eventBus).Execute(trainingId, new DateTime(2017, 12, 21), 5, 10, Guid.NewGuid(), trainerId);
            action.ShouldThrow<LocationNotExistsException>();
        }

        [TestMethod]
        public void throw_error_if_location_not_available_on_planNewSession()
        {
            // given
            var trainingId = Guid.NewGuid();
            var trainerId = Guid.NewGuid();
            var locationId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();
            var eventStore = new FakeEventStore();
            eventStore.Save(new TrainerCreated(trainerId, 1, "BOUDOUX", "Aurelien", "test@test.com"));
            eventStore.Save(new LocationCreated(locationId, 1, "Paris", "test", 5));
            eventStore.Save(new LocationAssigned(locationId, 2, new DateTime(2017, 12, 20), 10));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            Action action = () => new PlanSession(eventBus).Execute(trainingId, new DateTime(2017, 12, 21), 5, 1, locationId, trainerId);

            // then
            action.ShouldThrow<LocationAlreadyAssignedException>();
        }

        [TestMethod]
        public void reassigne_location_when_update_session_with_source_and_destination()
        {
            // given
            var trainingId = Guid.NewGuid();

            var locationId1 = Guid.NewGuid();
            var locationId2 = Guid.NewGuid();
            var trainerId = Guid.NewGuid();

            var sessionId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();
            var mockHandler = new MockHandler<LocationAssigned, LocationUnassigned, SessionUpdated>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();
            eventStore.Save(new LocationCreated(locationId1, 1, "Lyon", "test", 5));
            eventStore.Save(new LocationCreated(locationId2, 1, "Paris", "test", 5));
            eventStore.Save(new SessionPlanned(sessionId, 1, trainingId, new DateTime(2018, 1, 1), 5, 10, locationId1, trainerId));
            eventStore.Save(new LocationAssigned(locationId1, 2, new DateTime(2018, 1, 1), 5));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            new UpdateSession(eventBus).Execute(sessionId, trainingId,new DateTime(2018, 1, 1), 5, 10, locationId2, trainerId);

            // then
            mockHandler.AllEvents.Should()                
                .Contain(new LocationUnassigned(Guid.Empty, 0, new DateTime(2018, 1, 1), 5)).And
                .Contain(new LocationAssigned(Guid.Empty, 0, new DateTime(2018, 1, 1), 5)).And
                .Contain(new SessionUpdated(Guid.Empty, 0, new DateTime(2018, 1, 1), 5, 10, locationId2, trainerId, trainingId));
        }

        [TestMethod]
        public void reassigne_lieux_when_update_session_without_source_and_withdestination()
        {
            // given
            var trainingId = Guid.NewGuid();

            var locationId1 = Guid.NewGuid();
            var locationId2 = Guid.NewGuid();
            var trainerId = Guid.NewGuid();

            var sessionId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();
            var mockHandler = new MockHandler<LocationAssigned, LocationUnassigned, SessionUpdated>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();
            eventStore.Save(new LocationCreated(locationId1, 1, "Lyon", "test", 5));
            eventStore.Save(new LocationCreated(locationId2, 1, "Paris", "test", 5));
            eventStore.Save(new SessionPlanned(sessionId, 1, trainingId, new DateTime(2018, 1, 1), 5, 10, null, trainerId));
            eventStore.Save(new LocationAssigned(locationId1, 2, new DateTime(2018, 1, 1), 5));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            new UpdateSession(eventBus).Execute(sessionId, trainingId,new DateTime(2018, 1, 1), 5, 10, locationId2, trainerId);

            // then
            mockHandler.AllEvents.Should()
                .Contain(new LocationAssigned(Guid.Empty, 0, new DateTime(2018, 1, 1), 5)).And
                .Contain(new SessionUpdated(Guid.Empty, 0, new DateTime(2018, 1, 1), 5, 10, locationId2, trainerId, trainingId));
        }

        [TestMethod]
        public void reassigne_location_when_update_session_with_source_and_withoutdestination()
        {
            // given
            var trainingId = Guid.NewGuid();

            var locationId1 = Guid.NewGuid();
            var locationId2 = Guid.NewGuid();
            var trainerId = Guid.NewGuid();

            var sessionId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();
            var mockHandler = new MockHandler<LocationAssigned, LocationUnassigned, SessionUpdated>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();
            eventStore.Save(new LocationCreated(locationId1, 1, "Lyon", "test", 5));
            eventStore.Save(new LocationCreated(locationId2, 1, "Paris", "test", 5));
            eventStore.Save(new SessionPlanned(sessionId, 1, trainingId, new DateTime(2018, 1, 1), 5, 10, locationId1, trainerId));
            eventStore.Save(new LocationAssigned(locationId1, 2, new DateTime(2018, 1, 1), 5));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            new UpdateSession(eventBus).Execute(sessionId, trainingId,new DateTime(2018, 1, 1), 5, 10, null, trainerId);

            // then
            mockHandler.AllEvents.Should()
                .Contain(new LocationUnassigned(Guid.Empty, 0, new DateTime(2018, 1, 1), 5)).And
                .Contain(new SessionUpdated(Guid.Empty, 0, new DateTime(2018, 1, 1), 5, 10, null, trainerId, trainingId));
        }

        [TestMethod]
        public void dont_reassign_location_when_update_session_with_same_source_and_destination()
        {
            // given
            var trainingId = Guid.NewGuid();

            var locationId1 = Guid.NewGuid();
            var locationId2 = Guid.NewGuid();
            var formateurId = Guid.NewGuid();

            var sessionId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();
            var mockHandler = new MockHandler<LocationAssigned, LocationUnassigned, SessionUpdated>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();
            eventStore.Save(new LocationCreated(locationId1, 1, "Lyon", "test", 5));
            eventStore.Save(new LocationCreated(locationId2, 1, "Paris", "test", 5));
            eventStore.Save(new SessionPlanned(sessionId, 1, trainingId, new DateTime(2018, 1, 1), 5, 10, locationId1, formateurId));
            eventStore.Save(new LocationAssigned(locationId1, 2, new DateTime(2018, 1, 1), 5));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            new UpdateSession(eventBus).Execute(sessionId, trainingId, new DateTime(2018, 1, 1), 5, 10, locationId1, formateurId);

            // then
            mockHandler.AllEvents.Should()
                .Contain(new SessionUpdated(Guid.Empty, 0, new DateTime(2018, 1, 1), 5, 10, locationId1, formateurId, trainingId));          
        }

        [TestMethod]
        public void throw_error_if_update_location_that_is_not_available()
        {
            // given
            var trainingId = Guid.NewGuid();

            var locationId1 = Guid.NewGuid();
            var locationId2 = Guid.NewGuid();

            var trainerId = Guid.NewGuid();

            var sessionId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();

            var eventStore = new FakeEventStore();
            eventStore.Save(new LocationCreated(locationId1, 1, "Lyon", "test", 5));
            eventStore.Save(new LocationCreated(locationId2, 1, "Paris", "test", 5));
            eventStore.Save(new SessionPlanned(sessionId, 1, trainingId, new DateTime(2018, 1, 1), 5, 10, locationId1, trainerId));
            eventStore.Save(new LocationAssigned(locationId1, 2, new DateTime(2017, 12, 21), 5));
            eventStore.Save(new LocationAssigned(locationId2, 2, new DateTime(2017, 12, 20), 10));
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            Action action = () => new UpdateSession(eventBus).Execute(sessionId, trainingId, new DateTime(2017, 12, 21), 5, 10, locationId2, trainerId);

            action.ShouldThrow<LocationAlreadyAssignedException>();
        }

        [TestMethod]
        public void unassign_location_when_session_deleted()
        {
            // given
            var trainingId = Guid.NewGuid();
            var trainerId1 = Guid.NewGuid();
            var locationId = Guid.NewGuid();

            var sessionId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();
            var mockHandler = new MockHandler<TrainerUnassigned, LocationUnassigned, SessionDeleted>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();
            eventStore.Save(new TrainerCreated(trainerId1, 1, "BOUDOUX", "Aurelien", "test@test.com"));
            eventStore.Save(new TrainerAssigned(trainerId1, 2, new DateTime(2017, 12, 21), 5));

            eventStore.Save(new LocationCreated(locationId, 1, "Lyon", "test", 5));
            eventStore.Save(new LocationAssigned(locationId, 2, new DateTime(2017, 12, 21), 5));

            eventStore.Save(new SessionPlanned(sessionId, 1, trainingId, new DateTime(2017, 12, 21), 5, 10, locationId, trainerId1));
            
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
        public void reassign_location_if_update_session_period()
        {
            // given
            var trainingId = Guid.NewGuid();
            var locationId = Guid.NewGuid();

            var sessionId = Guid.NewGuid();
            var dispatcher = new EventDispatcher();
            var mockHandler = new MockHandler<LocationReassigned, SessionUpdated>();
            dispatcher.Register(mockHandler);

            var eventStore = new FakeEventStore();

            eventStore.Save(new LocationCreated(locationId, 1, "Lyon", "test", 5));
            eventStore.Save(new LocationAssigned(locationId, 2, new DateTime(2018, 1, 1), 5));

            eventStore.Save(new SessionPlanned(sessionId, 1, trainingId, new DateTime(2018, 1, 1), 5, 10, locationId, null));
            
            var eventBus = new EventBus(dispatcher, eventStore);

            // when
            new UpdateSession(eventBus).Execute(sessionId, trainingId, new DateTime(2018, 1, 2), 4, 10, locationId, null);

            // then
            mockHandler.AllEvents.Should()
                .Contain(new LocationReassigned(Guid.Empty, 0, new DateTime(2018, 1, 1), 5, new DateTime(2018, 1, 2), 4)).And
                .Contain(new SessionUpdated(Guid.Empty, 0, new DateTime(2018, 1, 2), 4, 10, locationId, null, trainingId));
        }

        [TestMethod]
        public void unassign_all_loction_when_formation_is_deleted()
        {
            // given
            var trainingId = Guid.NewGuid();

            var locationId1 = Guid.NewGuid();
            var locationId2 = Guid.NewGuid();

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
            eventStore.Save(new LocationCreated(locationId1, 1, "Paris", "test", 5));
            eventStore.Save(new LocationCreated(locationId2, 1, "Lyon", "test", 3));
            eventStore.Save(new SessionPlanned(sessionId1, 1, trainingId, new DateTime(2017, 10, 21), 2, 2, locationId1, null));
            eventStore.Save(new SessionPlanned(sessionId2, 2, trainingId, new DateTime(2017, 11, 21), 2, 2, locationId1, null));
            eventStore.Save(new SessionPlanned(sessionId3, 3, trainingId, new DateTime(2017, 12, 21), 2, 2, locationId1, null));
            eventStore.Save(new SessionPlanned(sessionId4, 1, trainingId, new DateTime(2017, 10, 21), 2, 2, locationId2, null));
            eventStore.Save(new SessionPlanned(sessionId5, 2, trainingId, new DateTime(2017, 11, 21), 2, 2, locationId2, null));
            eventStore.Save(new SessionPlanned(sessionId6, 3, trainingId, new DateTime(2017, 12, 21), 2, 2, locationId2, null));
            eventStore.Save(new LocationAssigned(locationId1, 1, new DateTime(2017, 10, 21), 2));
            eventStore.Save(new LocationAssigned(locationId1, 2, new DateTime(2017, 11, 21), 2));
            eventStore.Save(new LocationAssigned(locationId1, 3, new DateTime(2017, 12, 21), 2));
            eventStore.Save(new LocationAssigned(locationId2, 4, new DateTime(2017, 10, 21), 2));
            eventStore.Save(new LocationAssigned(locationId2, 5, new DateTime(2017, 11, 21), 2));
            eventStore.Save(new LocationAssigned(locationId2, 6, new DateTime(2017, 12, 21), 2));
            eventStore.Save(new TrainingCreated(trainingId, 1, "Formation de test", 1, Color.Empty.ToArgb()));
            var eventBus = new EventBus(dispatcher, eventStore);

            var sessionQueries = new FakeSessionQueries();
            sessionQueries.AddSession(trainingId, sessionId1, new DateTime(2017, 10, 21), 2, locationId1, null);
            sessionQueries.AddSession(trainingId, sessionId2, new DateTime(2017, 11, 21), 2, locationId1, null);
            sessionQueries.AddSession(trainingId, sessionId3, new DateTime(2017, 12, 21), 2, locationId1, null);
            sessionQueries.AddSession(trainingId, sessionId4, new DateTime(2017, 10, 21), 2, locationId2, null);
            sessionQueries.AddSession(trainingId, sessionId5, new DateTime(2017, 11, 21), 2, locationId2, null);
            sessionQueries.AddSession(trainingId, sessionId6, new DateTime(2017, 12, 21), 2, locationId2, null);

            // when
            new DeleteTraining(eventBus, sessionQueries).Execute(trainingId);

            // then
            mockHandler.AllEvents.OfType<SessionDeleted>().Should().HaveCount(6);
            mockHandler.AllEvents.OfType<LocationUnassigned>().Should().HaveCount(6);
            mockHandler.AllEvents.OfType<TrainingDeleted>().Should().HaveCount(1);
        }
        //---------

        [TestMethod]
         public void write_projection_with_training_id()
         {
             var dispatcher = new EventDispatcher();
             var projection = new SessionTestProjection();
             dispatcher.Register(projection);
             var eventBus = new EventBus(dispatcher, new FakeEventStore());

             var trainerId = Guid.NewGuid();

             var trainer = new CreateTrainer(eventBus, new FakeTrainerQueries()).Execute("BOUDOUX", "Aurelien", "test@test.com");
             var location = new CreateLocation(eventBus, new FakeLocationQueries()).Execute("Paris", "test", 5);

             new PlanSession(eventBus).Execute(trainerId, new DateTime(2017, 12, 20), 3, 3, location.AggregateId, trainer.AggregateId);
             projection.Planned.TrainingId.Should().Be(trainerId);
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
