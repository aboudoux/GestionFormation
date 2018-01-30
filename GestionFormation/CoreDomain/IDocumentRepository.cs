using System;
using System.Collections.Generic;
using System.Globalization;

namespace GestionFormation.CoreDomain
{
    public interface IDocumentRepository
    {
        Guid SaveConvention(string fileName);

        string CreateCertificatAssiduite(FullName stagiaire, string societe, string formation, string lieu, int durée, FullName formateur, DateTime dateSession);

        string CreateDiplome(FullName stagiaire, string societe, DateTime debutSession, DateTime finSession, FullName formateur);

        string CreateFeuillePresence(string formation, DateTime dateDebut, int durée, string lieu, FullName formateur, IReadOnlyList<Participant> participants);

        string CreateQuestionnaire(FullName formateur, string formation);

        string CreateConventionGratuite(string numero, string societe, string addresse, string codePostal, string ville, FullName contact, string formation, DateTime dateDebut, int durée, string lieu, IReadOnlyList<Participant> participants);

        string CreateConventionPayante(string numero, string societe, string addresse, string codePostal, string ville, FullName contact, string formation, DateTime dateDebut,  int durée, string lieu, IReadOnlyList<Participant> participants);
    }

    public class Participant
    {
        public Participant(FullName stagiaire, string societe)
        {
            Stagiaire = stagiaire;
            Societe = societe;
        }
        public FullName Stagiaire { get; }
        public string Societe { get; }
    }
}