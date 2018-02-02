using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GestionFormation.Applications.Agreements;
using GestionFormation.Applications.Contacts;
using GestionFormation.Applications.Formations;
using GestionFormation.Applications.Lieux;
using GestionFormation.Applications.Places;
using GestionFormation.Applications.Sessions;
using GestionFormation.Applications.Societes;
using GestionFormation.Applications.Stagiaires;
using GestionFormation.Applications.Trainers;
using GestionFormation.CoreDomain.Agreements;
using GestionFormation.CoreDomain.Reminders.Projections;
using GestionFormation.CoreDomain.Reminders.Queries;
using GestionFormation.CoreDomain.Seats;
using GestionFormation.CoreDomain.Sessions;
using GestionFormation.CoreDomain.Users;
using GestionFormation.Tests.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GestionFormation.Tests
{
    [TestClass]
    [TestCategory("IntegrationTests")]
    public class RappelSqlProjectionsShould
    {
        private RappelTestContext _context;
        private static SqlTestApplicationService _service;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            _service = new SqlTestApplicationService();
        }
        
        [TestInitialize]
        public void TestInitialize()
        {
            
            var formation = _service.Command<CreateFormation>().Execute("TEST RAPPEL " + Guid.NewGuid(), 20);
            var formateur = _service.Command<CreateTrainer>().Execute("TEST RAPPEL " + Guid.NewGuid(), Guid.NewGuid().ToString(), "");
            var lieu = _service.Command<CreateLieu>().Execute("TEST LIEU " + Guid.NewGuid(), "", 20);
            var session = _service.Command<PlanSession>().Execute(formation.AggregateId, DateTime.Now, 1, 20, lieu.AggregateId, formateur.AggregateId);

            _context = new RappelTestContext(_service, session, new ReminderSqlQueries());
        }

        [TestMethod]
        public void create_rappel_on_session_when_create_new_place()
        {
            var place = _context.CreatePlace();

            // then            
            var result = _context.Queries.GetAll(UserRole.Manager).ToList();

            result.Should().Contain(a =>a.SessionId == place.SessionId && a.ReminderType == RappelType.PlaceToValidate);
        }

        [TestMethod]
        public void remove_rappel_on_session_when_place_refused()
        {
            var place = _context.CreatePlace();
            _context.App.Command<RefuserPlace>().Execute(place.AggregateId, "test");

            var result = _context.Queries.GetAll(UserRole.Manager).ToList();
            result.Should().NotContain(a => a.SessionId == place.SessionId);
        }

        [TestMethod]
        public void remove_rappel_on_session_when_place_canceled()
        {
            var place = _context.CreatePlace();
            _context.App.Command<AnnulerPlace>().Execute(place.AggregateId, "test");

            var result = _context.Queries.GetAll(UserRole.Manager).ToList();
            result.Should().NotContain(a => a.SessionId == place.SessionId);
        }

        [TestMethod]
        public void create_rappel_for_convention_when_place_validated()
        {
            var place = _context.CreatePlace();
            _context.App.Command<ValiderPlace>().Execute(place.AggregateId);

            var result = _context.Queries.GetAll(UserRole.Operator).ToList();
            result.Should().Contain(a => a.SessionId == place.SessionId && a.ReminderType == RappelType.ConventionToCreate);
        }

        [TestMethod]
        public void create_just_one_rappel_of_create_convention_if_validate_places_for_same_societe()
        {
            var place1 = _context.CreatePlace();
            var place2 = _context.CreatePlace(place1.CompanyId);
            var place3 = _context.CreatePlace(place1.CompanyId);

            _context.App.Command<ValiderPlace>().Execute(place1.AggregateId);
            _context.App.Command<ValiderPlace>().Execute(place2.AggregateId);
            _context.App.Command<ValiderPlace>().Execute(place3.AggregateId);

            var result = _context.Queries.GetAll(UserRole.Operator).ToList();
            result.Where(a => a.SessionId == place1.SessionId).Should().HaveCount(1);
        }

        [TestMethod]
        public void create_distinct_rappel_convention_if_validate_place_of_different_societe()
        {
            var place1 = _context.CreatePlace();
            var place2 = _context.CreatePlace(place1.CompanyId);
            var place3 = _context.CreatePlace();
            var place4 = _context.CreatePlace(place3.CompanyId);

            _context.App.Command<ValiderPlace>().Execute(place1.AggregateId);
            _context.App.Command<ValiderPlace>().Execute(place2.AggregateId);
            _context.App.Command<ValiderPlace>().Execute(place3.AggregateId);
            _context.App.Command<ValiderPlace>().Execute(place4.AggregateId);

            var result = _context.Queries.GetAll(UserRole.Operator).ToList();
            result.Where(a => a.SessionId == place1.SessionId).Should().HaveCount(2);
        }

        [TestMethod]
        public void create_rappel_sign_convention_when_convention_created()
        {
            var place1 = _context.CreatePlace();            
            var contact = _context.App.Command<CreateContact>().Execute(place1.CompanyId,"TEST RAPPEL", "test", "", "");
            _context.App.Command<ValiderPlace>().Execute(place1.AggregateId);
            var convention = _context.App.Command<CreateAgreement>().Execute(contact.AggregateId, new List<Guid>() { place1.AggregateId}, AgreementType.Free);

            var result = _context.Queries.GetAll(UserRole.Operator).ToList();
            result.Where(a=>a.AgreementId == convention.AggregateId && a.ReminderType == RappelType.ConventionToSign );
            result.Where(a => a.SessionId == place1.SessionId).Should().HaveCount(0);
        }

        [TestMethod]
        public void create_rappel_convention_if_revoke_convention_with_validated_place()
        {
            var place1 = _context.CreatePlace();
            var place2 = _context.CreatePlace(place1.CompanyId);

            _context.App.Command<ValiderPlace>().Execute(place1.AggregateId);
            _context.App.Command<ValiderPlace>().Execute(place2.AggregateId);

            var contact = _context.App.Command<CreateContact>().Execute(place1.CompanyId,"TEST RAPPEL", "test", "", "");
            _context.App.Command<CreateAgreement>().Execute(contact.AggregateId, new List<Guid>() { place1.AggregateId , place2.AggregateId}, AgreementType.Free);

            _context.App.Command<AnnulerPlace>().Execute(place1.AggregateId, "test");

            var result = _context.Queries.GetAll(UserRole.Operator).ToList();
            result.Should().Contain(a => a.SessionId == place2.SessionId && a.ReminderType == RappelType.ConventionToCreate);
        }

        [TestMethod]
        public void remove_rappel_convention_if_all_place_canceled_or_refused()
        {
            var place1 = _context.CreatePlace();
            var place2 = _context.CreatePlace(place1.CompanyId);

            _context.App.Command<ValiderPlace>().Execute(place1.AggregateId);
            _context.App.Command<ValiderPlace>().Execute(place2.AggregateId);            

            var contact = _context.App.Command<CreateContact>().Execute(place1.CompanyId, "TEST RAPPEL", "test", "", "");
            _context.App.Command<CreateAgreement>().Execute(contact.AggregateId, new List<Guid>() { place1.AggregateId, place2.AggregateId }, AgreementType.Free);

            _context.App.Command<AnnulerPlace>().Execute(place1.AggregateId, "test");
            _context.App.Command<AnnulerPlace>().Execute(place2.AggregateId, "test");
            var result = _context.Queries.GetAll(UserRole.Operator).ToList();
            result.Should().NotContain(a => a.SessionId == place2.SessionId && a.ReminderType == RappelType.ConventionToCreate);
        }

        [TestMethod]
        public void remove_rappel_when_convention_created_on_revoked_convention()
        {
            var place1 = _context.CreatePlace();
            var place2 = _context.CreatePlace(place1.CompanyId);

            _context.App.Command<ValiderPlace>().Execute(place1.AggregateId);
            _context.App.Command<ValiderPlace>().Execute(place2.AggregateId);

            var contact = _context.App.Command<CreateContact>().Execute(place1.CompanyId,"TEST RAPPEL", "test", "", "");
            _context.App.Command<CreateAgreement>().Execute(contact.AggregateId, new List<Guid>() { place1.AggregateId, place2.AggregateId }, AgreementType.Free);

            _context.App.Command<AnnulerPlace>().Execute(place1.AggregateId, "test");
            var convention = _context.App.Command<CreateAgreement>().Execute(contact.AggregateId, new List<Guid>() { place2.AggregateId }, AgreementType.Free);

            var result = _context.Queries.GetAll(UserRole.Operator).ToList();
            result.Should().NotContain(a => a.AgreementId == convention.AggregateId && a.ReminderType == RappelType.ConventionToCreate);
            result.Should().Contain(a => a.AgreementId == convention.AggregateId && a.ReminderType == RappelType.ConventionToSign);
        }
    }

    public class RappelTestContext
    {
        private readonly SqlTestApplicationService _service;
        private readonly Session _session;
        private readonly IReminderQueries _queries;

        public RappelTestContext(SqlTestApplicationService service, Session session, IReminderQueries queries)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _queries = queries;
        }

        public SqlTestApplicationService App => _service;
        public Guid SessionId => _session.AggregateId;

        public IReminderQueries Queries => _queries;
        

        public Seat CreatePlace(Guid? societeId = null)
        {
            var stagiaire = _service.Command<CreateStagiaire>().Execute("TEST RAPPEL " + Guid.NewGuid(), Guid.NewGuid().ToString());
            if (!societeId.HasValue)
            {
                var societe = _service.Command<CreateSociete>().Execute("TEST RAPPEL " + Guid.NewGuid(), "", "", "");
                societeId = societe.AggregateId;
            }

            var place = _service.Command<ReservePlace>().Execute(SessionId, stagiaire.AggregateId, societeId.Value);
            place.SessionId.Should().NotBeEmpty();            

            return place;
        }        
    }
}