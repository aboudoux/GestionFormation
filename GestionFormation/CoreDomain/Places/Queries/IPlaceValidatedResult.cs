namespace GestionFormation.CoreDomain.Places.Queries
{
    public interface IPlaceValidatedResult
    {
        NomComplet Stagiaire { get; }
        string Societe { get; }
        NomComplet Contact { get; }
        string Telephone { get; }
        string Email { get; }
    }
}