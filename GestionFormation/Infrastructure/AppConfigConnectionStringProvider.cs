using System.Configuration;

namespace GestionFormation.Infrastructure
{
    public class AppConfigConnectionStringProvider : IConnectionStringProvider
    {
        public void Write(string connectionString)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.ConnectionStrings.ConnectionStrings["local"].ConnectionString = connectionString;
            config.Save(ConfigurationSaveMode.Modified, true);
            ConfigurationManager.RefreshSection("connectionStrings");
        }

        public string Read()
        {
            return ConfigurationManager.ConnectionStrings["local"].ConnectionString;
        }
    }
}