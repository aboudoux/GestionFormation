using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GestionFormation.Applications.Agreements;
using GestionFormation.Applications.BookingNotifications;
using GestionFormation.Applications.Companies;
using GestionFormation.Applications.Contacts;
using GestionFormation.Applications.Locations;
using GestionFormation.Applications.Seats;
using GestionFormation.Applications.Sessions;
using GestionFormation.Applications.Students;
using GestionFormation.Applications.Trainers;
using GestionFormation.Applications.Trainings;
using GestionFormation.CoreDomain.Agreements;
using GestionFormation.CoreDomain.BookingNotifications;
using GestionFormation.CoreDomain.BookingNotifications.Projections;
using GestionFormation.CoreDomain.BookingNotifications.Queries;
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
            
            var formation = _service.Command<CreateTraining>().Execute("TEST RAPPEL " + Guid.NewGuid(), 20);
            var formateur = _service.Command<CreateTrainer>().Execute("TEST RAPPEL " + Guid.NewGuid(), Guid.NewGuid().ToString(), "");
            var lieu = _service.Command<CreateLocation>().Execute("TEST LIEU " + Guid.NewGuid(), "", 20);
            var session = _service.Command<PlanSession>().Execute(formation.AggregateId, DateTime.Now, 1, 20, lieu.AggregateId, formateur.AggregateId);

            _context = new RappelTestContext(_service, session, new BookingNotificationQueries());
        }

        [TestMethod]
        public void create_rappel_on_session_when_create_new_place()
        {
            var place = _context.CreateSeat();

            // then            
            var result = _context.Queries.GetAll(UserRole.Manager).ToList();

            result.Should().Contain(a =>a.SessionId == place.SessionId && a.BookingNotificationType == BookingNotificationType.PlaceToValidate);
        }

        [TestMethod]
        public void remove_rappel_on_session_when_place_refused()
        {
            var place = _context.CreateSeat();
            _context.App.Command<RefuseSeat>().Execute(place.AggregateId, "test");
            _context.App.Command<RemoveBookingNotification>().FromSeat(place.AggregateId);

            var result = _context.Queries.GetAll(UserRole.Manager).ToList();
            result.Should().NotContain(a => a.SessionId == place.SessionId);
        }

        [TestMethod]
        public void remove_rappel_on_session_when_place_canceled()
        {
            var seat = _context.CreateSeat();
            _context.App.Command<CancelSeat>().Execute(seat.AggregateId, "test");
            _context.App.Command<RemoveBookingNotification>().FromSeat(seat.AggregateId);
            
            var result = _context.Queries.GetAll(UserRole.Manager).ToList();
            result.Should().NotContain(a => a.SessionId == seat.SessionId);
        }

        [TestMethod]
        public void create_rappel_for_convention_when_place_validated()
        {
            var seat = _context.CreateSeat();
            _context.App.Command<ValidateSeat>().Execute(seat.AggregateId);
            _context.App.Command<SendAgreementToCreateNotification>().Execute(seat.SessionId, seat.CompanyId);

            var result = _context.Queries.GetAll(UserRole.Operator).ToList();
            result.Should().Contain(a => a.SessionId == seat.SessionId && a.BookingNotificationType == BookingNotificationType.AgreementToCreate);
        }

        [TestMethod]
        public void create_just_one_rappel_of_create_convention_if_validate_places_for_same_societe()
        {
            var seat1 = _context.CreateSeat();
            var seat2 = _context.CreateSeat(seat1.CompanyId);
            var seat3 = _context.CreateSeat(seat1.CompanyId);

            _context.App.Command<ValidateSeat>().Execute(seat1.AggregateId);
            _context.App.Command<ValidateSeat>().Execute(seat2.AggregateId);
            _context.App.Command<ValidateSeat>().Execute(seat3.AggregateId);
            _context.App.Command<SendAgreementToCreateNotification>().Execute(seat1.SessionId, seat1.CompanyId);


            var result = _context.Queries.GetAll(UserRole.Operator).ToList();
            result.Where(a => a.SessionId == seat1.SessionId).Should().HaveCount(1);
        }

        [TestMethod]
        public void create_distinct_rappel_convention_if_validate_place_of_different_societe()
        {
            var place1 = _context.CreateSeat();
            var place2 = _context.CreateSeat(place1.CompanyId);
            var place3 = _context.CreateSeat();
            var place4 = _context.CreateSeat(place3.CompanyId);

            _context.App.Command<ValidateSeat>().Execute(place1.AggregateId);
            _context.App.Command<ValidateSeat>().Execute(place2.AggregateId);
            _context.App.Command<ValidateSeat>().Execute(place3.AggregateId);
            _context.App.Command<ValidateSeat>().Execute(place4.AggregateId);

            _context.App.Command<SendAgreementToCreateNotification>().Execute(place1.SessionId, place1.CompanyId);
            _context.App.Command<SendAgreementToCreateNotification>().Execute(place3.SessionId, place3.CompanyId);

            var result = _context.Queries.GetAll(UserRole.Operator).ToList();
            result.Where(a => a.SessionId == place1.SessionId).Should().HaveCount(2);
        }

        [TestMethod]
        public void create_rappel_sign_convention_when_convention_created()
        {
            var place1 = _context.CreateSeat();            
            var contact = _context.App.Command<CreateContact>().Execute(place1.CompanyId,"TEST RAPPEL", "test", "", "");
            _context.App.Command<ValidateSeat>().Execute(place1.AggregateId);
            _context.App.Command<SendAgreementToCreateNotification>().Execute(place1.SessionId, place1.CompanyId);

            var agreement = _context.App.Command<CreateAgreement>().Execute(contact.AggregateId, new List<Guid>() { place1.AggregateId}, AgreementType.Free);
            _context.App.Command<SendAgreementToSignNotification>().Execute(place1.SessionId, place1.CompanyId, agreement.AggregateId);

            var result = _context.Queries.GetAll(UserRole.Operator).ToList();
            result.Where(a=>a.AgreementId == agreement.AggregateId && a.BookingNotificationType == BookingNotificationType.AgreementToSign ).Should().HaveCount(1);
        }

        [TestMethod]
        public void create_rappel_convention_if_revoke_convention_with_validated_place()
        {
            var place1 = _context.CreateSeat();
            var place2 = _context.CreateSeat(place1.CompanyId);

            _context.App.Command<ValidateSeat>().Execute(place1.AggregateId);
            _context.App.Command<ValidateSeat>().Execute(place2.AggregateId);
            _context.App.Command<SendAgreementToCreateNotification>().Execute(place1.SessionId, place1.CompanyId);


            var contact = _context.App.Command<CreateContact>().Execute(place1.CompanyId,"TEST RAPPEL", "test", "", "");
            var agreement = _context.App.Command<CreateAgreement>().Execute(contact.AggregateId, new List<Guid>() { place1.AggregateId , place2.AggregateId}, CoreDomain.Agreements.AgreementType.Free);
            _context.App.Command<SendAgreementToSignNotification>().Execute(place1.SessionId, place1.CompanyId, agreement.AggregateId);

            _context.App.Command<CancelSeat>().Execute(place1.AggregateId, "test");
            _context.App.Command<AdjustBookingNotification>().Execute(place1.AggregateId);
            
            var result = _context.Queries.GetAll(UserRole.Operator).ToList();
            result.Should().Contain(a => a.SessionId == place2.SessionId && a.BookingNotificationType == BookingNotificationType.AgreementToCreate);
        }

        [TestMethod]
        public void remove_rappel_convention_if_all_place_canceled_or_refused()
        {
            var place1 = _context.CreateSeat();
            var place2 = _context.CreateSeat(place1.CompanyId);

            _context.App.Command<ValidateSeat>().Execute(place1.AggregateId);
            _context.App.Command<ValidateSeat>().Execute(place2.AggregateId);    
            _context.App.Command<SendAgreementToCreateNotification>().Execute(place1.SessionId, place1.CompanyId);
            
            var contact = _context.App.Command<CreateContact>().Execute(place1.CompanyId, "TEST RAPPEL", "test", "", "");
            var agreement = _context.App.Command<CreateAgreement>().Execute(contact.AggregateId, new List<Guid>() { place1.AggregateId, place2.AggregateId }, CoreDomain.Agreements.AgreementType.Free);
            _context.App.Command<SendAgreementToSignNotification>().Execute(place1.SessionId, place1.CompanyId, agreement.AggregateId);

            _context.App.Command<CancelSeat>().Execute(place1.AggregateId, "test");
            _context.App.Command<AdjustBookingNotification>().Execute(place1.AggregateId);

            _context.App.Command<CancelSeat>().Execute(place2.AggregateId, "test");
            _context.App.Command<AdjustBookingNotification>().Execute(place2.AggregateId);

            var result = _context.Queries.GetAll(UserRole.Operator).ToList();
            result.Should().NotContain(a => a.SessionId == place2.SessionId && a.BookingNotificationType == BookingNotificationType.AgreementToCreate);
        }

        [TestMethod]
        public void remove_rappel_when_convention_created_on_revoked_convention()
        {
            var place1 = _context.CreateSeat();
            var place2 = _context.CreateSeat(place1.CompanyId);

            _context.App.Command<ValidateSeat>().Execute(place1.AggregateId);
            _context.App.Command<ValidateSeat>().Execute(place2.AggregateId);
            _context.App.Command<SendAgreementToCreateNotification>().Execute(place1.SessionId, place1.CompanyId);

            var contact = _context.App.Command<CreateContact>().Execute(place1.CompanyId,"TEST RAPPEL", "test", "", "");
            var agreement = _context.App.Command<CreateAgreement>().Execute(contact.AggregateId, new List<Guid>() { place1.AggregateId, place2.AggregateId }, AgreementType.Free);
            _context.App.Command<SendAgreementToSignNotification>().Execute(place1.SessionId, place1.CompanyId, agreement.AggregateId);

            _context.App.Command<CancelSeat>().Execute(place1.AggregateId, "test");
            _context.App.Command<AdjustBookingNotification>().Execute(place1.AggregateId);

            var convention = _context.App.Command<CreateAgreement>().Execute(contact.AggregateId, new List<Guid>() { place2.AggregateId }, AgreementType.Free);
            _context.App.Command<SendAgreementToSignNotification>().Execute(place1.SessionId, place1.CompanyId, convention.AggregateId);

            var result = _context.Queries.GetAll(UserRole.Operator).ToList();
            result.Should().NotContain(a => a.AgreementId == convention.AggregateId && a.BookingNotificationType == BookingNotificationType.AgreementToCreate);
            result.Should().Contain(a => a.AgreementId == convention.AggregateId && a.BookingNotificationType == BookingNotificationType.AgreementToSign);
        }

        [TestMethod]
        public void dont_remove_create_agreement_reminder_if_exists_non_validated_seat()
        {
            var place1 = _context.CreateSeat();
            var place2 = _context.CreateSeat(place1.CompanyId);

            _context.App.Command<ValidateSeat>().Execute(place1.AggregateId);
            _context.App.Command<SendAgreementToCreateNotification>().Execute(place1.SessionId, place1.CompanyId);
            _context.App.Command<RefuseSeat>().Execute(place2.AggregateId, "test");
            _context.App.Command<AdjustBookingNotification>().Execute(place2.AggregateId);

            var result = _context.Queries.GetAll(UserRole.Operator).ToList();
            result.Should().Contain(a=> a.BookingNotificationType == BookingNotificationType.AgreementToCreate && a.SessionId == place1.SessionId && a.CompanyId == place1.CompanyId);
        }
    }

    public class RappelTestContext
    {
        private readonly SqlTestApplicationService _service;
        private readonly Session _session;
        private readonly INotificationQueries _queries;

        public RappelTestContext(SqlTestApplicationService service, Session session, INotificationQueries queries)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _queries = queries;
        }

        public SqlTestApplicationService App => _service;
        public Guid SessionId => _session.AggregateId;

        public INotificationQueries Queries => _queries;
        

        public Seat CreateSeat(Guid? companyId = null)
        {
            var student = _service.Command<CreateStudent>().Execute("TEST RAPPEL " + Guid.NewGuid(), Guid.NewGuid().ToString());
            if (!companyId.HasValue)
            {
                var company = _service.Command<CreateCompany>().Execute("TEST RAPPEL " + Guid.NewGuid(), "", "", "");
                companyId = company.AggregateId;
            }

            var seat = _service.Command<ReserveSeat>().Execute(SessionId, student.AggregateId, companyId.Value);
            _service.Command<SendSeatToValidateNotification>().Execute(SessionId, seat.CompanyId, seat.AggregateId);
            seat.SessionId.Should().NotBeEmpty();            

            return seat;
        }        
    }
}