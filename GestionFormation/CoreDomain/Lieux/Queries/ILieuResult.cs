using System;

namespace GestionFormation.CoreDomain.Lieux.Queries
{
    public interface ILieuResult
    {
        Guid LieuId { get; }
        string Nom { get; }
        string Addresse { get; }
        int Places { get; }
    }
}
