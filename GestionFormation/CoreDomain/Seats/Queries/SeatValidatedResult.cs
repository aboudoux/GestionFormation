namespace GestionFormation.CoreDomain.Seats.Queries
{
    public class SeatValidatedResult : ISeatValidatedResult
    {
        public SeatValidatedResult(string studentLastname, string studentFirstname, string company, string contactLastName, string contactFirstname, string telephone, string email)
        {
            Student = new FullName(studentLastname, studentFirstname);
            Company = company;
            Contact = new FullName(contactLastName, contactFirstname);
            Telephone = telephone;
            Email = email;
        }
        public FullName Student { get; }
        public string Company { get; }
        public FullName Contact { get; }
        public string Telephone { get; }
        public string Email { get; }
    }
}