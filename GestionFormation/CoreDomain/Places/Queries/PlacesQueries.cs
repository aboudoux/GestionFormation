using System;
using System.Collections.Generic;
using System.Linq;
using GestionFormation.EventStore;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Places.Queries
{
    public class PlacesQueries : IPlacesQueries, IRuntimeDependency
    {
        public IEnumerable<IPlaceResult> GetAll(Guid sessionId)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var querie = from place in context.Places
                    where place.SessionId == sessionId
                    join convention in context.Conventions on place.AssociatedConventionId equals convention.ConventionId into pc
                    from convention in pc.DefaultIfEmpty()
                    select new {place, convention};

                return querie.ToList().Select(a => new PlaceResult(a.place, a.convention));
            }
        }

        public IEnumerable<IConventionPlaceResult> GetConventionPlaces(Guid conventionId)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var querie = from p in context.Places
                    where p.AssociatedConventionId == conventionId
                    join stagiaire in context.Stagiaires on p.StagiaireId equals stagiaire.StagiaireId
                    join societe in context.Societes on p.SocieteId equals societe.SocieteId
                    select new
                    {
                        StagiaireNom = stagiaire.Nom,
                        StagiairePrenom = stagiaire.Prenom,
                        SocieteNom = societe.Nom,
                        societe.Addresse,
                        societe.CodePostal,
                        societe.Ville,                        
                    };

                return querie.ToList().Select(a=>new ConventionPlaceResult(){ Societe = a.SocieteNom, Stagiaire = new NomComplet(a.StagiaireNom , a.StagiairePrenom), Adresse = a.Addresse, CodePostal = a.CodePostal, Ville = a.Ville});
            }
        }

        public IEnumerable<IPlaceValidatedResult> GetValidatedPlaces(Guid sessionId)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var querie = from p in context.Places
                    join convention in context.Conventions on p.AssociatedConventionId equals convention.ConventionId
                    join contact in context.Contacts on convention.ContactId equals contact.ContactId             
                    join stagiaire in context.Stagiaires on p.StagiaireId equals stagiaire.StagiaireId
                    join societe in context.Societes on p.SocieteId equals societe.SocieteId
                    where p.SessionId == sessionId && p.Status == PlaceStatus.Validé && convention.DocumentId.HasValue
                    select new
                    {
                        StagiaireNom = stagiaire.Nom,
                        StagiairePrenom = stagiaire.Prenom,
                        SocieteNom = societe.Nom,
                        ContactNom = contact.Nom,
                        ContactPrenom = contact.Prenom,
                        Telephone = contact.Telephone,
                        Email = contact.Email
                    };

                return querie.ToList().Select(a => new PlaceValidatedResult(a.StagiaireNom,  a.StagiairePrenom, a.SocieteNom, a.ContactNom, a.ContactPrenom, a.Telephone, a.Email));
            }
        }
    }
}
