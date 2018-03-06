using System;
using GestionFormation.CoreDomain;
using GestionFormation.CoreDomain.Seats.Queries;

namespace GestionFormation.Infrastructure.Seats.Queries
{
    public class SeatValidatedResult : ISeatValidatedResult
    {
        public SeatValidatedResult(Guid seatId, Guid studentId, string studentLastname, string studentFirstname, string company, string contactLastName, string contactFirstname, string telephone, string email, bool missing, Guid? certificateOfAttendanceId, Guid? signedAgreementId, string address, string zipCode, string city)
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
            SignedAgreementId = signedAgreementId;
            Address = address;
            ZipCode = zipCode;
            City = city;
        }

        public Guid SeatId { get; }
        public Guid? StudentId { get; }
        public FullName Student { get; }
        public string Company { get; }
        public FullName Contact { get; }
        public string Address { get; }
        public string ZipCode { get; }
        public string City { get; }
        public string Telephone { get; }
        public string Email { get; }
        public bool IsMissing { get; }
        public Guid? CertificateOfAttendanceId { get; }
        public Guid? SignedAgreementId { get; }
    }
}