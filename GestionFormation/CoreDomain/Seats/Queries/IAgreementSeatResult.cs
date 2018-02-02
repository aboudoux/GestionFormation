namespace GestionFormation.CoreDomain.Seats.Queries
{
    public interface IAgreementSeatResult
    {
        FullName Student { get; }
        string Company { get; }
        string Address { get; }
        string ZipCode { get; }
        string City { get; }
    }
}