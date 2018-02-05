using System;
using System.Linq;
using FluentAssertions;
using GestionFormation.Applications.Trainings;
using GestionFormation.CoreDomain.Trainings.Events;
using GestionFormation.CoreDomain.Trainings.Exceptions;
using GestionFormation.Kernel;
using GestionFormation.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GestionFormation.Tests
{
    [TestClass]
    [TestCategory("UnitTests")]
    public class CreateTrainingShould
    {
        [TestMethod]
        public void throw_error_if_formation_name_already_exists()
        {
            var fakeQuery = new FakeTrainingQueries();
            fakeQuery.AddFormation("TEST",1);
            var create = new CreateTraining(new EventBus(new EventDispatcher(), new FakeEventStore()), fakeQuery );

            Action action = () =>create.Execute("TEST",1);
            action.ShouldThrow<TrainingAlreadyExistsException>();
        }

        [TestMethod]
        public void return_proper_id_when_create_command_is_called()
        {
            var fakeQuery = new FakeTrainingQueries();            
            var create = new CreateTraining(new EventBus(new EventDispatcher(), new FakeEventStore()), fakeQuery);
            var formation = create.Execute("TEST",1);

            var firstEvent = formation.UncommitedEvents.GetStream().First();
            firstEvent.Should().BeAssignableTo<TrainingCreated>();
            firstEvent.AggregateId.Should().Be(formation.AggregateId);
        }
    }
}