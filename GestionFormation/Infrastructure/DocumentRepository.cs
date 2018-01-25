using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GestionFormation.CoreDomain;
using GestionFormation.Kernel;

namespace GestionFormation.Infrastructure
{
    public class DocumentRepository : IDocumentRepository, IRuntimeDependency
    {
        private readonly string _templateDirectory;

        private const string CertificatAssiduite = "CertificatAssiduite.rtf";
        private const string Diplome = "Diplome.rtf";
        private const string FeuillePresence = "Presence.rtf";
        private const string Questionnaire = "Questionnaire.rtf";

        public DocumentRepository()
        {
            _templateDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Templates");
        }

        public DocumentRepository(string templateDirectory)
        {
            _templateDirectory = Path.Combine(templateDirectory,"Templates");
        }

        public Guid SaveConvention(string fileName)
        {
            return Guid.NewGuid();
        }

        public string CreateCertificatAssiduite(NomComplet stagiaire, string societe, string formation, string lieu, int dur�e, NomComplet formateur, DateTime dateSession)
        {
            return MakeDocument(CertificatAssiduite)
                .Merge("$stagiaire$", stagiaire.ToString())
                .Merge("$societe$", societe)
                .Merge("$formation$", formation)
                .Merge("$lieu$", lieu)
                .Merge("$date$", dateSession.ToString("d"))
                .Merge("$duree$", dur�e.ToString())
                .Merge("$formateur$", formateur.ToString())
                .Merge("$longdate$", DateTime.Now.ToString("D"))
                .Generate();            
        }

        public string CreateDiplome(NomComplet stagiaire, string societe, DateTime debutSession, DateTime finSession, NomComplet formateur)
        {
            return MakeDocument(Diplome)
                .Merge("$stagiaire$", stagiaire.ToString())
                .Merge("$societe$", societe)
                .Merge("$datedebut$", debutSession.ToString("d"))
                .Merge("$datefin$", finSession.ToString("d"))
                .Merge("$formateur$", formateur.ToString())
                .Merge("$date$", DateTime.Now.ToString("d"))
                .Generate();
        }

        public string CreateFeuillePresence(string formation, DateTime dateDebut, int dur�e, string lieu, NomComplet formateur, IReadOnlyList<Participant> participants)
        {
            if(participants == null)
                throw new ArgumentNullException(nameof(participants));
            if(!participants.Any())
                throw new Exception("Impossible de g�n�rer votre document car il n'y a aucun participant.");
            if(participants.Count() > 20)
                throw new Exception("Impossible de g�n�rer une feuille de pr�sence car il y � plus de 20 participants");

            var document = MakeDocument(FeuillePresence)
                .Merge("$formation$", formation)
                .Merge("$datedebut$", dateDebut.ToString("d"))
                .Merge("$duree$", dur�e.ToString())
                .Merge("$lieu$", lieu)
                .Merge("$formateur$", formateur.ToString())
                .Merge("$longdate$", DateTime.Now.ToString("D"));

            for (var i = 1; i <= 20; i++)
            {
                if (participants.Count >= i)
                    document.Merge($"$stagiaire{i}$", participants[i - 1].Stagiaire.ToString()).Merge($"$societe{i}$", participants[i-1].Societe);
                else
                    document.Merge($"$stagiaire{i}$", string.Empty).Merge($"$societe{i}$", string.Empty);
                        
            }
            return document.Generate();
        }

        public string CreateQuestionnaire(NomComplet formateur, string formation)
        {
            return MakeDocument(Questionnaire)
                .Merge("$formateur$", formateur.ToString())
                .Merge("$formation$", formation)
                .Merge("$date$", DateTime.Now.ToString("d"))
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
                    throw new Exception($"Impossible de trouver le mod�le '{template}'");

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