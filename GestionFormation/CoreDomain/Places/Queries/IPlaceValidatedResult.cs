namespace GestionFormation.CoreDomain.Places.Queries
{
    public interface IPlaceValidatedResult
    {
        string Stagiaire { get; }
        string Societe { get; }
        string Contact { get; }
        string Telephone { get; }
        string Email { get; }
    }
}