using System;

namespace GestionFormation.CoreDomain.Conventions.Queries
{
    public interface IPrintableConventionResult
    {
        string NumeroConvention { get; }
        TypeConvention TypeConvention { get; }
        string Formation { get; }
        string Lieu { get; }
        DateTime DateDebut { get; }
        int Durée { get; }
    }
}