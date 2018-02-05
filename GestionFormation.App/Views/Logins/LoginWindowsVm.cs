using System;
using System.Threading.Tasks;
using System.Windows;
using GestionFormation.App.Core;
using GestionFormation.Applications.Users;
using GestionFormation.CoreDomain;
using GestionFormation.CoreDomain.Users;
using GestionFormation.CoreDomain.Users.Queries;

namespace GestionFormation.App.Views.Logins
{

    public class LoginWindowsVm : PopupWindowVm
    {       
        private readonly IApplicationService _applicationService;
        private readonly IUserQueries _userQueries;
        private readonly IComputerService _computerService;
        private string _username;
        private string _password;
        private bool _connecting;

        public LoginWindowsVm(IApplicationService applicationService, IUserQueries userQueries, IComputerService computerService)
        {
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
            _userQueries = userQueries ?? throw new ArgumentNullException(nameof(userQueries));
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
                    if (!_userQueries.Exists("admin"))
                        await Task.Run(() => _applicationService.Command<CreateUser>().Execute("admin", "1234", "Administrateur", string.Empty, string.Empty, UserRole.Admin));

                    var command = new Logon(_userQueries);

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