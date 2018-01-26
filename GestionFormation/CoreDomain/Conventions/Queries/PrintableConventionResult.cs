using System;

namespace GestionFormation.CoreDomain.Conventions.Queries
{
    public class PrintableConventionResult : IPrintableConventionResult
    {        
        public string NumeroConvention { get; set; }
        public TypeConvention TypeConvention { get; set; }
        public string Formation { get; set; }
        public string Lieu { get; set; }
        public DateTime DateDebut { get; set; }
        public int Durée { get; set; }
    }
}