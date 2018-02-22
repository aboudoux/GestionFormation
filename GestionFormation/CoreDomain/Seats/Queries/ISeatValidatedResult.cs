using System;

namespace GestionFormation.CoreDomain.Seats.Queries
{
    public interface ISeatValidatedResult
    {
        Guid SeatId { get; }
        Guid StudentId { get; }
        FullName Student { get; }
        string Company { get; }
        FullName Contact { get; }
        string Address { get; }
        string ZipCode { get; }
        string City { get; }
        string Telephone { get; }
        string Email { get; }
        bool IsMissing { get; }
        Guid? CertificateOfAttendanceId { get; }
        Guid? SignedAgreementId { get; }
    }
}