using System.Collections.Generic;
using System.Text;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Formateurs.Queries
{
    public interface IFormateurQueries
    {
        IReadOnlyList<IFormateurResult> GetAll();
    }
}
