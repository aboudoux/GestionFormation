using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionFormation.Infrastructure.Locations.Projections
{
    [Table("Location")]
    public class LocationSqlEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public bool Enabled { get; set; }
        public int Seats { get; set; }
    }
}