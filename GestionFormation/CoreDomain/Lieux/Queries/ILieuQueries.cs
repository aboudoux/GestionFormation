using System;
using System.Collections.Generic;

namespace GestionFormation.CoreDomain.Lieux.Queries
{
    public interface ILieuQueries
    {
        IReadOnlyList<ILieuResult> GetAll();

        Guid? GetLieu(string nom);
    }
}