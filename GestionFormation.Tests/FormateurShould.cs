using System;
using System.Collections.Specialized;
using System.Globalization;
using FluentAssertions;
using GestionFormation.Applications.Lieux.Exceptions;
using GestionFormation.CoreDomain.Trainers;
using GestionFormation.CoreDomain.Trainers.Events;
using GestionFormation.CoreDomain.Trainers.Exceptions;
using GestionFormation.Kernel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GestionFormation.Tests
{
    [TestClass]
    [TestCategory("UnitTests")]
    public class FormateurShould
    {
        [DataTestMethod]
        [DataRow("", "aurelien")]
        [DataRow("BOUDOUX", "")]
        [DataRow("BOUDOUX", null)]
        [DataRow(null, "aurelien")]
        [DataRow("", "")]
        [DataRow("", null)]
        [DataRow(null, "")]
        [DataRow(null, null)]
        public void throw_error_if_created_with_empty_name(string nom, string prenom)
        {
            Action action = () => Trainer.Create(nom, prenom, "test@test.com");
            action.ShouldThrow<TrainerNameException>();
        }

        [DataTestMethod]
        [DataRow("", "aurelien")]
        [DataRow("BOUDOUX", "")]
        [DataRow("BOUDOUX", null)]
        [DataRow(null, "aurelien")]
        [DataRow("", "")]
        [DataRow("", null)]
        [DataRow(null, "")]
        [DataRow(null, null)]
        public void throw_error_if_updated_with_empty_name(string nom, string prenom)
        {
            var history = new History();
            history.Add(new TrainerCreated(Guid.NewGuid(), 1, "BOUDOUX", "Aurelien", "test@test.com"));
            var formateur = new Trainer(history);

            Action action = () => formateur.Update(nom, prenom,"");
            action.ShouldThrow<TrainerNameException>();
        }

        [TestMethod]
        public void raise_formateurAssigned_when_formateur_is_assigned_to_a_session()
        {
            var history = new History();
            history.Add(new TrainerCreated(Guid.NewGuid(), 1, "BOUDOUX", "Aurelien", "test@test.com"));
            var formateur = new Trainer(history);

            formateur.Assign(new DateTime(2017,12,20),3);
            formateur.UncommitedEvents.GetStream().Should().Contain(new TrainerAssigned(Guid.Empty, 0, new DateTime(2017, 12, 20), 3));
        }

        [DataTestMethod]
        [DataRow("15/01/2017", 1)]
        [DataRow("16/01/2017", 1)]
        [DataRow("17/01/2017", 1)]
        [DataRow("10/01/2017", 10)]
        [DataRow("20/01/2017", 10)]
        public void throw_error_if_formateur_already_assigned_to_a_session(string startDate, int durée)
        {
            var history = new History();
            history.Add(new TrainerCreated(Guid.NewGuid(), 1, "BOUDOUX", "Aurelien", "test@test.com"));
            history.Add(new TrainerAssigned(Guid.NewGuid(), 2, new DateTime(2017,01,15), 10));
            var formateur = new Trainer(history);

            var start = DateTime.ParseExact(startDate, "dd/MM/yyyy", new DateTimeFormatInfo());
            Action action = () => formateur.Assign(start, durée);
            action.ShouldThrow<TrainerAlreadyAssignedException>();
        }

        [TestMethod]
        public void raise_assignationChanged_when_change_formateur_assignation()
        {
            var history = new History();
            history.Add(new TrainerCreated(Guid.NewGuid(), 1, "BOUDOUX", "Aurelien", "test@test.com"));
            history.Add(new TrainerAssigned(Guid.NewGuid(), 2, new DateTime(2017, 01, 15), 10));
            var formateur = new Trainer(history);
            formateur.ChangeAssignation(new DateTime(2017, 01, 15), 10, new DateTime(2017,01,10), 10);

            formateur.UncommitedEvents.GetStream().Should().Contain(new TrainerReassigned(Guid.Empty, 0, new DateTime(2017, 01, 15), 10, new DateTime(2017, 01, 10), 10));
        }

        [TestMethod]
        public void throw_error_if_trying_to_reassign_formateur_to_an_already_assigned_session()
        {
            var history = new History();
            history.Add(new TrainerCreated(Guid.NewGuid(), 1, "BOUDOUX", "Aurelien", "test@test.com"));
            history.Add(new TrainerAssigned(Guid.NewGuid(), 2, new DateTime(2017, 01, 15), 10));
            history.Add(new TrainerAssigned(Guid.NewGuid(), 2, new DateTime(2017, 02, 15), 10));
            var formateur = new Trainer(history);

            Action action = () => formateur.ChangeAssignation(new DateTime(2017, 01, 15), 10, new DateTime(2017, 02, 10), 10);
            action.ShouldThrow<TrainerAlreadyAssignedException>();
        }

        [TestMethod]
        public void throw_error_if_trying_to_update_assignation_that_not_exists()
        {
            var history = new History();
            history.Add(new TrainerCreated(Guid.NewGuid(), 1, "BOUDOUX", "Aurelien", "test@test.com"));
            var formateur = new Trainer(history);

            Action action = () => formateur.ChangeAssignation(new DateTime(2017, 01, 15), 10, new DateTime(2017, 02, 10), 10);
            action.ShouldThrow<PeriodDoNotExistsException>();
        }

        [TestMethod]
        public void raise_formationUnassigned_if_formateur_no_longer_assigned_to_a_session()
        {
            var history = new History();
            history.Add(new TrainerCreated(Guid.NewGuid(), 1, "BOUDOUX", "Aurelien", "test@test.com"));
            history.Add(new TrainerAssigned(Guid.NewGuid(), 2, new DateTime(2017, 01, 15), 10));
            history.Add(new TrainerAssigned(Guid.NewGuid(), 2, new DateTime(2017, 02, 15), 10));
            var formateur = new Trainer(history);
            formateur.UnAssign(new DateTime(2017, 02, 15), 10);
            formateur.UncommitedEvents.GetStream().Should().Contain(new TrainerUnassigned(Guid.Empty, 0, new DateTime(2017, 02, 15), 10));
        }

        [TestMethod]
        public void reasign_old_period_if_properly_removed()
        {
            var history = new History();
            history.Add(new TrainerCreated(Guid.NewGuid(), 1, "BOUDOUX", "Aurelien", "test@test.com"));
            var formateur = new Trainer(history);
            formateur.Assign(new DateTime(2017, 01, 15), 10);
            formateur.UnAssign(new DateTime(2017, 01, 15), 10);
            formateur.Assign(new DateTime(2017, 01, 13), 10);
        }

        [TestMethod]
        public void reasign_old_period_if_properly_removed_from_event_store()
        {
            var history = new History();
            history.Add(new TrainerCreated(Guid.NewGuid(), 1, "BOUDOUX", "Aurelien", "test@test.com"));
            history.Add(new TrainerAssigned(Guid.NewGuid(), 2, new DateTime(2017, 01, 15), 10));
            history.Add(new TrainerUnassigned(Guid.NewGuid(), 2, new DateTime(2017, 01, 15), 10));

            var formateur = new Trainer(history);
            formateur.Assign(new DateTime(2017, 01, 13), 10);

            formateur.UncommitedEvents.GetStream().Should().Contain(new TrainerAssigned(Guid.Empty, 1, new DateTime(2017, 01, 13), 10));

        }

        [DataTestMethod]
        [DataRow("10/01/2017", 3, true)]
        [DataRow("15/01/2017", 1, false)]
        [DataRow("10/01/2017", 10, false)]
        [DataRow("10/01/2017", 30, false)]
        [DataRow("18/01/2017", 2, false)]
        [DataRow("16/01/2017", 9, false)]
        [DataRow("20/01/2017", 10, false)]
        [DataRow("25/01/2017", 1, true)]
        [DataRow("23/01/2017", 15, false)]
        [DataRow("10/01/2017", 30, false)]
        [DataRow("24/01/2017", 1, false)]
        [DataRow("30/01/2017",5, true)]
        public void test_assigned_session(string startDate, int durée, bool expectedResult)
        {
            var start = DateTime.ParseExact(startDate, "dd/MM/yyyy", new DateTimeFormatInfo());
            var assignedSession = new AssignedSession();

            assignedSession.Add(new DateTime(2017, 01, 15), 10);
            assignedSession.IsFreeFor(start, durée).Should().Be(expectedResult);
        }

        [TestMethod]
        public void throw_error_if_trying_to_delete_assigned_formateur()
        {
            var history = new History();
            history.Add(new TrainerCreated(Guid.NewGuid(), 1, "BOUDOUX", "Aurelien", "test@test.com"));
            history.Add(new TrainerAssigned(Guid.NewGuid(), 2, new DateTime(2017, 01, 15), 10));
            var formateur = new Trainer(history);

            Action action = () => formateur.Delete();
            action.ShouldThrow<ForbiddenDeleteTrainerException>();
        }

        [TestMethod]
        public void raise_formateurdeleted_if_call_delete_and_formateur_not_assigned()
        {
            var history = new History();
            history.Add(new TrainerCreated(Guid.NewGuid(), 1, "BOUDOUX", "Aurelien", "test@test.com"));            
            var formateur = new Trainer(history);

            formateur.Delete();
            formateur.UncommitedEvents.GetStream().Should().Contain(new TrainerDeleted(Guid.Empty, 0));
        }

        [TestMethod]
        public void raise_formateurDisabled()
        {
            var history = new History();
            history.Add(new TrainerCreated(Guid.NewGuid(), 1, "BOUDOUX", "Aurelien", "test@test.com"));
            var formateur = new Trainer(history);
            formateur.Disable();

            formateur.UncommitedEvents.GetStream().Should().Contain(new TrainerDisabled(Guid.Empty, 0));
        }

        [TestMethod]
        public void dont_raise_formateurDisabled_if_already_disabled()
        {
            var history = new History();
            history.Add(new TrainerCreated(Guid.NewGuid(), 1, "BOUDOUX", "Aurelien", "test@test.com"));
            history.Add(new TrainerDisabled(Guid.NewGuid(), 2));
            var formateur = new Trainer(history);
            formateur.Disable();

            formateur.UncommitedEvents.GetStream().Should().BeEmpty();
        }

        [TestMethod]
        public void test_assign_and_unassign_with_complete_date()
        {
            var start = DateTime.Now;
            var assignedSession = new AssignedSession();

            assignedSession.Add(start, 10);
            assignedSession.PeriodExists(start.AddHours(1), 10).Should().BeTrue();
        }
    }
}