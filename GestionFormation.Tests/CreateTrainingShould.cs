using System;
using System.Drawing;
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
        public void throw_error_if_training_name_already_exists()
        {
            var fakeQuery = new FakeTrainingQueries();
            fakeQuery.AddTraining("TEST",1);
            var create = new CreateTraining(new EventBus(new EventDispatcher(), new FakeEventStore()), fakeQuery );

            Action action = () =>create.Execute("TEST",1, Color.Empty.ToArgb());
            action.ShouldThrow<TrainingAlreadyExistsException>();
        }

        [TestMethod]
        public void return_proper_id_when_create_command_is_called()
        {
            var fakeQuery = new FakeTrainingQueries();            
            var create = new CreateTraining(new EventBus(new EventDispatcher(), new FakeEventStore()), fakeQuery);
            var training = create.Execute("TEST",1, Color.Empty.ToArgb());

            var firstEvent = training.UncommitedEvents.GetStream().First();
            firstEvent.Should().BeAssignableTo<TrainingCreated>();
            firstEvent.AggregateId.Should().Be(training.AggregateId);
        }
    }
}