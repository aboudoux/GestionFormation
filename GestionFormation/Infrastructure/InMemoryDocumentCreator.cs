using System;
using System.Collections.Generic;
using GestionFormation.CoreDomain;

namespace GestionFormation.Infrastructure
{
    public class InMemoryDocumentCreator : IDocumentCreator
    {
        public Guid SaveAgreement(string fileName)
        {
            return Guid.NewGuid();
        }

        public string CreateCertificateOfAttendance(FullName student, string company, string training, string location, int duration,
            FullName trainer, DateTime startSession)
        {
            throw new NotImplementedException();
        }

        public string CreateDegree(FullName student, string company, DateTime startSession, DateTime endSession,
            FullName trainer)
        {
            throw new NotImplementedException();
        }

        public string CreateTimesheet(string training, DateTime startSession, int duration, string location, FullName trainer,
            IReadOnlyList<Participant> participants)
        {
            throw new NotImplementedException();
        }

        public string CreateSurvey(FullName trainer, string training)
        {
            throw new NotImplementedException();
        }

        public string CreateFreeAgreement(string agreementNumber, string company, string address, string zipCode, string city,
            FullName contact, string training, DateTime startSession, int duration, string location, IReadOnlyList<Participant> participants)
        {
            throw new NotImplementedException();
        }

        public string CreatePaidAgreement(string agreementNumber, string company, string address, string zipCode, string city,
            FullName contact, string training, DateTime startSession, int duration, string location, IReadOnlyList<Participant> participants)
        {
            throw new NotImplementedException();
        }
    }
}