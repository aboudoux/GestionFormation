using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GestionFormation.CoreDomain;
using GestionFormation.Kernel;

namespace GestionFormation.Infrastructure
{
    public class DocumentCreator : IDocumentCreator, IRuntimeDependency
    {
        private readonly string _templateDirectory;

        private const string CertificateOfAttendance = "CertificatAssiduite.rtf";
        private const string Degree = "Diplome.rtf";
        private const string Timesheet = "Presence.rtf";
        private const string Survey = "Questionnaire.rtf";
        private const string FreeAgreement = "ConventionG.rtf";
        private const string PaidAgreement = "ConventionP.rtf";
        private const string FirstPage = "FirstPage.rtf";

        public DocumentCreator()
        {
            _templateDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Templates");
        }

        public DocumentCreator(string templateDirectory)
        {
            _templateDirectory = Path.Combine(templateDirectory,"Templates");
        }
       
        public string CreateCertificateOfAttendance(FullName student, string company, string training, string location, int duration, FullName trainer, DateTime startSession)
        {
            return MakeDocument(CertificateOfAttendance)
                .Merge("$stagiaire$", student.ToString())
                .Merge("$societe$", company)
                .Merge("$formation$", training)
                .Merge("$lieu$", location)
                .Merge("$date$", startSession.ToString("d"))
                .Merge("$duree$", duration.ToString())
                .Merge("$formateur$", trainer.ToString())
                .Merge("$longdate$", DateTime.Now.ToString("D"))
                .Generate();            
        }

        public string CreateDegree(FullName student, string company, DateTime startSession, DateTime endSession, FullName trainer)
        {
            return MakeDocument(Degree)
                .Merge("$stagiaire$", student.ToString())
                .Merge("$societe$", company)
                .Merge("$datedebut$", startSession.ToString("d"))
                .Merge("$datefin$", endSession.ToString("d"))
                .Merge("$formateur$", trainer.ToString())
                .Merge("$date$", DateTime.Now.ToString("d"))
                .Generate();
        }

        public string CreateTimesheet(string training, DateTime startSession, int duration, string location, FullName trainer, IReadOnlyList<Attendee> participants)
        {
            if(participants == null)
                throw new ArgumentNullException(nameof(participants));
            if(!participants.Any())
                throw new Exception("Impossible de générer votre document car il n'y a aucun participant.");
            if(participants.Count() > 20)
                throw new Exception("Impossible de générer une feuille de présence car il y à plus de 20 participants");

            var document = MakeDocument(Timesheet)
                .Merge("$formation$", training)
                .Merge("$datedebut$", startSession.ToString("d"))
                .Merge("$duree$", duration.ToString())
                .Merge("$lieu$", location)
                .Merge("$formateur$", trainer.ToString())
                .Merge("$longdate$", DateTime.Now.ToString("D"));

            for (var i = 1; i <= 20; i++)
            {
                if (participants.Count >= i)
                    document.Merge($"$stagiaire{i}$", participants[i - 1].Student.ToString()).Merge($"$societe{i}$", participants[i-1].Company);
                else
                    document.Merge($"$stagiaire{i}$", string.Empty).Merge($"$societe{i}$", string.Empty);                        
            }
            return document.Generate();
        }

        public string CreateSurvey(FullName trainer, string training)
        {
            return MakeDocument(Survey)
                .Merge("$formateur$", trainer.ToString())
                .Merge("$formation$", training)
                .Merge("$date$", DateTime.Now.ToString("d"))
                .Generate();
        }

