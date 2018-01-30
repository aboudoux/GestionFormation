using System;
using System.Collections.Generic;
using FluentAssertions;
using GestionFormation.CoreDomain.Trainings.Queries;
using GestionFormation.Tests.Fakes;
using GestionFormation.Web.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GestionFormation.Web.Tests.Controllers
{
    [TestClass]
    [TestCategory("UnitTests")]
    public class FormationApiShould
    {
        [TestMethod]
        public void GetAllFormation()
        {
            var fake = new FakeTrainingQueries();
            fake.AddFormation("test1", 1);
            var controller =  new FormationQueryController(fake);
            var result = controller.GetAllFormations().TryGetContent<List<ITrainingResult>>();
            result.Should().HaveCount(1);
        }
    }
}
