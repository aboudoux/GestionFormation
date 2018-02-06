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

        private const string CertificatAssiduite = "CertificatAssiduite.rtf";
        private const string Diplome = "Diplome.rtf";
        private const string FeuillePresence = "Presence.rtf";
        private const string Questionnaire = "Questionnaire.rtf";
        private const string ConventionGratuite = "ConventionG.rtf";
        private const string ConventionPayante = "ConventionP.rtf";

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
            return MakeDocument(CertificatAssiduite)
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
            return MakeDocument(Diplome)
                .Merge("$stagiaire$", student.ToString())
                .Merge("$societe$", company)
                .Merge("$datedebut$", startSession.ToString("d"))
                .Merge("$datefin$", endSession.ToString("d"))
                .Merge("$formateur$", trainer.ToString())
                .Merge("$date$", DateTime.Now.ToString("d"))
                .Generate();
        }

        public string CreateTimesheet(string training, DateTime startSession, int duration, string location, FullName trainer, IReadOnlyList<Participant> participants)
        {
            if(participants == null)
                throw new ArgumentNullException(nameof(participants));
            if(!participants.Any())
                throw new Exception("Impossible de générer votre document car il n'y a aucun participant.");
            if(participants.Count() > 20)
                throw new Exception("Impossible de générer une feuille de présence car il y à plus de 20 participants");

            var document = MakeDocument(FeuillePresence)
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
            return MakeDocument(Questionnaire)
                .Merge("$formateur$", trainer.ToString())
                .Merge("$formation$", training)
                .Merge("$date$", DateTime.Now.ToString("d"))
                .Generate();
        }

        public string CreateFreeAgreement(string agreementNumber, string company, string address, string zipCode, string city,
            FullName contact, string training, DateTime startSession, int duration, string location,
            IReadOnlyList<Participant> participants)
        {
            if (participants == null)
                throw new ArgumentNullException(nameof(participants));
            if (!participants.Any())
                throw new Exception("Impossible de générer votre document car il n'y a aucun participant.");
            if (participants.Count() > 8)
                throw new Exception("Impossible de générer une convention avec plus de 8 participants");

            var document = MakeDocument(ConventionGratuite)
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
                if (participants.Count >= i)
                    document.Merge($"$stagiaire{i}$", participants[i - 1].Student.ToString());
                else
                    document.Merge($"$stagiaire{i}$", string.Empty);
            }

            return document.Generate();
        }

        public string CreatePaidAgreement(string agreementNumber, string company, string address, string zipCode, string city,
            FullName contact, string training, DateTime startSession, int duration, string location,
            IReadOnlyList<Participant> participants)
        {
            if (participants == null)
                throw new ArgumentNullException(nameof(participants));
            if (!participants.Any())
                throw new Exception("Impossible de générer votre document car il n'y a aucun participant.");
            if (participants.Count() > 8)
                throw new Exception("Impossible de générer une convention avec plus de 8 participants");

            var document = MakeDocument(ConventionPayante)
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