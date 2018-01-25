namespace GestionFormation.CoreDomain.Places.Queries
{
    public class PlaceValidatedResult : IPlaceValidatedResult
    {
        public PlaceValidatedResult(string stagiaire, string societe, string contact, string telephone, string email)
        {
            Stagiaire = stagiaire;
            Societe = societe;
            Contact = contact;
            Telephone = telephone;
            Email = email;
        }
        public string Stagiaire { get; }
        public string Societe { get; }
        public string Contact { get; }
        public string Telephone { get; }
        public string Email { get; }
    }
}