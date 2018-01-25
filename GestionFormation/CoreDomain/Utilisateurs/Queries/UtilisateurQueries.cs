using System.Collections.Generic;
using System.Linq;
using GestionFormation.EventStore;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Utilisateurs.Queries
{
    public class UtilisateurQueries : IUtilisateurQueries, IRuntimeDependency
    {
        public IEnumerable<IUtilisateurResult> GetAll()
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                return context.Utilisateurs.ToList().Select(a => new UtilisateurResult(a));
            }
        }

        public IUtilisateurResult GetLogin(string login, string password)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var encryptedPassword = password.GetHash();
                var result = context.Utilisateurs.FirstOrDefault(a =>a.Login == login && a.EncryptedPassword == encryptedPassword && a.IsEnabled);
                return result != null ? new UtilisateurResult(result) : null;
            }
        }

        public bool Exists(string login)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {             
                return context.Utilisateurs.Any(a => a.Login == login);
            }
        }
    }
}