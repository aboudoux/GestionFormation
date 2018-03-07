using System.Configuration;
using GestionFormation.Kernel;

namespace GestionFormation.Infrastructure
{
    public class AppConfigConfigurationFile : IConfigurationFile, IRuntimeDependency
    {
        public string GetCloseSessionEmail()
        {
            return ConfigurationManager.AppSettings["close_session_email"];
        }
    }
}