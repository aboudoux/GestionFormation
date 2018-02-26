namespace GestionFormation.CoreDomain.Sessions.Queries
{
    public interface ICompleteSessionResult : ISessionResult
    {
        string Training { get; }
        FullName Trainer { get; }         
        string Location { get; }
        int Color { get; }
    }
}