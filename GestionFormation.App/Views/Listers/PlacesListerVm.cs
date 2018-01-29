using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GestionFormation.CoreDomain;
using GestionFormation.CoreDomain.Places.Queries;

namespace GestionFormation.App.Views.Listers
{
    public class PlacesListerVm : ListerWindowVm<PlaceListerItem>
    {
        private readonly IPlacesQueries _placesQueries;
        public override string Title => "Liste des places";

        public PlacesListerVm(IPlacesQueries placesQueries)
        {
            _placesQueries = placesQueries ?? throw new ArgumentNullException(nameof(placesQueries));
        }

        protected override async Task<IEnumerable<PlaceListerItem>> LoadAsync()
        {
            return await Task.Run(() => _placesQueries.GetPlacesList().Select(a => new PlaceListerItem(a)));
        }
    }

    public class PlaceListerItem
    {
        public PlaceListerItem(IListePlace place)
        {
            Societe = place.Societe;
            Stagiaire = new NomComplet(place.StagiaireNom, place.StagiairePrenom);
            Formateur = new NomComplet(place.FormateurNom, place.FormateurPrenom);
            Formation = place.Formation;
            DateDebut = place.DateDebut;
            Durée = place.Duree;
            NumeroConvention = place.NumeroConvention;
            Contact = new NomComplet(place.ContactNom, place.ContactPrenom);
            Telephone = place.Telephone;
            Email = place.Email;
        }

        [DisplayName("Société")]
        public string Societe { get; }
        public NomComplet Stagiaire { get; }
        public NomComplet Formateur { get; }
        public string Formation { get; }
        [DisplayName("Début")]
        public DateTime DateDebut { get; }
        public int Durée { get; }

        [DisplayName("Statut")]
        public string EtatPlace { get; }

        [DisplayName("Convention")]
        public string NumeroConvention { get; }
        [DisplayName("Etat")]
        public string EtatConvention { get; }

        public NomComplet Contact { get; }
        public string Email { get; }
        [DisplayName("Téléphone")]
        public string Telephone { get; }
        
    }
}