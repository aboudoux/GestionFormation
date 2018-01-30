using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GestionFormation.Applications.Contacts;
using GestionFormation.Applications.Conventions;
using GestionFormation.Applications.Formateurs;
using GestionFormation.Applications.Formations;
using GestionFormation.Applications.Lieux;
using GestionFormation.Applications.Places;
using GestionFormation.Applications.Sessions;
using GestionFormation.Applications.Societes;
using GestionFormation.Applications.Stagiaires;
using GestionFormation.CoreDomain.Agreements;
using GestionFormation.CoreDomain.Agreements.Queries;
using GestionFormation.CoreDomain.Seats.Queries;
using GestionFormation.Tests.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GestionFormation.Tests
{
    [TestClass]
    [TestCategory("IntegrationTests")]
    public class SqlIntegrationsTests
    {
        [TestMethod]
        public void plan_new_session()
        {
            var service = new SqlTestApplicationService();

            var formateur = service.Command<CreateFormateur>().Execute("TEST", DateTime.Now.ToString("G"), "test@test.com");
            var lieu = service.Command<CreateLieu>().Execute(DateTime.Now.ToString("G"), "test", 5);

            var createdFormation = service.Command<CreateFormation>().Execute("Essai " + DateTime.Now.ToString("G"), 2);
            service.Command<PlanSession>().Execute(createdFormation.AggregateId, new DateTime(2018,1,8), 3, 5, lieu.AggregateId, formateur.AggregateId);
        }

        [TestMethod]
        public void test_convention_query()
        {
            // given
            var service = new SqlTestApplicationService();
            
            var formateur = service.Command<CreateFormateur>().Execute("TEST CONVENTION", DateTime.Now.ToString("G"), "test@test.com");
            var lieu = service.Command<CreateLieu>().Execute(DateTime.Now.ToString("G") + " - " + Guid.NewGuid(), "test convention", 5);
            var stagiaire = service.Command<CreateStagiaire>().Execute("STAGIAIRE", "CONVENTION TEST");
            var societe1 = service.Command<CreateSociete>().Execute("SOCIETE1", "CONVENTION TEST", "", "");
            var societe2 = service.Command<CreateSociete>().Execute("SOCIETE2", "CONVENTION TEST", "", "");
            var societe3 = service.Command<CreateSociete>().Execute("SOCIETE3", "CONVENTION TEST", "", "");

            var createdFormation = service.Command<CreateFormation>().Execute("Essai convention" + DateTime.Now.ToString("G"), 2);
            var session = service.Command<PlanSession>().Execute(createdFormation.AggregateId, new DateTime(2018, 1, 15), 3, 5, lieu.AggregateId, formateur.AggregateId);

            var place1 = service.Command<ReservePlace>().Execute(session.AggregateId, stagiaire.AggregateId, societe1.AggregateId);
            var place2 = service.Command<ReservePlace>().Execute(session.AggregateId, stagiaire.AggregateId, societe1.AggregateId);
            var place3 = service.Command<ReservePlace>().Execute(session.AggregateId, stagiaire.AggregateId, societe2.AggregateId);
            var place4 = service.Command<ReservePlace>().Execute(session.AggregateId, stagiaire.AggregateId, societe2.AggregateId);
            var place5 = service.Command<ReservePlace>().Execute(session.AggregateId, stagiaire.AggregateId, societe3.AggregateId);

            service.Command<ValiderPlace>().Execute(place1.AggregateId);
            service.Command<ValiderPlace>().Execute(place2.AggregateId);
            service.Command<ValiderPlace>().Execute(place3.AggregateId);
            service.Command<ValiderPlace>().Execute(place4.AggregateId);
            service.Command<ValiderPlace>().Execute(place5.AggregateId);
            
            var contact = service.Command<CreateContact>().Execute(place1.CompanyId,"CONTACT", "CONVENTION TEST","","");

            service.Command<CreateConvention>().Execute(contact.AggregateId, new List<Guid>(){ place1.AggregateId, place2.AggregateId}, AgreementType.Free);
            service.Command<CreateConvention>().Execute(contact.AggregateId, new List<Guid>(){ place3.AggregateId, place4.AggregateId}, AgreementType.Free);
            service.Command<CreateConvention>().Execute(contact.AggregateId, new List<Guid>(){ place5.AggregateId}, AgreementType.Free);

            // when
            var conventionQueries = new AgreementQueries();
            var allConventions = conventionQueries.GetAll(session.AggregateId);

            // then 
            allConventions.Should().HaveCount(3);
            allConventions.First(a => a.Company == "SOCIETE1").Seats.Count.Should().Be(2);
            allConventions.First(a => a.Company == "SOCIETE2").Seats.Count.Should().Be(2);
            allConventions.First(a => a.Company == "SOCIETE3").Seats.Count.Should().Be(1);

            allConventions.All(a => !string.IsNullOrWhiteSpace(a.AgreementNumber)).Should().BeTrue();
        }

        [TestMethod]
        public void get_places_without_convention()
        {
            var service = new SqlTestApplicationService();

            var formateur = service.Command<CreateFormateur>().Execute("TEST CONVENTION", DateTime.Now.ToString("G"), "test@test.com");
            var lieu = service.Command<CreateLieu>().Execute(DateTime.Now.ToString("G") + " - " + Guid.NewGuid(), "test convention", 5);
            var stagiaire = service.Command<CreateStagiaire>().Execute("STAGIAIRE", "CONVENTION TEST");
            var societe1 = service.Command<CreateSociete>().Execute("SOCIETE1", "CONVENTION TEST", "", "");

            var createdFormation = service.Command<CreateFormation>().Execute($"Essai convention {Guid.NewGuid()}" + DateTime.Now.ToString("G"), 2);
            var session = service.Command<PlanSession>().Execute(createdFormation.AggregateId, new DateTime(2018, 1, 15), 3, 5, lieu.AggregateId, formateur.AggregateId);

            var place1 = service.Command<ReservePlace>().Execute(session.AggregateId, stagiaire.AggregateId, societe1.AggregateId);
            var place2 = service.Command<ReservePlace>().Execute(session.AggregateId, stagiaire.AggregateId, societe1.AggregateId);
            var place3 = service.Command<ReservePlace>().Execute(session.AggregateId, stagiaire.AggregateId, societe1.AggregateId);

            service.Command<ValiderPlace>().Execute(place2.AggregateId);
            service.Command<ValiderPlace>().Execute(place3.AggregateId);

            var contact = service.Command<CreateContact>().Execute(place1.CompanyId,"CONTACT", "CONVENTION TEST", "", "");
            service.Command<CreateConvention>().Execute(contact.AggregateId, new List<Guid>() { place2.AggregateId }, AgreementType.Free);
            var convention2 = service.Command<CreateConvention>().Execute(contact.AggregateId, new List<Guid>() { place3.AggregateId }, AgreementType.Free);
            service.Command<SignConvention>().Execute(convention2.AggregateId, Guid.NewGuid());

            var query = new SeatQueries();
            var places = query.GetAll(session.AggregateId);

            places.Should().HaveCount(3);
            
            var place_1 = places.First(a => a.SeatId == place1.AggregateId);
            place_1.AgreementId.Should().BeNull();
            place_1.Agreementnumber.Should().BeNullOrEmpty();
            place_1.AgreementSigned.Should().BeFalse();

            var place_2 = places.First(a => a.SeatId == place2.AggregateId);
            place_2.AgreementId.Should().NotBeNull();
            place_2.Agreementnumber.Should().NotBeNullOrWhiteSpace();
            place_2.AgreementSigned.Should().BeFalse();

            var place_3 = places.First(a => a.SeatId == place3.AggregateId);
            place_3.AgreementId.Should().NotBeNull();
            place_3.Agreementnumber.Should().NotBeNullOrWhiteSpace();
            place_3.AgreementSigned.Should().BeTrue();
        }      
    }
}