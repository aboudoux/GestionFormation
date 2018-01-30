namespace GestionFormation.CoreDomain.Seats.Queries
{
    public interface ISeatValidatedResult
    {
        FullName Trainee { get; }
        string Company { get; }
        FullName Contact { get; }
        string Telephone { get; }
        string Email { get; }
    }
}