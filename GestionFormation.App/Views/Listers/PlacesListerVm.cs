using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GestionFormation.CoreDomain;
using GestionFormation.CoreDomain.Seats.Queries;

namespace GestionFormation.App.Views.Listers
{
    public class PlacesListerVm : ListerWindowVm<PlaceListerItem>
    {
        private readonly ISeatQueries _seatQueries;
        public override string Title => "Liste des places";

        public PlacesListerVm(ISeatQueries seatQueries)
        {
            _seatQueries = seatQueries ?? throw new ArgumentNullException(nameof(seatQueries));
        }

        protected override async Task<IEnumerable<PlaceListerItem>> LoadAsync()
        {
            return await Task.Run(() => _seatQueries.GetSeatsList().Select(a => new PlaceListerItem(a)));
        }
    }

    public class PlaceListerItem
    {
        public PlaceListerItem(IListSeat place)
        {
            Societe = place.Company;
            Stagiaire = new FullName(place.TraineeLastname, place.TraineeFirstname);
            Formateur = new FullName(place.TrainerLastname, place.TrainerFirstname);
            Formation = place.Training;
            DateDebut = place.SessionStart;
            Durée = place.Duration;
            NumeroConvention = place.AgreementNumber;
            Contact = new FullName(place.ContactLastname, place.Contactfirstname);
            Telephone = place.Telephone;
            Email = place.Email;
        }

        [DisplayName("Société")]
        public string Societe { get; }
        public FullName Stagiaire { get; }
        public FullName Formateur { get; }
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

        public FullName Contact { get; }
        public string Email { get; }
        [DisplayName("Téléphone")]
        public string Telephone { get; }
        
    }
}