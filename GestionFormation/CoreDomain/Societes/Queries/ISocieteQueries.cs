using System;
using System.Collections.Generic;
using System.Text;

namespace GestionFormation.CoreDomain.Societes.Queries
{
    public interface ISocieteQueries
    {
        IEnumerable<ISocieteResult> GetAll();
        bool Exists(Guid societeId);
    }
}
