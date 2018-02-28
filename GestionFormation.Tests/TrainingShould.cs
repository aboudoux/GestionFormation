using System;
using System.Drawing;
using FluentAssertions;
using GestionFormation.CoreDomain.Trainings;
using GestionFormation.CoreDomain.Trainings.Events;
using GestionFormation.CoreDomain.Trainings.Exceptions;
using GestionFormation.Kernel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GestionFormation.Tests
{
    [TestClass]
    [TestCategory("UnitTests")]
    public class TrainingShould
    {
        [TestMethod]
        public void raise_trainingCreated_on_create_new_training()
        {
            var training = Training.Create("TED",1, Color.Empty.ToArgb());
            training.UncommitedEvents.GetStream().Should().Contain(new TrainingCreated(Guid.Empty, 1, "TED",1, Color.Empty.ToArgb()));
        }

        [TestMethod]
        public void raise_trainingUpdated_on_update_training()
        {
            var history = new History();
            history.Add(new TrainingCreated(Guid.NewGuid(), 1, "TEST",2, Color.Empty.ToArgb()));

            var training = new Training(history);
            training.Update("ESSAI",3, Color.Empty.ToArgb());
            training.UncommitedEvents.GetStream().Should().Contain(new TrainingUpdated(Guid.NewGuid(), 1, "ESSAI",3, Color.Empty.ToArgb()));
        }

        [TestMethod]
        public void dont_raise_update_if_last_update_equal()
        {
            var history = new History();
            var trainingId = Guid.NewGuid();
            history.Add(new TrainingCreated(trainingId, 1, "TED",1, Color.Empty.ToArgb()));
            history.Add(new TrainingUpdated(trainingId, 2, "WELL",1, Color.Empty.ToArgb()));

            var training = new Training(history);
            training.Update("WELL",1, Color.Empty.ToArgb());

            training.UncommitedEvents.GetStream().Should().BeEmpty();
        }

        [TestMethod]
        public void raise_trainingDeleted_on_delete_training()
        {
            var history = new History();
            var trainingId = Guid.NewGuid();
            history.Add(new TrainingCreated(trainingId, 1, "TED",1, Color.Empty.ToArgb()));

            var training = new Training(history);
            training.Delete();

            training.UncommitedEvents.GetStream().Should().Contain(new TrainingDeleted(Guid.Empty, 0));
        }

        [TestMethod]
        public void dont_raise_trainingDeleted_if_training_already_deleted()
        {
            var history = new History();
            var trainingId = Guid.NewGuid();
            history.Add(new TrainingCreated(trainingId, 1, "TED",1, Color.Empty.ToArgb()));
            history.Add(new TrainingDeleted(trainingId, 2));

            var training = new Training(history);
            training.Delete();

            training.UncommitedEvents.GetStream().Should().BeEmpty();
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void throw_error_if_training_name_null_or_empty(string trainingName)
        {
            Action action = () => Training.Create(trainingName,1, Color.Empty.ToArgb());
            action.ShouldThrow<TrainingEmptyNameException>();
        }        
    }
}