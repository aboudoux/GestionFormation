using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using FluentAssertions;
using GestionFormation.Applications.Agreements;
using GestionFormation.Applications.Companies;
using GestionFormation.Applications.Contacts;
using GestionFormation.Applications.Locations;
using GestionFormation.Applications.Seats;
using GestionFormation.Applications.Sessions;
using GestionFormation.Applications.Students;
using GestionFormation.Applications.Trainers;
using GestionFormation.Applications.Trainings;
using GestionFormation.CoreDomain.Agreements;
using GestionFormation.Infrastructure.Agreements.Queries;
using GestionFormation.Infrastructure.Seats.Queries;
using GestionFormation.Tests.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GestionFormation.Tests
{
    [TestClass]
    [TestCategory("IntegrationTests")]
    public class SqlIntegrationsTests
    {
        private static SqlTestApplicationService _service;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            _service = new SqlTestApplicationService();
        }

        [TestMethod]
        public void plan_new_session()
        {
            var trainer = _service.Command<CreateTrainer>().Execute("TEST", DateTime.Now.ToString("G"), "test@test.com");
            var location = _service.Command<CreateLocation>().Execute(DateTime.Now.ToString("G"), "test", 5);

            var createdFormation = _service.Command<CreateTraining>().Execute("Essai " + DateTime.Now.ToString("G"), 2, Color.Empty.ToArgb());
            _service.Command<PlanSession>().Execute(createdFormation.AggregateId, new DateTime(2018,1,8), 3, 5, location.AggregateId, trainer.AggregateId);
        }

        [TestMethod]
        public void test_convention_query()
        {
            // given
            var trainer = _service.Command<CreateTrainer>().Execute("TEST CONVENTION", DateTime.Now.ToString("G"), "test@test.com");
            var location = _service.Command<CreateLocation>().Execute(DateTime.Now.ToString("G") + " - " + Guid.NewGuid(), "test convention", 5);
            var student = _service.Command<CreateStudent>().Execute("STAGIAIRE", "CONVENTION TEST");

            var sname1 = "SOCIETE1 " + Guid.NewGuid();
            var sname2 = "SOCIETE2 " + Guid.NewGuid();
            var sname3 = "SOCIETE3 " + Guid.NewGuid();

            var company1 = _service.Command<CreateCompany>().Execute(sname1, "CONVENTION TEST", "", "");
            var company2 = _service.Command<CreateCompany>().Execute(sname2, "CONVENTION TEST", "", "");
            var company3 = _service.Command<CreateCompany>().Execute(sname3, "CONVENTION TEST", "", "");

            var createdTraining = _service.Command<CreateTraining>().Execute("Essai convention" + DateTime.Now.ToString("G"), 2, Color.Empty.ToArgb());
            var session = _service.Command<PlanSession>().Execute(createdTraining.AggregateId, new DateTime(2018, 1, 15), 3, 5, location.AggregateId, trainer.AggregateId);

            var seat1 = _service.Command<ReserveSeat>().Execute(session.AggregateId, student.AggregateId, company1.AggregateId, true);
            var seat2 = _service.Command<ReserveSeat>().Execute(session.AggregateId, student.AggregateId, company1.AggregateId, true);
            var seat3 = _service.Command<ReserveSeat>().Execute(session.AggregateId, student.AggregateId, company2.AggregateId, true);
            var seat4 = _service.Command<ReserveSeat>().Execute(session.AggregateId, student.AggregateId, company2.AggregateId, true);
            var seat5 = _service.Command<ReserveSeat>().Execute(session.AggregateId, student.AggregateId, company3.AggregateId, true);

            _service.Command<ValidateSeat>().Execute(seat1.AggregateId);
            _service.Command<ValidateSeat>().Execute(seat2.AggregateId);
            _service.Command<ValidateSeat>().Execute(seat3.AggregateId);
            _service.Command<ValidateSeat>().Execute(seat4.AggregateId);
            _service.Command<ValidateSeat>().Execute(seat5.AggregateId);
            
            var contact = _service.Command<CreateContact>().Execute(seat1.CompanyId,"CONTACT", "CONVENTION TEST","","");

            _service.Command<CreateAgreement>().Execute(contact.AggregateId, new List<Guid>(){ seat1.AggregateId, seat2.AggregateId}, AgreementType.Free);
            _service.Command<CreateAgreement>().Execute(contact.AggregateId, new List<Guid>(){ seat3.AggregateId, seat4.AggregateId}, AgreementType.Free);
            _service.Command<CreateAgreement>().Execute(contact.AggregateId, new List<Guid>(){ seat5.AggregateId}, AgreementType.Free);

            // when
            var conventionQueries = new AgreementQueries();
            var allConventions = conventionQueries.GetAll(session.AggregateId);

            // then 
            allConventions.Should().HaveCount(3);
            allConventions.First(a => a.Company == sname1).Seats.Count.Should().Be(2);
            allConventions.First(a => a.Company == sname2).Seats.Count.Should().Be(2);
            allConventions.First(a => a.Company == sname3).Seats.Count.Should().Be(1);

            allConventions.All(a => !string.IsNullOrWhiteSpace(a.AgreementNumber)).Should().BeTrue();
        }

        [TestMethod]
        public void get_places_without_convention()
        {
            var trainer = _service.Command<CreateTrainer>().Execute("TEST CONVENTION " + Guid.NewGuid(), DateTime.Now.ToString("G"), "test@test.com");
            var location = _service.Command<CreateLocation>().Execute(DateTime.Now.ToString("G") + " - " + Guid.NewGuid(), "test convention", 5);
            var student = _service.Command<CreateStudent>().Execute("STAGIAIRE", "CONVENTION TEST");
            var company1 = _service.Command<CreateCompany>().Execute("SOCIETE1 " + Guid.NewGuid(), "CONVENTION TEST", "", "");

            var createdTraining = _service.Command<CreateTraining>().Execute($"Essai convention {Guid.NewGuid()}" + DateTime.Now.ToString("G"), 2, Color.Empty.ToArgb());
            var session = _service.Command<PlanSession>().Execute(createdTraining.AggregateId, new DateTime(2018, 1, 15), 3, 5, location.AggregateId, trainer.AggregateId);

            var seat1 = _service.Command<ReserveSeat>().Execute(session.AggregateId, student.AggregateId, company1.AggregateId, true);
            var seat2 = _service.Command<ReserveSeat>().Execute(session.AggregateId, student.AggregateId, company1.AggregateId, true);
            var seat3 = _service.Command<ReserveSeat>().Execute(session.AggregateId, student.AggregateId, company1.AggregateId, true);

            _service.Command<ValidateSeat>().Execute(seat2.AggregateId);
            _service.Command<ValidateSeat>().Execute(seat3.AggregateId);

            var contact = _service.Command<CreateContact>().Execute(seat1.CompanyId,"CONTACT", "CONVENTION TEST", "", "");
            _service.Command<CreateAgreement>().Execute(contact.AggregateId, new List<Guid>() { seat2.AggregateId }, AgreementType.Free);
            var agreement = _service.Command<CreateAgreement>().Execute(contact.AggregateId, new List<Guid>() { seat3.AggregateId }, AgreementType.Free);
            _service.Command<SignAgreement>().Execute(agreement.AggregateId, Guid.NewGuid());

            var query = new SeatQueries();
            var seats = query.GetAll(session.AggregateId);

            seats.Should().HaveCount(3);
            
            var seat_1 = seats.First(a => a.SeatId == seat1.AggregateId);
            seat_1.AgreementId.Should().BeNull();
            seat_1.Agreementnumber.Should().BeNullOrEmpty();
            seat_1.AgreementSigned.Should().BeFalse();

            var seat_2 = seats.First(a => a.SeatId == seat2.AggregateId);
            seat_2.AgreementId.Should().NotBeNull();
            seat_2.Agreementnumber.Should().NotBeNullOrWhiteSpace();
            seat_2.AgreementSigned.Should().BeFalse();

            var seat_3 = seats.First(a => a.SeatId == seat3.AggregateId);
            seat_3.AgreementId.Should().NotBeNull();
            seat_3.Agreementnumber.Should().NotBeNullOrWhiteSpace();
            seat_3.AgreementSigned.Should().BeTrue();
        }      
    }
}