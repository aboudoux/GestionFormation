using System;
using System.Threading.Tasks;
using System.Windows;
using GestionFormation.App.Core;
using GestionFormation.Applications.Utilisateurs;
using GestionFormation.CoreDomain;
using GestionFormation.CoreDomain.Utilisateurs;
using GestionFormation.CoreDomain.Utilisateurs.Queries;

namespace GestionFormation.App.Views.Logins
{

    public class LoginWindowsVm : PopupWindowVm
    {       
        private readonly IApplicationService _applicationService;
        private readonly IUtilisateurQueries _utilisateurQueries;
        private readonly IComputerService _computerService;
        private string _username;
        private string _password;
        private bool _connecting;

        public LoginWindowsVm(IApplicationService applicationService, IUtilisateurQueries utilisateurQueries, IComputerService computerService)
        {
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
            _utilisateurQueries = utilisateurQueries ?? throw new ArgumentNullException(nameof(utilisateurQueries));
            _computerService = computerService ?? throw new ArgumentNullException(nameof(computerService));

            SetValiderCommandCanExecute(()=>!string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password) && !Connecting);
        }


        public override Task Init()
        {
            Username = _computerService.GetLocalUserName();
            return base.Init();
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

        public bool Connecting
        {
            get => _connecting;
            set
            {
                Set(() => Connecting, ref _connecting, value);
                ValiderCommand.RaiseCanExecuteChanged();
            }
        }

        protected override async Task ExecuteValiderAsync()
        {
            try
            {
                Connecting = true;                            
                await HandleMessageBoxError.ExecuteAsync(async () =>
                {
                    if (!_utilisateurQueries.Exists("admin"))
                        await Task.Run(() => _applicationService.Command<CreateUtilisateur>().Execute("admin", "1234", "Administrateur", string.Empty, string.Empty, UtilisateurRole.Admin));

                    var command = new Logon(_utilisateurQueries);

                    var loggedUser = await Task.Run(() => command.Execute(Username, Password));
                    Bootstrapper.SetLoggedUser(loggedUser);
                    await base.ExecuteValiderAsync();
                });
            }
            finally
            {
                Connecting = false;
            }
        }        
    }
}