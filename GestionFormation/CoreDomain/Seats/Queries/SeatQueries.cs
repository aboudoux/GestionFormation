using System;
using System.Collections.Generic;
using System.Linq;
using GestionFormation.EventStore;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Seats.Queries
{
    public class SeatQueries : ISeatQueries, IRuntimeDependency
    {
        public IEnumerable<ISeatResult> GetAll(Guid sessionId)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var querie = from seat in context.Seats
                    where seat.SessionId == sessionId
                    join agreement in context.Agreements on seat.AssociatedAgreementId equals agreement.AgreementId into pc
                    from agreement in pc.DefaultIfEmpty()
                    select new {Seat = seat, Agreement = agreement};

                return querie.ToList().Select(a => new SeatResult(a.Seat, a.Agreement));
            }
        }

        public ISeatResult GetSeat(Guid seatId)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var querie = from seat in context.Seats
                    where seat.SeatId == seatId
                    join agreement in context.Agreements on seat.AssociatedAgreementId equals agreement.AgreementId into pc
                    from agreement in pc.DefaultIfEmpty()
                    select new { Seat = seat, Agreement = agreement };

                var result = querie.FirstOrDefault();
                return result == null ? null : new SeatResult(result.Seat, result.Agreement);
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
                        Addresse = company.Address,
                        CodePostal = company.ZipCode,
                        Ville = company.City,                        
                    };

                return querie.ToList().Select(a=>new AgreementSeatResult(){ Company = a.CompanyName, Student = new FullName(a.StudentLastname , a.StudentFirstname), Address = a.Addresse, ZipCode = a.CodePostal, City = a.Ville});
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
                    where p.SessionId == sessionId && p.Status == SeatStatus.Valid && agreement.DocumentId.HasValue
                    select new
                    {
                        StagiaireNom = student.Lastname,
                        StagiairePrenom = student.Firstname,
                        SocieteNom = company.Name,
                        ContactNom = contact.Lastname,
                        ContactPrenom = contact.Firstname,
                        Telephone = contact.Telephone,
                        Email = contact.Email
                    };

                return querie.ToList().Select(a => new SeatValidatedResult(a.StagiaireNom,  a.StagiairePrenom, a.SocieteNom, a.ContactNom, a.ContactPrenom, a.Telephone, a.Email));
            }
        }

        public IEnumerable<IListSeat> GetSeatsList()
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var querie = from p in context.Seats
                        join stagiaire in context.Students on p.StudentId equals stagiaire.StudentId
                        join societe in context.Companies on p.CompanyId equals societe.CompanyId
                        join session in context.Sessions on p.SessionId equals session.SessionId
                        join formateur in context.Trainers on session.TrainerId equals formateur.TrainerId
                        join formation in context.Trainings on session.TrainingId equals formation.TrainingId
                        join convention in context.Agreements on p.AssociatedAgreementId equals convention.AgreementId into conventions
                        from convention in conventions.DefaultIfEmpty()
                        join contact in context.Contacts on convention.ContactId equals contact.ContactId into contacts
                        from contact in contacts.DefaultIfEmpty()

                        select new ListSeatResult()
                        {
                            SeatStatus = p.Status,
                            Company = societe.Name,
                            StudentLastname = stagiaire.Lastname,
                            StudentFirstname = stagiaire.Firstname,
                            TrainerLastname = formateur.Lastname,
                            TrainerFirstname = formateur.Firstname,
                            Training = formation.Name,
                            SessionStart = session.SessionStart,
                            Duration = session.Duration,
                            AgreementNumber = convention == null ? "" : convention.AgreementNumber,
                            ContactLastname = convention == null ? "" : contact.Lastname,
                            Contactfirstname = convention == null ? "" : contact.Firstname,
                            Telephone = convention == null ? "" : contact.Telephone,
                            Email = convention == null ? "" : contact.Email
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
