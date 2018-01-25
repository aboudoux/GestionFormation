using System.Linq;

namespace GestionFormation.CoreDomain
{
    public class NomComplet
    {
        private readonly string _nomComplet;
        
        public NomComplet(string nom, string prenom)
        {
            if (!string.IsNullOrWhiteSpace(prenom))
                _nomComplet = prenom.First().ToString().ToUpper() + prenom.Substring(1).ToLower();

            if (string.IsNullOrWhiteSpace(nom)) return;

            if (string.IsNullOrWhiteSpace(_nomComplet))
                _nomComplet = nom.ToUpper();
            else
                _nomComplet += " " + nom.ToUpper();
        }

        public override string ToString()
        {
            return _nomComplet;
        }
    }
}