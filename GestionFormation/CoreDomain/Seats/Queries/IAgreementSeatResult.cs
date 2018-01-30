namespace GestionFormation.CoreDomain.Seats.Queries
{
    public interface IAgreementSeatResult
    {
        FullName Trainee { get; }
        string Company { get; }
        string Address { get; }
        string ZipCode { get; }
        string City { get; }
    }
}