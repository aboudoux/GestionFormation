using System;
using System.Globalization;
using FluentAssertions;
using GestionFormation.Applications.Locations;
using GestionFormation.Applications.Locations.Exceptions;
using GestionFormation.CoreDomain.Locations;
using GestionFormation.CoreDomain.Locations.Events;
using GestionFormation.CoreDomain.Locations.Exceptions;
using GestionFormation.CoreDomain.Trainers.Exceptions;
using GestionFormation.Kernel;
using GestionFormation.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GestionFormation.Tests
{
    [TestClass]
    [TestCategory("UnitTests")]
    public class LocationShould
    {
        [TestMethod]
        public void raise_LocationCreated_on_create_new_location()
        {
            var location = Location.Create("Saint Priest", "allée du toscan", 1);
            location.UncommitedEvents.GetStream().Should().Contain(new LocationCreated(Guid.Empty, 0, "Saint Priest", "allée du toscan", 1));
        }

        [TestMethod]
        public void raise_LocationUpdated_on_update_location()
        {
            var history = new History();
            history.Add(new LocationCreated(Guid.Empty, 1, "Saint Priest", "yolo",1));

            var location = new Location(history);
            location.Update("Saint Priest", "tata",2);
            location.UncommitedEvents.GetStream().Should().Contain(new LocationUpdated(Guid.Empty, 0, "Saint Priest", "tata",2));
        }

        [TestMethod]
        public void raise_LocationDeleted_on_delete_location()
        {
            var history = new History();
            history.Add(new LocationCreated(Guid.Empty, 1, "Saint Priest", "yolo",1));

            var location = new Location(history);
            location.Delete();
            location.UncommitedEvents.GetStream().Should().Contain(new LocationDeleted(Guid.Empty, 0));
        }

        [TestMethod]
        public void dont_raise_LocationDeleted_if_location_already_deleted()
        {
            var history = new History();
            history.Add(new LocationCreated(Guid.Empty, 1, "Saint Priest", "yolo",1));
            history.Add(new LocationDeleted(Guid.Empty, 2));

            var location = new Location(history);
            location.Delete();
            location.UncommitedEvents.GetStream().Should().BeEmpty();
        }

        [TestMethod]
        public void dont_raise_multiple_update_if_last_update_is_same_data()
        {
            var history = new History();
            history.Add(new LocationCreated(Guid.Empty, 1, "Saint Priest", "yolo",1));
            history.Add(new LocationUpdated(Guid.Empty, 2, "Saint Priest", "test",1));

            var location = new Location(history);
            location.Update("Saint Priest", "test",1);
            location.UncommitedEvents.GetStream().Should().BeEmpty();
        }

        [TestMethod]        
        public void dont_raise_multiple_update_if_multiple_update_call_with_same_data()
        {
            var history = new History();
            history.Add(new LocationCreated(Guid.Empty, 1, "Saint Priest", "yolo",1));            

            var location = new Location(history);
            location.Update("Saint Priest", "TEST",1);
            location.Update("Saint Priest", "TEST",1);
            location.UncommitedEvents.GetStream().Should().HaveCount(1);
        }

        [TestMethod]
        public void raise_multiple_update_if_data_change()
        {
            var history = new History();
            history.Add(new LocationCreated(Guid.Empty, 1, "Saint Priest", "yolo",1));

            var location = new Location(history);
            location.Update("Saint Priest", "TEST",1);
            location.Update("Saint Priest", "TEST2",1);
            location.Update("Saint Priest", "TEST3",1);
            location.UncommitedEvents.GetStream().Should().HaveCount(3);
        }

        [TestMethod]
        public void throw_error_if_creating_location_with_same_name()
        {
            var queries = new FakeLocationQueries();
            queries.Add("lulu", "test",1);
            queries.Add("lala", "test",1);

            Action action = () => new CreateLocation(new EventBus(new EventDispatcher(), new FakeEventStore()), queries).Execute("lala", "ok", 1);
            action.ShouldThrow<LocationAlreadyExistsException>();
        }
        
        [TestMethod]
        public void throw_error_if_updating_name_whith_same_name()
        {
            var queries = new FakeLocationQueries();
            queries.Add("lulu", "test",1);
            queries.Add("lala", "test",1);
            queries.Add("titi", "test",1);

            var eventStore = new FakeEventStore();
            var lieuId = Guid.NewGuid();
            eventStore.Save(new LocationCreated(lieuId,1, "COUCOU", "osef",1));
            
            Action action = () => new UpdateLocation(new EventBus(new EventDispatcher(), eventStore), queries).Execute(lieuId,"lala", "ok", 1);
            action.ShouldThrow<LocationAlreadyExistsException>();
        }

        [TestMethod]
        public void dont_throw_error_if_updating_myself()
        {
            var queries = new FakeLocationQueries();
            queries.Add("lulu", "test", 1);
            queries.Add("lala", "test", 1);
            queries.Add("titi", "test", 1);

            var eventStore = new FakeEventStore();
            var lieuId = Guid.NewGuid();
            eventStore.Save(new LocationCreated(lieuId, 1, "COUCOU", "osef", 1));

            new UpdateLocation(new EventBus(new EventDispatcher(), eventStore), queries).Execute(lieuId, "COUCOU", "ok", 2);

            eventStore.GetEvents(lieuId).Should().Contain(new LocationUpdated(lieuId, 0, "COUCOU", "ok", 2));            
        }

        [TestMethod]
        public void throw_error_if_create_with_empty_name()
        {
            Action action = () => Location.Create("", "",1);
            action.ShouldThrow<LocationWithEmptyNameException>();
        }

        [TestMethod]
        public void throw_error_if_update_with_empty_name()
        {
            var history = new History();
            history.Add(new LocationCreated(Guid.Empty, 1, "Saint Priest", "yolo",1));

            var location = new Location(history);
            Action action =  () => location.Update(null, "TEST",1);
            action.ShouldThrow<LocationWithEmptyNameException>();
        }

        [TestMethod]
        public void raise_locationAssigned_when_location_is_assigned_to_a_new_session()
        {
            var history = new History();
            history.Add(new LocationCreated(Guid.NewGuid(), 1, "Lyon", "test", 5));
            var location = new Location(history);

            location.Assign(new DateTime(2017, 12, 20), 3);
            location.UncommitedEvents.GetStream().Should().Contain(new LocationAssigned(Guid.Empty, 0, new DateTime(2017, 12, 20), 3));
        }

        [TestMethod]
        [DataTestMethod]
        [DataRow("15/01/2017", 1)]
        [DataRow("16/01/2017", 1)]
        [DataRow("17/01/2017", 1)]
        [DataRow("10/01/2017", 10)]
        [DataRow("20/01/2017", 10)]
        public void throw_error_if_location_already_assigned_to_a_session(string startDate, int durée)
        {
            var history = new History();
            history.Add(new LocationCreated(Guid.NewGuid(), 1, "Lyon", "test", 5));
            history.Add(new LocationAssigned(Guid.NewGuid(), 2 , new DateTime(2017, 01, 15), 10));
            var location = new Location(history);

            var start = DateTime.ParseExact(startDate, "dd/MM/yyyy", new DateTimeFormatInfo());
            Action action = () => location.Assign(start, durée);
            action.ShouldThrow<LocationAlreadyAssignedException>();
        }

        [TestMethod]
        public void raise_locationReasigned_when_change_location_assignation()
        {
            var history = new History();
            history.Add(new LocationCreated(Guid.NewGuid(), 1, "Lyon", "test", 5));
            history.Add(new LocationAssigned(Guid.NewGuid(), 2, new DateTime(2017, 01, 15), 10));
            var location = new Location(history);

            location.ChangeAssignation(new DateTime(2017, 01, 15), 10, new DateTime(2017, 01, 10), 10);

            location.UncommitedEvents.GetStream().Should().Contain(new LocationReassigned(Guid.Empty, 0, new DateTime(2017, 01, 15), 10, new DateTime(2017, 01, 10), 10));
        }

        [TestMethod]
        public void throw_error_if_trying_to_reassign_location_to_an_already_assigned_session()
        {
            var history = new History();
            history.Add(new LocationCreated(Guid.NewGuid(), 1, "Lyon", "test", 5));
            history.Add(new LocationAssigned(Guid.NewGuid(), 2, new DateTime(2017, 01, 15), 10));
            history.Add(new LocationAssigned(Guid.NewGuid(), 3, new DateTime(2017, 02, 15), 10));
            var location = new Location(history);

            Action action = () => location.ChangeAssignation(new DateTime(2017, 01, 15), 10, new DateTime(2017, 02, 10), 10);
            action.ShouldThrow<LocationAlreadyAssignedException>();
        }

        [TestMethod]
        public void throw_error_if_trying_to_update_assignation_that_not_exists()
        {
            var history = new History();
            history.Add(new LocationCreated(Guid.NewGuid(), 1, "Lyon", "test", 5));
            var location = new Location(history);

            Action action = () => location.ChangeAssignation(new DateTime(2017, 01, 15), 10, new DateTime(2017, 02, 10), 10);
            action.ShouldThrow<PeriodDoNotExistsException>();
        }

        [TestMethod]
        public void raise_locationUnassigned_if_lieu_no_longer_assigned_to_a_session()
        {
            var history = new History();
            history.Add(new LocationCreated(Guid.NewGuid(), 1, "Lyon", "test", 5));
            history.Add(new LocationAssigned(Guid.NewGuid(), 2, new DateTime(2017, 01, 15), 10));
            history.Add(new LocationAssigned(Guid.NewGuid(), 3, new DateTime(2017, 02, 15), 10));
            var location = new Location(history);

            location.UnAssign(new DateTime(2017, 02, 15), 10);
            location.UncommitedEvents.GetStream().Should().Contain(new LocationUnassigned(Guid.Empty, 0, new DateTime(2017, 02, 15), 10));
        }

        [TestMethod]
        public void reasign_old_period_if_properly_removed()
        {
            var history = new History();
            history.Add(new LocationCreated(Guid.NewGuid(), 1, "Paris", "test", 5));
            var location = new Location(history);
            location.Assign(new DateTime(2017, 01, 15), 10);
            location.UnAssign(new DateTime(2017, 01, 15), 10);
            location.Assign(new DateTime(2017, 01, 13), 10);
        }

        [TestMethod]
        public void reasign_old_period_if_properly_removed_from_event_store()
        {
            var history = new History();
            history.Add(new LocationCreated(Guid.NewGuid(), 1, "Paris", "test", 5));
            history.Add(new LocationAssigned(Guid.NewGuid(), 2, new DateTime(2017, 01, 15), 10));
            history.Add(new LocationUnassigned(Guid.NewGuid(), 2, new DateTime(2017, 01, 15), 10));

            var location = new Location(history);
            location.Assign(new DateTime(2017, 01, 13), 10);

            location.UncommitedEvents.GetStream().Should().Contain(new LocationAssigned(Guid.Empty, 1, new DateTime(2017, 01, 13), 10));
        }

        [TestMethod]
        public void throw_error_if_trying_to_delete_assigned_location()
        {
            var history = new History();
            history.Add(new LocationCreated(Guid.NewGuid(), 1, "Paris", "test", 5));
            history.Add(new LocationAssigned(Guid.NewGuid(), 2, new DateTime(2017, 01, 15), 10));
            var location = new Location(history);

            Action action = () => location.Delete();
            action.ShouldThrow<ForbiddenDeleteLocationException>();
        }

        [TestMethod]
        public void raise_locationdeleted_if_call_delete_and_location_not_assigned()
        {
            var history = new History();
            history.Add(new LocationCreated(Guid.NewGuid(), 1, "Paris", "test", 5));
            var location = new Location(history);

            location.Delete();
            location.UncommitedEvents.GetStream().Should().Contain(new LocationDeleted(Guid.Empty, 0));
        }
    }
}