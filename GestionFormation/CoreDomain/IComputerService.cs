using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain
{
    public interface IComputerService
    {
        void OpenTypeConventionMail();
    }

    public class ComputerService : IComputerService, IRuntimeDependency
    {
        public void OpenTypeConventionMail()
        {            
        }
    }
}