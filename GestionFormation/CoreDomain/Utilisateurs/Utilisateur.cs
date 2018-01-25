using System;
using GestionFormation.CoreDomain.Utilisateurs.Events;
using GestionFormation.CoreDomain.Utilisateurs.Exceptions;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Utilisateurs
{
    public class Utilisateur : AggregateRootUpdatableAndDeletable<UtilisateurUpdated, UtilisateurDeleted>
    {
        private UtilisateurRole _currentRole;

        public Utilisateur(History history) : base(history)
        {
        }

        protected override void AddPlayers(EventPlayer player)
        {
            base.AddPlayers(player);
            player.Add<UtilisateurCreated>(e => _currentRole = e.Role);
        }

        public static Utilisateur Create(string login, string password, string nom, string prenom, string email, UtilisateurRole role)
        {
            if(string.IsNullOrWhiteSpace(login))
                throw new UtilisateurEmptyException("Le login");
            if(string.IsNullOrWhiteSpace(password))
                throw new UtilisateurEmptyException("Le mot de passe");
            if(string.IsNullOrWhiteSpace(nom))
                throw new UtilisateurEmptyException("Le nom");

            var utilisateur = new Utilisateur(History.Empty);
            utilisateur.AggregateId = Guid.NewGuid();
            
            utilisateur.UncommitedEvents.Add(new UtilisateurCreated(utilisateur.AggregateId, 1, login, password.GetHash(), nom, prenom, email, role));
            return utilisateur;
        }

        public void Update(string nom, string prenom, string email, bool isEnabled)
        {                     
            if (string.IsNullOrWhiteSpace(nom))
                throw new UtilisateurEmptyException("Le nom");

            Update(new UtilisateurUpdated(AggregateId, GetNextSequence(), nom, prenom, email, isEnabled));
        }

        public void Delete()
        {
            Delete(new UtilisateurDeleted(AggregateId, GetNextSequence()));
        }

        public void ChangePassword(string newPassword)
        {
            if(string.IsNullOrWhiteSpace(newPassword))
                throw new UtilisateurEmptyException("Le mot de passe");

            RaiseEvent(new PasswordChanged(AggregateId, GetNextSequence(), newPassword.GetHash()));
        }

        public void ChangeRole(UtilisateurRole role)
        {
            if(_currentRole != role)
                RaiseEvent(new UtilisateurRoleChanged(AggregateId, GetNextSequence(), role));
        }
    }

    public class UtilisateurRoleChanged : DomainEvent
    {
        public UtilisateurRole Role { get; }

        public UtilisateurRoleChanged(Guid aggregateId, int sequence, UtilisateurRole role) : base(aggregateId, sequence)
        {
            Role = role;
        }
    }
}
