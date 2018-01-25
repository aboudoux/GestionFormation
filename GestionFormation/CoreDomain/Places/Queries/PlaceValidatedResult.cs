namespace GestionFormation.CoreDomain.Places.Queries
{
    public class PlaceValidatedResult : IPlaceValidatedResult
    {
        public PlaceValidatedResult(string nomStagiaire, string prenomStagiaire, string societe, string nomContact, string prenomContact, string telephone, string email)
        {
            Stagiaire = new NomComplet(nomStagiaire, prenomStagiaire);
            Societe = societe;
            Contact = new NomComplet(nomContact, prenomContact);
            Telephone = telephone;
            Email = email;
        }
        public NomComplet Stagiaire { get; }
        public string Societe { get; }
        public NomComplet Contact { get; }
        public string Telephone { get; }
        public string Email { get; }
    }
}