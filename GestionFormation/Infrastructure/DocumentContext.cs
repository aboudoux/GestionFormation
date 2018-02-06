using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace GestionFormation.Infrastructure
{
    public class DocumentContext : DbContext
    {
        public DocumentContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }

        public DbSet<Document> Documents { get; set; }
    }

    [Table("Document")]
    public class Document
    {
        [Key]
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public byte[] Data { get; set; }
    }
}