namespace GestionFormation.CoreDomain.Seats.Queries
{
    public class SeatValidatedResult : ISeatValidatedResult
    {
        public SeatValidatedResult(string traineeLastname, string traineeFirstname, string company, string contactLastName, string contactFirstname, string telephone, string email)
        {
            Trainee = new FullName(traineeLastname, traineeFirstname);
            Company = company;
            Contact = new FullName(contactLastName, contactFirstname);
            Telephone = telephone;
            Email = email;
        }
        public FullName Trainee { get; }
        public string Company { get; }
        public FullName Contact { get; }
        public string Telephone { get; }
        public string Email { get; }
    }
}