namespace GestionFormation.CoreDomain
{
    public interface IComputerService
    {
        void OpenTypeConventionMail(string subject, string body);

        string GetLocalUserName();

    }
}