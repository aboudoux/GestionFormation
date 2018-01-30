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
        private const string ConventionGratuite = "ConventionG.rtf";
        private const string ConventionPayante = "ConventionP.rtf";

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

        public string CreateCertificatAssiduite(FullName stagiaire, string societe, string formation, string lieu, int durée, FullName formateur, DateTime dateSession)
        {
            return MakeDocument(CertificatAssiduite)
                .Merge("$stagiaire$", stagiaire.ToString())
                .Merge("$societe$", societe)
                .Merge("$formation$", formation)
                .Merge("$lieu$", lieu)
                .Merge("$date$", dateSession.ToString("d"))
                .Merge("$duree$", durée.ToString())
                .Merge("$formateur$", formateur.ToString())
                .Merge("$longdate$", DateTime.Now.ToString("D"))
                .Generate();            
        }

        public string CreateDiplome(FullName stagiaire, string societe, DateTime debutSession, DateTime finSession, FullName formateur)
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

        public string CreateFeuillePresence(string formation, DateTime dateDebut, int durée, string lieu, FullName formateur, IReadOnlyList<Participant> participants)
        {
            if(participants == null)
                throw new ArgumentNullException(nameof(participants));
            if(!participants.Any())
                throw new Exception("Impossible de générer votre document car il n'y a aucun participant.");
            if(participants.Count() > 20)
                throw new Exception("Impossible de générer une feuille de présence car il y à plus de 20 participants");

            var document = MakeDocument(FeuillePresence)
                .Merge("$formation$", formation)
                .Merge("$datedebut$", dateDebut.ToString("d"))
                .Merge("$duree$", durée.ToString())
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

        public string CreateQuestionnaire(FullName formateur, string formation)
        {
            return MakeDocument(Questionnaire)
                .Merge("$formateur$", formateur.ToString())
                .Merge("$formation$", formation)
                .Merge("$date$", DateTime.Now.ToString("d"))
                .Generate();
        }

        public string CreateConventionGratuite(string numero, string societe, string addresse, string codePostal, string ville,
            FullName contact, string formation, DateTime dateDebut, int durée, string lieu,
            IReadOnlyList<Participant> participants)
        {
            if (participants == null)
                throw new ArgumentNullException(nameof(participants));
            if (!participants.Any())
                throw new Exception("Impossible de générer votre document car il n'y a aucun participant.");
            if (participants.Count() > 8)
                throw new Exception("Impossible de générer une convention avec plus de 8 participants");

            var document = MakeDocument(ConventionGratuite)
                .Merge("$numero$", numero)
                .Merge("$societe$", societe)
                .Merge("$adresse$", addresse)
                .Merge("$cp$", codePostal)
                .Merge("$ville$", ville)
                .Merge("$contact$", contact.ToString())
                .Merge("$formation$", formation)
                .Merge("$datedebut$", dateDebut.ToString("d"))
                .Merge("$datefin$", dateDebut.AddDays(durée-1).ToString("d"))
                .Merge("$duree$", durée.ToString())
                .Merge("$lieu$", lieu)
                .Merge("$longdate$", DateTime.Now.ToString("D"));

            for (var i = 1; i <= 8; i++)
            {
                if (participants.Count >= i)
                    document.Merge($"$stagiaire{i}$", participants[i - 1].Stagiaire.ToString());
                else
                    document.Merge($"$stagiaire{i}$", string.Empty);
            }

            return document.Generate();
        }

        public string CreateConventionPayante(string numero, string societe, string addresse, string codePostal, string ville,
            FullName contact, string formation, DateTime dateDebut, int durée, string lieu,
            IReadOnlyList<Participant> participants)
        {
            if (participants == null)
                throw new ArgumentNullException(nameof(participants));
            if (!participants.Any())
                throw new Exception("Impossible de générer votre document car il n'y a aucun participant.");
            if (participants.Count() > 8)
                throw new Exception("Impossible de générer une convention avec plus de 8 participants");

            var document = MakeDocument(ConventionPayante)
                .Merge("$numero$", numero)
                .Merge("$societe$", societe)
                .Merge("$adresse$", addresse)
                .Merge("$cp$", codePostal)
                .Merge("$ville$", ville)
                .Merge("$contact$", contact.ToString())
                .Merge("$formation$", formation)
                .Merge("$datedebut$", dateDebut.ToString("d"))
                .Merge("$datefin$", dateDebut.AddDays(durée - 1).ToString("d"))
                .Merge("$duree$", durée.ToString())
                .Merge("$lieu$", lieu)
                .Merge("$prix$", (participants.Count() * 450 * durée).ToString())
                .Merge("$longdate$", DateTime.Now.ToString("D"));

            for (var i = 1; i <= 8; i++)
            {
                if (participants.Count >= i)
                    document.Merge($"$stagiaire{i}$", participants[i - 1].Stagiaire.ToString());
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