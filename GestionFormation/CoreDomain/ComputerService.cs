using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain
{
    public class ComputerService : IComputerService, IRuntimeDependency
    {
        public void OpenTypeConventionMail(string subject, string body)
        {
            var app = new Microsoft.Office.Interop.Outlook.Application();
            var mailItem = (Microsoft.Office.Interop.Outlook._MailItem)app.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem);
            mailItem.Subject = subject;
            mailItem.Body = body;

            mailItem.Display(false);
        }

        public string GetLocalUserName()
        {
            return Environment.UserName;
        }
    }
}