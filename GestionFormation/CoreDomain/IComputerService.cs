using System;
using System.Collections.Generic;
using GestionFormation.Infrastructure;

namespace GestionFormation.CoreDomain
{
    public interface IComputerService
    {
        void OpenMailInOutlook(string subject, string body, List<MailAttachement> attachements = null, string recipient = null);
        string GetLocalUserName();
        void Print(string documentPath);
    }
}