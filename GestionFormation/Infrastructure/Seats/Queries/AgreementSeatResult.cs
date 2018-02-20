using GestionFormation.CoreDomain;
using GestionFormation.CoreDomain.Seats.Queries;

namespace GestionFormation.Infrastructure.Seats.Queries
{
    public class AgreementSeatResult : IAgreementSeatResult
    {    
        public FullName Student { get; set; }
        public string Company { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
    }
}