using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionFormation.CoreDomain.Companies.Projections
{
    [Table("Company")]
    public class CompanySqlEntity
    {
        [Key]
        public Guid CompanyId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public bool Removed { get; set; }
    }
}