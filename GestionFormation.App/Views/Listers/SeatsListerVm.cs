using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GestionFormation.CoreDomain;
using GestionFormation.CoreDomain.Seats.Queries;

namespace GestionFormation.App.Views.Listers
{
    public class SeatsListerVm : ListerWindowVm<SeatListerItem>
    {
        private readonly ISeatQueries _seatQueries;
        public override string Title => "Liste des places";

        public SeatsListerVm(ISeatQueries seatQueries)
        {
            _seatQueries = seatQueries ?? throw new ArgumentNullException(nameof(seatQueries));
        }

        protected override async Task<IEnumerable<SeatListerItem>> LoadAsync()
        {
            return await Task.Run(() => _seatQueries.GetSeatsList().Select(a => new SeatListerItem(a)));
        }
    }

    public class SeatListerItem
    {
        public SeatListerItem(IListSeat place)
        {
            Company = place.Company;
            Student = new FullName(place.StudentLastname, place.StudentFirstname);
            Trainer = new FullName(place.TrainerLastname, place.TrainerFirstname);
            Training = place.Training;
            StartSession = place.SessionStart;
            Location = place.Duration;
            AgreementNumber = place.AgreementNumber;
            Contact = new FullName(place.ContactLastname, place.Contactfirstname);
            Telephone = place.Telephone;
            Email = place.Email;
        }

        [DisplayName("Société")]
        public string Company { get; }
        [DisplayName("Stagiaire")]
        public FullName Student { get; }
        [DisplayName("Formateur")]
        public FullName Trainer { get; }
        [DisplayName("Formation")]
        public string Training { get; }
        [DisplayName("Début")]
        public DateTime StartSession { get; }
        [DisplayName("Durée")]
        public int Location { get; }

        [DisplayName("Statut")]
        public string SeatState { get; }

        [DisplayName("Convention")]
        public string AgreementNumber { get; }
        [DisplayName("Etat")]
        public string AgreementState { get; }

        [DisplayName("Contact")]
        public FullName Contact { get; }
        [DisplayName("Email")]
        public string Email { get; }
        [DisplayName("Téléphone")]
        public string Telephone { get; }        
    }
}