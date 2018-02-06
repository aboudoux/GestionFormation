using System;
using System.IO;
using GestionFormation.CoreDomain;
using GestionFormation.EventStore;
using GestionFormation.Kernel;

namespace GestionFormation.Infrastructure
{
    public class DocumentRepository : IDocumentRepository, IRuntimeDependency
    {
        public Guid Save(string filePath)
        {
            if(!File.Exists(filePath))
                throw new FileNotFoundException(filePath);

            var data = File.ReadAllBytes(filePath);

            using (var context = new DocumentContext(ConnectionString.Get()))
            {
                var document = new Document();
                document.Id = Guid.NewGuid();
                document.FileName = Path.GetFileName(filePath);
                document.Data = data;
                context.Documents.Add(document);
                context.SaveChanges();

                return document.Id;
            }
        }

        public string GetDocument(Guid documentId)
        {
            using (var context = new DocumentContext(ConnectionString.Get()))
            {
                var document = context.Documents.Find(documentId);
                if (document == null)
                    throw new EntityNotFoundException(documentId, "Document");

                var dir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));
                var file = Path.Combine(dir.FullName, document.FileName);
                File.WriteAllBytes(file, document.Data);
                return file;
            }
        }
    }
}