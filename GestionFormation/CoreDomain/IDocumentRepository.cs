using System;
using System.Collections.Generic;
using System.Globalization;

namespace GestionFormation.CoreDomain
{
    public interface IDocumentRepository
    {
        Guid SaveConvention(string fileName);

        string CreateCertificatAssiduite(NomComplet stagiaire, string societe, string formation, string lieu, int durée, NomComplet formateur, DateTime dateSession);
        string CreateDiplome(NomComplet stagiaire, string societe, DateTime debutSession, DateTime finSession, NomComplet formateur);

        string CreateFeuillePresence(string formation, DateTime dateDebut, int durée, string lieu, NomComplet formateur, IReadOnlyList<Participant> participants);

        string CreateQuestionnaire(NomComplet formateur, string formation);

    }

    public class Participant
    {
        public Participant(string nom, string prenom, string societe)
        {
            Stagiaire = new NomComplet(nom, prenom);
            Societe = societe;
        }
        public NomComplet Stagiaire { get; }
        public string Societe { get; }
    }
}