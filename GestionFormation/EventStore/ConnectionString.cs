namespace GestionFormation.EventStore
{
    public static class ConnectionString
    {
        //public static string Get() => "Data Source=158.138.54.60;Initial Catalog=GestionFormation;User ID=sa;Password=enterprise1+";

        public static string Get() => "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=GestionFormation;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
    }
}