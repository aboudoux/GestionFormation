using System;

namespace GestionFormation.CoreDomain
{
    public interface IDocumentRepository
    {
        Guid Save(string filePath);
        string GetDocument(Guid documentId);
    }
}