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

        string CreateConventionGratuite(string numero, string societe, string addresse, string codePostal, string ville, NomComplet contact, string formation, DateTime dateDebut, int durée, string lieu, IReadOnlyList<Participant> participants);

        string CreateConventionPayante(string numero, string societe, string addresse, string codePostal, string ville, NomComplet contact, string formation, DateTime dateDebut,  int durée, string lieu, IReadOnlyList<Participant> participants);
    }

    public class Participant
    {
        public Participant(NomComplet stagiaire, string societe)
        {
            Stagiaire = stagiaire;
            Societe = societe;
        }
        public NomComplet Stagiaire { get; }
        public string Societe { get; }
    }
}