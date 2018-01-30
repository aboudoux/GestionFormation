using System;
using FluentAssertions;
using GestionFormation.Applications.Formations;
using GestionFormation.CoreDomain.Trainings.Exceptions;
using GestionFormation.Kernel;
using GestionFormation.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GestionFormation.Tests
{
    [TestClass]
    [TestCategory("UnitTests")]
    public class UpdateFormationShould
    {
        [TestMethod]        
        public void throw_error_if_formation_name_already_exists()
        {
            var fakeQuery = new FakeTrainingQueries();
            var idTest1 = Guid.NewGuid();
            fakeQuery.AddFormation(idTest1, "TEST1", 1);
            fakeQuery.AddFormation("TEST2", 1);
            
            var update = new UpdateFormation(new EventBus(new EventDispatcher(), new FakeEventStore()), fakeQuery );
            Action action = () => update.Execute(idTest1, "TEST2",1);
            action.ShouldThrow<TrainingAlreadyExistsException>();
        }
    }
}