using System;
using FluentAssertions;
using GestionFormation.CoreDomain.Formations;
using GestionFormation.CoreDomain.Formations.Events;
using GestionFormation.CoreDomain.Formations.Exceptions;
using GestionFormation.Kernel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GestionFormation.Tests
{
    [TestClass]
    [TestCategory("UnitTests")]
    public class FormationShould
    {
        [TestMethod]
        public void raise_formationCreated_on_create_new_formation()
        {
            var formation = Formation.Create("TED",1);
            formation.UncommitedEvents.GetStream().Should().Contain(new FormationCreated(Guid.Empty, 1, "TED",1));
        }

        [TestMethod]
        public void raise_formationUpdated_on_update_formation()
        {
            var history = new History();
            history.Add(new FormationCreated(Guid.NewGuid(), 1, "TEST",2));

            var formation = new Formation(history);
            formation.Update("ESSAI",3);
            formation.UncommitedEvents.GetStream().Should().Contain(new FormationUpdated(Guid.NewGuid(), 1, "ESSAI",3));
        }

        [TestMethod]
        public void dont_raise_update_if_last_update_equal()
        {
            var history = new History();
            var formationId = Guid.NewGuid();
            history.Add(new FormationCreated(formationId, 1, "TED",1));
            history.Add(new FormationUpdated(formationId, 2, "WELL",1));

            var formation = new Formation(history);
            formation.Update("WELL",1);

            formation.UncommitedEvents.GetStream().Should().BeEmpty();
        }

        [TestMethod]
        public void raise_forationDeleted_on_delete_formation()
        {
            var history = new History();
            var formationId = Guid.NewGuid();
            history.Add(new FormationCreated(formationId, 1, "TED",1));

            var formation = new Formation(history);
            formation.Delete();

            formation.UncommitedEvents.GetStream().Should().Contain(new FormationDeleted(Guid.Empty, 0));
        }

        [TestMethod]
        public void dont_raise_formationDeleted_if_formation_already_deleted()
        {
            var history = new History();
            var formationId = Guid.NewGuid();
            history.Add(new FormationCreated(formationId, 1, "TED",1));
            history.Add(new FormationDeleted(formationId, 2));

            var formation = new Formation(history);
            formation.Delete();

            formation.UncommitedEvents.GetStream().Should().BeEmpty();
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void throw_error_if_formation_name_null_or_empty(string formationName)
        {
            Action action = () => Formation.Create(formationName,1);
            action.ShouldThrow<FormationEmptyNameException>();
        }        
    }
}