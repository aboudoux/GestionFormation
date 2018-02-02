namespace GestionFormation.CoreDomain.Seats.Queries
{
    public interface ISeatValidatedResult
    {
        FullName Student { get; }
        string Company { get; }
        FullName Contact { get; }
        string Telephone { get; }
        string Email { get; }
    }
}