namespace GestionFormation.CoreDomain.Places.Queries
{
    public class ConventionPlaceResult : IConventionPlaceResult
    {    
        public NomComplet Stagiaire { get; set; }
        public string Societe { get; set; }
        public string Adresse { get; set; }
        public string CodePostal { get; set; }
        public string Ville { get; set; }
    }
}