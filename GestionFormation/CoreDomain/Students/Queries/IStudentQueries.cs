using System.Collections.Generic;

namespace GestionFormation.CoreDomain.Students.Queries
{
    public interface IStudentQueries
    {
        IReadOnlyList<IStudentResult> GetAll();
    }
}