using System;
using GestionFormation.CoreDomain.Seats.Queries;

namespace GestionFormation.App.Views.Seats
{
    public class AgreementState
    {
        public AgreementState(ISeatResult result)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            if (result.AgreementId.HasValue)
            
                Label = (result.AgreementSigned ? "Signée" : "Attente de signature");
            else
                Label = result.AgreementRevoked ? "Révoquée" : "Non générée";
        }

        public string Label { get; }

        public string Icon
        {
            get
            {
                var iconPath = "/Images/Etats/Conventions/";
                switch (Label)
                {
                    case "Révoquée": return iconPath + "revoked.png";
                    case "Signée": return iconPath + "signed.png";
                    case "Attente de signature": return iconPath + "signature_pending.png";
                    case "Non générée": return iconPath + "to_create.png";

                    default: throw new Exception($"le statut {Label} est introuvable.");
                }
            }
        }
    }
}