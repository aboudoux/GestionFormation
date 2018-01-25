namespace GestionFormation.CoreDomain.Sessions.Queries
{
    public interface ICompleteSessionResult : ISessionResult
    {
        string Formation { get; }
        string Formateur { get; } 
        string Lieu { get; }
    }
}