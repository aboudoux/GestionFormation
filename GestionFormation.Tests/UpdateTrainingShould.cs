using System;
using System.Drawing;
using FluentAssertions;
using GestionFormation.Applications.Trainings;
using GestionFormation.CoreDomain.Trainings.Exceptions;
using GestionFormation.Kernel;
using GestionFormation.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GestionFormation.Tests
{
    [TestClass]
    [TestCategory("UnitTests")]
    public class UpdateTrainingShould
    {
        [TestMethod]        
        public void throw_error_if_training_name_already_exists()
        {
            var fakeQuery = new FakeTrainingQueries();
            var idTest1 = Guid.NewGuid();
            fakeQuery.AddTraining(idTest1, "TEST1", 1);
            fakeQuery.AddTraining("TEST2", 1);
            
            var update = new UpdateTraining(new EventBus(new EventDispatcher(), new FakeEventStore()), fakeQuery );
            Action action = () => update.Execute(idTest1, "TEST2",1, Color.Empty.ToArgb());
            action.ShouldThrow<TrainingAlreadyExistsException>();
        }
    }
}