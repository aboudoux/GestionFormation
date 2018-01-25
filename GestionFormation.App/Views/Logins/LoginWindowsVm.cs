using System;
using System.Threading.Tasks;
using GestionFormation.App.Core;
using GestionFormation.Applications.Utilisateurs;
using GestionFormation.CoreDomain.Utilisateurs;
using GestionFormation.CoreDomain.Utilisateurs.Queries;

namespace GestionFormation.App.Views.Logins
{
    public class LoginWindowsVm : PopupWindowVm
    {
        private readonly IApplicationService _applicationService;
        private readonly IUtilisateurQueries _utilisateurQueries;
        private string _username;
        private string _password;

        public LoginWindowsVm(IApplicationService applicationService, IUtilisateurQueries utilisateurQueries)
        {
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
            _utilisateurQueries = utilisateurQueries ?? throw new ArgumentNullException(nameof(utilisateurQueries));  
            
            SetValiderCommandCanExecute(()=>!string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password));
        }
        
        public string Username
        {
            get => _username;
            set
            {
                Set(()=>Username, ref _username, value);
                ValiderCommand.RaiseCanExecuteChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                Set(()=>Password, ref _password, value);
                ValiderCommand.RaiseCanExecuteChanged();
            }
        }

        protected override async Task ExecuteValiderAsync()
        {            
            if (!_utilisateurQueries.Exists("admin"))
                await Task.Run(()=> _applicationService.Command<CreateUtilisateur>().Execute("admin", "1234", "Administrateur", string.Empty, string.Empty, UtilisateurRole.Admin));

            var command = new Logon(_utilisateurQueries);
            await HandleMessageBoxError.Execute(async () =>
            {
                var loggedUser = await Task.Run(() => command.Execute(Username, Password));
                Bootstrapper.SetLoggedUser(loggedUser);
                await base.ExecuteValiderAsync();
            });
        }        
    }
}