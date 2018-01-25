using System;
using System.Linq;
using FluentAssertions;
using GestionFormation.Applications.Formations;
using GestionFormation.CoreDomain.Formations.Events;
using GestionFormation.CoreDomain.Formations.Exceptions;
using GestionFormation.Kernel;
using GestionFormation.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GestionFormation.Tests
{
    [TestClass]
    [TestCategory("UnitTests")]
    public class CreateFormationShould
    {
        [TestMethod]
        public void throw_error_if_formation_name_already_exists()
        {
            var fakeQuery = new FakeFormationQueries();
            fakeQuery.AddFormation("TEST",1);
            var create = new CreateFormation(new EventBus(new EventDispatcher(), new FakeEventStore()), fakeQuery );

            Action action = () =>create.Execute("TEST",1);
            action.ShouldThrow<FormationAlreadyExistsException>();
        }

        [TestMethod]
        public void return_proper_id_when_create_command_is_called()
        {
            var fakeQuery = new FakeFormationQueries();            
            var create = new CreateFormation(new EventBus(new EventDispatcher(), new FakeEventStore()), fakeQuery);
            var formation = create.Execute("TEST",1);

            var firstEvent = formation.UncommitedEvents.GetStream().First();
            firstEvent.Should().BeAssignableTo<FormationCreated>();
            firstEvent.AggregateId.Should().Be(formation.AggregateId);
        }
    }
}