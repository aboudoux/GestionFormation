using System;
using GestionFormation.CoreDomain;
using GestionFormation.CoreDomain.Seats.Queries;

namespace GestionFormation.Infrastructure.Seats.Queries
{
    public class SeatValidatedResult : ISeatValidatedResult
    {
        public SeatValidatedResult(Guid seatId, Guid studentId, string studentLastname, string studentFirstname, string company, string contactLastName, string contactFirstname, string telephone, string email, bool missing, Guid? certificateOfAttendanceId)
        {
            SeatId = seatId;
            StudentId = studentId;
            Student = new FullName(studentLastname, studentFirstname);
            Company = company;
            Contact = new FullName(contactLastName, contactFirstname);
            Telephone = telephone;
            Email = email;
            IsMissing = missing;
            CertificateOfAttendanceId = certificateOfAttendanceId;
        }

        public Guid SeatId { get; }
        public Guid StudentId { get; }
        public FullName Student { get; }
        public string Company { get; }
        public FullName Contact { get; }
        public string Telephone { get; }
        public string Email { get; }
        public bool IsMissing { get; }
        public Guid? CertificateOfAttendanceId { get; }
    }
}