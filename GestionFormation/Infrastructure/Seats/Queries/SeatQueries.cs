using System;
using System.Collections.Generic;
using System.Linq;
using GestionFormation.CoreDomain;
using GestionFormation.CoreDomain.Seats;
using GestionFormation.CoreDomain.Seats.Queries;
using GestionFormation.EventStore;
using GestionFormation.Kernel;

namespace GestionFormation.Infrastructure.Seats.Queries
{
    public class SeatQueries : ISeatQueries, IRuntimeDependency
    {
        public IEnumerable<ISeatResult> GetAll(Guid sessionId)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var querie = from seat in context.Seats
                    where seat.SessionId == sessionId
                    join student in context.Students on seat.StudentId equals student.StudentId
                    join company in context.Companies on seat.CompanyId equals company.CompanyId
                    join agreement in context.Agreements on seat.AssociatedAgreementId equals agreement.AgreementId into pc
                    from agreement in pc.DefaultIfEmpty()
                    select new {Seat = seat, Agreement = agreement, student.Firstname, student.Lastname, company.Name};

                return querie.ToList().Select(a => new SeatResult(a.Seat, a.Agreement, a.Lastname, a.Firstname, a.Name));
            }
        }

        public ISeatResult GetSeat(Guid seatId)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var querie = from seat in context.Seats
                    where seat.SeatId == seatId
                    join student in context.Students on seat.StudentId equals student.StudentId
                    join company in context.Companies on seat.CompanyId equals company.CompanyId
                    join agreement in context.Agreements on seat.AssociatedAgreementId equals agreement.AgreementId into pc
                    from agreement in pc.DefaultIfEmpty()
                    select new { Seat = seat, Agreement = agreement, student.Firstname, student.Lastname, company.Name };

                var result = querie.FirstOrDefault();
                return result == null ? null : new SeatResult(result.Seat, result.Agreement, result.Lastname, result.Firstname, result.Name);
            }
        }

        public IEnumerable<IAgreementSeatResult> GetSeatAgreements(Guid agreementId)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var querie = from p in context.Seats
                    where p.AssociatedAgreementId == agreementId
                    join student in context.Students on p.StudentId equals student.StudentId
                    join company in context.Companies on p.CompanyId equals company.CompanyId
                    select new
                    {
                        StudentLastname = student.Lastname,
                        StudentFirstname = student.Firstname,
                        CompanyName = company.Name,
                        company.Address,
                        company.ZipCode,
                        company.City,                        
                    };

                return querie.ToList().Select(a=>new AgreementSeatResult(){ Company = a.CompanyName, Student = new FullName(a.StudentLastname , a.StudentFirstname), Address = a.Address, ZipCode = a.ZipCode, City = a.City});
            }
        }

        public IEnumerable<ISeatValidatedResult> GetValidatedSeats(Guid sessionId)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var querie = from p in context.Seats
                    join agreement in context.Agreements on p.AssociatedAgreementId equals agreement.AgreementId
                    join contact in context.Contacts on agreement.ContactId equals contact.ContactId             
                    join student in context.Students on p.StudentId equals student.StudentId
                    join company in context.Companies on p.CompanyId equals company.CompanyId                                        
                    where p.SessionId == sessionId && p.Status == SeatStatus.Valid //&& agreement.DocumentId.HasValue                    
                    select new
                    {
                        p.SeatId,
                        student.StudentId,
                        StudentLastname = student.Lastname,
                        StudentFirstname = student.Firstname,
                        CompanyName = company.Name,
                        company.Address,
                        company.ZipCode,
                        company.City,
                        ContactLastname = contact.Lastname,
                        ContactFirstname = contact.Firstname,
                        contact.Telephone,
                        contact.Email,
                        Missing = p.StudentMissing,
                        p.CertificateOfAttendanceId,
                        agreement.DocumentId,
                    };

                return querie.ToList().Select(a => new SeatValidatedResult(a.SeatId, a.StudentId, a.StudentLastname,  a.StudentFirstname, a.CompanyName, a.ContactLastname, a.ContactFirstname, a.Telephone, a.Email, a.Missing, a.CertificateOfAttendanceId, a.DocumentId, a.Address, a.ZipCode, a.City));
            }
        }

        public IEnumerable<IListSeat> GetSeatsList()
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var querie = from p in context.Seats
                        join student in context.Students on p.StudentId equals student.StudentId
                        join company in context.Companies on p.CompanyId equals company.CompanyId
                        join session in context.Sessions on p.SessionId equals session.SessionId
                        join trainer in context.Trainers on session.TrainerId equals trainer.TrainerId
                        join training in context.Trainings on session.TrainingId equals training.TrainingId
                        join agreement in context.Agreements on p.AssociatedAgreementId equals agreement.AgreementId into agreements
                        from agreement in agreements.DefaultIfEmpty()
                        join contact in context.Contacts on agreement.ContactId equals contact.ContactId into contacts
                        from contact in contacts.DefaultIfEmpty()

                        select new ListSeatResult()
                        {
                            SeatStatus = p.Status,
                            Company = company.Name,
                            StudentLastname = student.Lastname,
                            StudentFirstname = student.Firstname,
                            TrainerLastname = trainer.Lastname,
                            TrainerFirstname = trainer.Firstname,
                            Training = training.Name,
                            SessionStart = session.SessionStart,
                            Duration = session.Duration,
                            AgreementNumber = agreement == null ? "" : agreement.AgreementNumber,
                            ContactLastname = agreement == null ? "" : contact.Lastname,
                            Contactfirstname = agreement == null ? "" : contact.Firstname,
                            Telephone = agreement == null ? "" : contact.Telephone,
                            Email = agreement == null ? "" : contact.Email
                        };

                return querie.ToList();
            }
        }
    }

    public class ListSeatResult : IListSeat
    {
        public SeatStatus SeatStatus { get; set; }
        public string Company { get; set; }
        public string StudentLastname { get; set; }
        public string StudentFirstname { get; set; }
        public string TrainerLastname { get; set; }
        public string TrainerFirstname { get; set; }
        public string Training { get; set; }
        public DateTime SessionStart { get; set; }
        public int Duration { get; set; }
        public string AgreementNumber { get; set; }
        public string ContactLastname { get; set; }
        public string Contactfirstname { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
    }
}
