namespace GestionFormation.CoreDomain.Places.Queries
{
    public interface IConventionPlaceResult
    {
        NomComplet Stagiaire { get; }
        string Societe { get; }
        string Adresse { get; }
        string CodePostal { get; }
        string Ville { get; }
    }
}