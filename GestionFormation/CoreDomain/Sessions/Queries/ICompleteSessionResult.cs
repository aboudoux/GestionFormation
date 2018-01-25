namespace GestionFormation.CoreDomain.Sessions.Queries
{
    public interface ICompleteSessionResult : ISessionResult
    {
        string Formation { get; }
        NomComplet Formateur { get; }         
        string Lieu { get; }
    }
}