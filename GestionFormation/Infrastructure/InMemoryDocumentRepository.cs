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

        public string CreateCertificatAssiduite(NomComplet stagiaire, string societe, string formation, string lieu, int durée, NomComplet formateur, DateTime dateSession)
        {
            throw new NotImplementedException();
        }

        public string CreateDiplome(NomComplet stagiaire, string societe, DateTime debutSession, DateTime finSession, NomComplet formateur)
        {
            throw new NotImplementedException();
        }

        public string CreateFeuillePresence(string formation, DateTime dateDebut, int durée, string lieu, NomComplet formateur, IReadOnlyList<Participant> participants)
        {
            throw new NotImplementedException();
        }

        public string CreateQuestionnaire(NomComplet formateur, string formation)
        {
            throw new NotImplementedException();
        }
    }
}