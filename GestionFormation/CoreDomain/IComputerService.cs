using System.Collections.Generic;

namespace GestionFormation.CoreDomain
{
    public interface IComputerService
    {
        void OpenMailInOutlook(string subject, string body, List<MailAttachement> attachements = null, string recipient = null);

        string GetLocalUserName();
    }
}