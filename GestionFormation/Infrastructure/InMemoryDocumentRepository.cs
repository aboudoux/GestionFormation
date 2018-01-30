using System;
using System.Collections.Generic;
using GestionFormation.CoreDomain;

namespace GestionFormation.Infrastructure
{
    public class InMemoryDocumentRepository : IDocumentRepository
    {
        public Guid SaveConvention(string fileName)
        {
            return Guid.NewGuid();
        }

        public string CreateCertificatAssiduite(FullName stagiaire, string societe, string formation, string lieu, int durée,
            FullName formateur, DateTime dateSession)
        {
            throw new NotImplementedException();
        }

        public string CreateDiplome(FullName stagiaire, string societe, DateTime debutSession, DateTime finSession,
            FullName formateur)
        {
            throw new NotImplementedException();
        }

        public string CreateFeuillePresence(string formation, DateTime dateDebut, int durée, string lieu, FullName formateur,
            IReadOnlyList<Participant> participants)
        {
            throw new NotImplementedException();
        }

        public string CreateQuestionnaire(FullName formateur, string formation)
        {
            throw new NotImplementedException();
        }

        public string CreateConventionGratuite(string numero, string societe, string addresse, string codePostal, string ville,
            FullName contact, string formation, DateTime dateDebut, int durée, string lieu, IReadOnlyList<Participant> participants)
        {
            throw new NotImplementedException();
        }

        public string CreateConventionPayante(string numero, string societe, string addresse, string codePostal, string ville,
            FullName contact, string formation, DateTime dateDebut, int durée, string lieu, IReadOnlyList<Participant> participants)
        {
            throw new NotImplementedException();
        }
    }
}