        public string CreateFreeAgreement(string agreementNumber, string company, string address, string zipCode, string city,
            FullName contact, string training, DateTime startSession, int duration, string location,
            IReadOnlyList<Attendee> attendees)
        {
            if (attendees == null)
                throw new ArgumentNullException(nameof(attendees));
            if (!attendees.Any())
                throw new Exception("Impossible de générer votre document car il n'y a aucun participant.");
            if (attendees.Count() > 8)
                throw new Exception("Impossible de générer une convention avec plus de 8 participants");

            var document = MakeDocument(FreeAgreement)
                .Merge("$numero$", agreementNumber)
                .Merge("$societe$", company)
                .Merge("$adresse$", address)
                .Merge("$cp$", zipCode)
                .Merge("$ville$", city)
                .Merge("$contact$", contact.ToString())
                .Merge("$formation$", training)
                .Merge("$datedebut$", startSession.ToString("d"))
                .Merge("$datefin$", startSession.AddDays(duration-1).ToString("d"))
                .Merge("$duree$", duration.ToString())
                .Merge("$lieu$", location)
                .Merge("$longdate$", DateTime.Now.ToString("D"));

            for (var i = 1; i <= 8; i++)
            {
                if (attendees.Count >= i)
                    document.Merge($"$stagiaire{i}$", attendees[i - 1].Student.ToString());
                else
                    document.Merge($"$stagiaire{i}$", string.Empty);
            }

            return document.Generate();
        }

        public string CreatePaidAgreement(string agreementNumber, string company, string address, string zipCode, string city,
            FullName contact, string training, DateTime startSession, int duration, string location,
            IReadOnlyList<Attendee> participants)
        {
            if (participants == null)
                throw new ArgumentNullException(nameof(participants));
            if (!participants.Any())
                throw new Exception("Impossible de générer votre document car il n'y a aucun participant.");
            if (participants.Count() > 8)
                throw new Exception("Impossible de générer une convention avec plus de 8 participants");

            var document = MakeDocument(PaidAgreement)
                .Merge("$numero$", agreementNumber)
                .Merge("$societe$", company)
                .Merge("$adresse$", address)
                .Merge("$cp$", zipCode)
                .Merge("$ville$", city)
                .Merge("$contact$", contact.ToString())
                .Merge("$formation$", training)
                .Merge("$datedebut$", startSession.ToString("d"))
                .Merge("$datefin$", startSession.AddDays(duration - 1).ToString("d"))
                .Merge("$duree$", duration.ToString())
                .Merge("$lieu$", location)
                .Merge("$prix$", (participants.Count() * 450 * duration).ToString())
                .Merge("$longdate$", DateTime.Now.ToString("D"));

            for (var i = 1; i <= 8; i++)
            {
                if (participants.Count >= i)
                    document.Merge($"$stagiaire{i}$", participants[i - 1].Student.ToString());
                else
                    document.Merge($"$stagiaire{i}$", string.Empty);
            }

            return document.Generate();
        }

        public string CreateFirstPage(string training, DateTime startSession, string company, FullName contact, string address, string zipCode, string city)
        {
            return MakeDocument(FirstPage)
                .Merge("$societe$", company)
                .Merge("$contact$", contact.ToString())
                .Merge("$adresse$", address)
                .Merge("$cp$", zipCode)
                .Merge("$ville$", city)
                .Merge("$date$", DateTime.Now.ToString("d"))
                .Merge("$formation$", training)
                .Merge("$longdate$", startSession.ToString("D"))
                .Generate();
        }

        private DocumentGenerator MakeDocument(string templateName)
        {
            return new DocumentGenerator(_templateDirectory, templateName);
        }

        private class DocumentGenerator
        {
            private readonly Encoding _encoding = Encoding.GetEncoding("ISO-8859-1");
            private readonly string _tempFile;
            private string _content;


            public DocumentGenerator(string templateDirectory, string templateName)
            {
                var template = Path.Combine(templateDirectory, templateName);
                if (!File.Exists(template))
                    throw new Exception($"Impossible de trouver le modèle '{template}'");

                _tempFile = GetRtfTempFileName();
                _content = File.ReadAllText(template, _encoding);
            }

            public DocumentGenerator Merge(string mergeField, string value)
            {
                _content = _content.Replace(mergeField, value);
                return this;
            }

            public string Generate()
            {
                File.WriteAllText(_tempFile, _content, _encoding);
                return _tempFile;
            }

            private static string GetRtfTempFileName()
            {
                return Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".rtf");
            }
        }
    }
}