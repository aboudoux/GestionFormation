using System;

namespace GestionFormation.CoreDomain.Agreements.Queries
{
    public class PrintableAgreementResult : IPrintableAgreementResult
    {
        public string TrainingName { get; set; }
        public string AgreementNumber { get; set; }
        public AgreementType AgreementType { get; set; }
        public string Training { get; set; }
        public string Location { get; set; }
        public DateTime StartDate { get; set; }
        public int Duration { get; set; }
    }
}