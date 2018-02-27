using System;
using GestionFormation.CoreDomain.Seats;

namespace GestionFormation.App.Views.Seats
{
    public class SeatState
    {
        private SeatStatus Statut;
        public SeatState(SeatStatus status)
        {
            Statut = status;
        }

        public string Label
        {
            get
            {
                switch (Statut)
                {
                    case SeatStatus.ToValidate: return "Attente de validation";
                    case SeatStatus.Canceled: return "Annulée";
                    case SeatStatus.Refused: return "Refusée";
                    case SeatStatus.Valid: return "Validée";
                    default: throw new Exception($"Le statut {Statut} est introuvable");
                }
            }
        }

        public string Icon
        {
            get
            {
                var iconPath = "/Images/Etats/Places/";
                switch (Statut)
                {
                    case SeatStatus.ToValidate: return iconPath + "pending_validation.png";
                    case SeatStatus.Canceled: return iconPath + "canceled.png";
                    case SeatStatus.Refused: return iconPath + "refused.png";
                    case SeatStatus.Valid: return iconPath + "validated.png";
                    default: throw new Exception($"Le statut {Statut} est introuvable");
                }
            }
        }
    }
}