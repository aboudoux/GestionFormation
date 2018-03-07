using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using GestionFormation.CoreDomain;
using GestionFormation.Kernel;
using Microsoft.Office.Interop.Outlook;

namespace GestionFormation.Infrastructure
{
    public class ComputerService : IComputerService, IRuntimeDependency
    {
        public void OpenMailInOutlook(string subject, string body, List<MailAttachement> attachements = null, string recipient = null, string bccRecipient = null)
        {
            var app = new Application();
            var mailItem = (_MailItem)app.CreateItem(OlItemType.olMailItem);
            mailItem.Subject = subject;
            mailItem.Body = body;
            mailItem.To = recipient;
            mailItem.BCC = bccRecipient;

            if (attachements != null)
                foreach (var attachement in attachements)
                    mailItem.Attachments.Add(attachement.FilePath, OlAttachmentType.olByValue,1, DisplayName: attachement.DisplayName);

            mailItem.Display(false);
        }

        public string GetLocalUserName()
        {
            return Environment.UserName;
        }

        public void Print(string documentPath)
        {            
            Process.Start("CMD", $"/C PRINT \"{documentPath}\"");
        }
    }

    public class MailAttachement
    {
        public string FilePath { get; }
        public string DisplayName { get; }

        public MailAttachement(string filePath, string displayName)
        {
            if(!File.Exists(filePath))
                throw new FileNotFoundException(filePath);

            var tempDir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));
            FilePath = Path.Combine(tempDir.FullName, displayName) + Path.GetExtension(filePath);
            DisplayName = displayName;

            File.Copy(filePath, FilePath );            
        }
    }
}