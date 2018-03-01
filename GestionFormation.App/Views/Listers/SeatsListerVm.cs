using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GestionFormation.App.Core;
using GestionFormation.App.Views.Listers.Bases;
using GestionFormation.CoreDomain;
using GestionFormation.CoreDomain.Seats;
using GestionFormation.CoreDomain.Seats.Queries;

namespace GestionFormation.App.Views.Listers
{
    public class SeatsListerVm : ListerWindowVm<SeatListerItem>
    {
        private readonly ISeatQueries _seatQueries;
        public override string Title => "Liste des places";

        public SeatsListerVm(ISeatQueries seatQueries, IApplicationService applicationService) : base(applicationService)
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
        public SeatListerItem(IListSeat seat)
        {
            Company = seat.Company;
            Student = new FullName(seat.StudentLastname, seat.StudentFirstname);
            Trainer = new FullName(seat.TrainerLastname, seat.TrainerFirstname);
            Training = seat.Training;
            StartSession = seat.SessionStart;
            Location = seat.Duration;
            AgreementNumber = seat.AgreementNumber;
            Contact = new FullName(seat.ContactLastname, seat.Contactfirstname);
            Telephone = seat.Telephone;
            Email = seat.Email;
            SeatState = GetSeatStatus(seat.SeatStatus);
        }

        private string GetSeatStatus(SeatStatus status)
        {
            switch (status)
            {
                case SeatStatus.Valid: return "Validé";
                case SeatStatus.ToValidate: return "A valider";
                case SeatStatus.Canceled: return "Annulé";
                case SeatStatus.Refused: return "Refusé";
                    default: return "Inconnu";
            }
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