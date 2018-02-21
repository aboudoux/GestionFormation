using System;
using FluentAssertions;
using GestionFormation.Applications.Companies;
using GestionFormation.Applications.Companies.Exceptions;
using GestionFormation.CoreDomain.Companies.Events;
using GestionFormation.Kernel;
using GestionFormation.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GestionFormation.Tests
{
    [TestClass]
    [TestCategory("UnitTests")]
    public class CompanyShould
    {
        [DataTestMethod]
        [DataRow("trend")]
        [DataRow("Trend")]
        [DataRow("TREND")]
        public void throw_error_if_creating_company_with_same_name(string companyName)
        {
            var queries = new FakeCompanyQueries();
            queries.Add("trend");
            queries.Add("DOT SHARK");
            queries.Add("Peaks");

            Action action = () => new CreateCompany(new EventBus(new EventDispatcher(), new FakeEventStore()), queries).Execute(companyName, String.Empty, String.Empty, String.Empty);
            action.ShouldThrow<CompanyAlreadyExistsException>();
        }

        [TestMethod]
        public void throw_error_if_updating_company_with_same_name()
        {
            var queries = new FakeCompanyQueries();
            queries.Add("trend");

            var eventStore = new FakeEventStore();
            var trainerId = Guid.NewGuid();
            eventStore.Save(new CompanyCreated(trainerId, 1, "Peaks", "", "", ""));

            Action action = () => new UpdateCompany(new EventBus(new EventDispatcher(), eventStore), queries).Execute(trainerId, "TREND", String.Empty, String.Empty, String.Empty);
            action.ShouldThrow<CompanyAlreadyExistsException>();
        }

        [TestMethod]
        public void dont_throw_error_if_updating_existing_company()
        {
            var queries = new FakeCompanyQueries();
            queries.Add("trend");            

            var eventStore = new FakeEventStore();
            var companyId = Guid.NewGuid();
            eventStore.Save(new CompanyCreated(companyId, 1, "Peaks", "", "", ""));
            queries.Add("Peaks", companyId: companyId);

            new UpdateCompany(new EventBus(new EventDispatcher(), eventStore), queries).Execute(companyId, "Peaks", "ceci est un test", String.Empty, String.Empty);

            eventStore.GetEvents(companyId).Should().Contain(new CompanyUpdated(Guid.Empty, 0, "Peaks", "ceci est un test", String.Empty, String.Empty));
        }
    }
}