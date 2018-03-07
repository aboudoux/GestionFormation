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
using GestionFormation.CoreDomain.Notifications;
using GestionFormation.CoreDomain.Notifications.Queries;
using GestionFormation.CoreDomain.Seats;
using GestionFormation.CoreDomain.Sessions;
using GestionFormation.CoreDomain.Users;
using GestionFormation.Infrastructure.Notifications.Queries;
using GestionFormation.Tests.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GestionFormation.Tests
{
    [TestClass]
    [TestCategory("IntegrationTests")]
    public class NotificationSqlProjectionsShould
    {
        private NotificationTestContext _context;
        private static SqlTestApplicationService _service;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            _service = new SqlTestApplicationService();
        }
        
        [TestInitialize]
        public void TestInitialize()
        {            
            var training = _service.Command<CreateTraining>().Execute("TEST RAPPEL " + Guid.NewGuid(), 20, Color.Empty.ToArgb());
            var trainer = _service.Command<CreateTrainer>().Execute("TEST RAPPEL " + Guid.NewGuid(), Guid.NewGuid().ToString(), "");
            var location = _service.Command<CreateLocation>().Execute("TEST LIEU " + Guid.NewGuid(), "", 20);
            var session = _service.Command<PlanSession>().Execute(training.AggregateId, DateTime.Now, 1, 20, location.AggregateId, trainer.AggregateId);

            _context = new NotificationTestContext(_service, session, new NotificationQueries());
        }

        [TestMethod]
        public void create_notification_on_session_when_create_new_seat()
        {
            var seat = _context.CreateSeat();

            var result = _context.Queries.GetAll(UserRole.Manager).ToList();

            result.Should().Contain(a =>a.SessionId == seat.SessionId && a.NotificationType == NotificationType.SeatToValidate);
        }

        [TestMethod]
        public void remove_notification_on_session_when_seat_refused()
        {
            var seat = _context.CreateSeat();
            _context.App.Command<RefuseSeat>().Execute(seat.AggregateId, "test");

            var result = _context.Queries.GetAll(UserRole.Manager).ToList();
            result.Should().NotContain(a => a.SessionId == seat.SessionId);
        }

        [TestMethod]
        public void remove_notification_on_session_when_seat_canceled()
        {
            var seat = _context.CreateSeat();
            _context.App.Command<CancelSeat>().Execute(seat.AggregateId, "test");
            
            var result = _context.Queries.GetAll(UserRole.Manager).ToList();
            result.Should().NotContain(a => a.SessionId == seat.SessionId);
        }

        [TestMethod]
        public void create_notification_for_agreement_when_seat_validated()
        {
            var seat = _context.CreateSeat();
            _context.App.Command<ValidateSeat>().Execute(seat.AggregateId);

            var result = _context.Queries.GetAll(UserRole.Operator).ToList();
            result.Should().Contain(a => a.SessionId == seat.SessionId && a.NotificationType == NotificationType.AgreementToCreate);
        }

        [TestMethod]
        public void create_just_one_notification_of_create_agreement_if_validate_seat_for_same_company()
        {
            var seat1 = _context.CreateSeat();
            var seat2 = _context.CreateSeat(seat1.CompanyId);
            var seat3 = _context.CreateSeat(seat1.CompanyId);

            _context.App.Command<ValidateSeat>().Execute(seat1.AggregateId);
            _context.App.Command<ValidateSeat>().Execute(seat2.AggregateId);
            _context.App.Command<ValidateSeat>().Execute(seat3.AggregateId);

            var result = _context.Queries.GetAll(UserRole.Operator).ToList();
            result.Where(a => a.SessionId == seat1.SessionId).Should().HaveCount(1);
        }

        [TestMethod]
        public void create_distinct_notification_agreement_if_validate_seat_of_different_company()
        {
            var seat1 = _context.CreateSeat();
            var seat2 = _context.CreateSeat(seat1.CompanyId);
            var seat3 = _context.CreateSeat();
            var seat4 = _context.CreateSeat(seat3.CompanyId);

            _context.App.Command<ValidateSeat>().Execute(seat1.AggregateId);
            _context.App.Command<ValidateSeat>().Execute(seat2.AggregateId);
            _context.App.Command<ValidateSeat>().Execute(seat3.AggregateId);
            _context.App.Command<ValidateSeat>().Execute(seat4.AggregateId);

            var result = _context.Queries.GetAll(UserRole.Operator).ToList();
            result.Where(a => a.SessionId == seat1.SessionId).Should().HaveCount(2);
        }

        [TestMethod]
        public void create_notification_sign_agreement_when_agreement_created()
        {
            var seat1 = _context.CreateSeat();            
            var contact = _context.App.Command<CreateContact>().Execute(seat1.CompanyId,"TEST RAPPEL", "test", "", "");
            _context.App.Command<ValidateSeat>().Execute(seat1.AggregateId);

            var agreement = _context.App.Command<CreateAgreement>().Execute(contact.AggregateId, new List<Guid>() { seat1.AggregateId}, AgreementType.Free);

            var result = _context.Queries.GetAll(UserRole.Operator).ToList();
            result.Where(a=>a.AgreementId == agreement.AggregateId && a.NotificationType == NotificationType.AgreementToSign ).Should().HaveCount(1);
        }

        [TestMethod]
        public void create_notification_agreement_if_revoke_agreement_with_validated_seat()
        {
            var seat1 = _context.CreateSeat();
            var seat2 = _context.CreateSeat(seat1.CompanyId);

            _context.App.Command<ValidateSeat>().Execute(seat1.AggregateId);
            _context.App.Command<ValidateSeat>().Execute(seat2.AggregateId);


            var contact = _context.App.Command<CreateContact>().Execute(seat1.CompanyId,"TEST RAPPEL", "test", "", "");
            var agreement = _context.App.Command<CreateAgreement>().Execute(contact.AggregateId, new List<Guid>() { seat1.AggregateId , seat2.AggregateId}, CoreDomain.Agreements.AgreementType.Free);

            _context.App.Command<CancelSeat>().Execute(seat1.AggregateId, "test");
            
            var result = _context.Queries.GetAll(UserRole.Operator).ToList();
            result.Should().Contain(a => a.SessionId == seat2.SessionId && a.NotificationType == NotificationType.AgreementToCreate);
        }

        [TestMethod]
        public void remove_notification_agreement_if_all_Seat_canceled_or_refused()
        {
            var seat1 = _context.CreateSeat();
            var seat2 = _context.CreateSeat(seat1.CompanyId);

            _context.App.Command<ValidateSeat>().Execute(seat1.AggregateId);
            _context.App.Command<ValidateSeat>().Execute(seat2.AggregateId);    
            
            var contact = _context.App.Command<CreateContact>().Execute(seat1.CompanyId, "TEST RAPPEL", "test", "", "");
            var agreement = _context.App.Command<CreateAgreement>().Execute(contact.AggregateId, new List<Guid>() { seat1.AggregateId, seat2.AggregateId }, CoreDomain.Agreements.AgreementType.Free);

            _context.App.Command<CancelSeat>().Execute(seat1.AggregateId, "test");

            _context.App.Command<CancelSeat>().Execute(seat2.AggregateId, "test");

            var result = _context.Queries.GetAll(UserRole.Operator).ToList();
            result.Should().NotContain(a => a.SessionId == seat2.SessionId && a.NotificationType == NotificationType.AgreementToCreate);
        }

        [TestMethod]
        public void remove_notification_when_agreemenet_created_on_revoked_agreement()
        {
            var seat1 = _context.CreateSeat();
            var seat2 = _context.CreateSeat(seat1.CompanyId);

            _context.App.Command<ValidateSeat>().Execute(seat1.AggregateId);
            _context.App.Command<ValidateSeat>().Execute(seat2.AggregateId);

            var contact = _context.App.Command<CreateContact>().Execute(seat1.CompanyId,"TEST RAPPEL", "test", "", "");
            var agreement = _context.App.Command<CreateAgreement>().Execute(contact.AggregateId, new List<Guid>() { seat1.AggregateId, seat2.AggregateId }, AgreementType.Free);

            _context.App.Command<CancelSeat>().Execute(seat1.AggregateId, "test");

            var convention = _context.App.Command<CreateAgreement>().Execute(contact.AggregateId, new List<Guid>() { seat2.AggregateId }, AgreementType.Free);

            var result = _context.Queries.GetAll(UserRole.Operator).ToList();
            result.Should().NotContain(a => a.AgreementId == convention.AggregateId && a.NotificationType == NotificationType.AgreementToCreate);
            result.Should().Contain(a => a.AgreementId == convention.AggregateId && a.NotificationType == NotificationType.AgreementToSign);
        }

        [TestMethod]
        public void dont_remove_create_agreement_reminder_if_exists_non_validated_seat()
        {
            var seat1 = _context.CreateSeat();
            var seat2 = _context.CreateSeat(seat1.CompanyId);

            _context.App.Command<ValidateSeat>().Execute(seat1.AggregateId);
            _context.App.Command<RefuseSeat>().Execute(seat2.AggregateId, "test");

            var result = _context.Queries.GetAll(UserRole.Operator).ToList();
            result.Should().Contain(a=> a.NotificationType == NotificationType.AgreementToCreate && a.SessionId == seat1.SessionId && a.CompanyId == seat1.CompanyId);
        }

        [TestMethod]
        public void create_distinct_notification_when_adding_prevalided_seat()
        {
            var seat1 = _context.CreateSeat();
            var seat2 = _context.CreateSeat(seat1.CompanyId, false);
            _context.App.Command<ValidateSeat>().Execute(seat2.AggregateId);

            var operatorNotif = _context.Queries.GetAll(UserRole.Operator).Where(a => a.SessionId == seat1.SessionId);
            var managerNotif = _context.Queries.GetAll(UserRole.Manager).Where(a => a.SessionId == seat1.SessionId);

            operatorNotif.Should().HaveCount(1).And.Contain(a=>a.NotificationType == NotificationType.AgreementToCreate);
            managerNotif.Should().HaveCount(1).And.Contain(a => a.NotificationType == NotificationType.SeatToValidate);
        }

        [TestMethod]
        public void not_create_agreementToCreate_notification_twice_when_validating_seat()
        {
            var seat1 = _context.CreateSeat(sendValidationNotification: true);
            var seat2 = _context.CreateSeat(seat1.CompanyId, false);

            _context.App.Command<ValidateSeat>().Execute(seat2.AggregateId);
            _context.App.Command<ValidateSeat>().Execute(seat1.AggregateId);

            var operatorNotif = _context.Queries.GetAll(UserRole.Operator).Where(a => a.SessionId == seat1.SessionId);
            var managerNotif = _context.Queries.GetAll(UserRole.Manager).Where(a => a.SessionId == seat1.SessionId);

            operatorNotif.Should().HaveCount(1).And.Contain(a => a.NotificationType == NotificationType.AgreementToCreate);
            managerNotif.Should().BeEmpty();
        }

        [TestMethod]
        public void not_remove_agreement_to_sign_when_removing_valided_seat_where_agreement_not_created()
        {
            var seat1 = _context.CreateSeat(sendValidationNotification:false);
            var seat2 = _context.CreateSeat(seat1.CompanyId, sendValidationNotification: false);

            _context.App.Command<ValidateSeat>().Execute(seat1.AggregateId);
            _context.App.Command<ValidateSeat>().Execute(seat2.AggregateId);            

            var contact = _context.App.Command<CreateContact>().Execute(seat1.CompanyId, "TEST RAPPEL", "test", "", "");
            var agreement = _context.App.Command<CreateAgreement>().Execute(contact.AggregateId, new List<Guid>() { seat1.AggregateId, seat2.AggregateId }, AgreementType.Free);

            var place3 = _context.CreateSeat(seat1.CompanyId, sendValidationNotification:false);
            _context.App.Command<ValidateSeat>().Execute(place3.AggregateId);

            _context.App.Command<RefuseSeat>().Execute(place3.AggregateId, "test");

            var operatorNotif = _context.Queries.GetAll(UserRole.Operator).Where(a => a.SessionId == seat1.SessionId);
            var managerNotif = _context.Queries.GetAll(UserRole.Manager).Where(a => a.SessionId == seat1.SessionId);

            operatorNotif.Should().HaveCount(1).And.Contain(a => a.NotificationType == NotificationType.AgreementToSign);
            managerNotif.Should().BeEmpty();
        }
    }

    public class NotificationTestContext
    {
        private readonly SqlTestApplicationService _service;
        private readonly Session _session;
        private readonly INotificationQueries _queries;

        public NotificationTestContext(SqlTestApplicationService service, Session session, INotificationQueries queries)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _queries = queries;
        }

        public SqlTestApplicationService App => _service;
        public Guid SessionId => _session.AggregateId;

        public INotificationQueries Queries => _queries;
        

        public Seat CreateSeat(Guid? companyId = null, bool sendValidationNotification = true)
        {
            var student = _service.Command<CreateStudent>().Execute("TEST RAPPEL " + Guid.NewGuid(), Guid.NewGuid().ToString());
            if (!companyId.HasValue)
            {
                var company = _service.Command<CreateCompany>().Execute("TEST RAPPEL " + Guid.NewGuid(), "", "", "");
                companyId = company.AggregateId;
            }

            var seat = _service.Command<BookSeat>().Execute(SessionId, student.AggregateId, companyId.Value, sendValidationNotification);
            
            seat.SessionId.Should().NotBeEmpty();            

            return seat;
        }        
    }
}