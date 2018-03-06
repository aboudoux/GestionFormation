using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GestionFormation.App.Core;
using GestionFormation.Applications.Users;
using GestionFormation.CoreDomain.Users;
using GestionFormation.CoreDomain.Users.Queries;

namespace GestionFormation.App.Views.EditableLists.Users
{
    public class UserListVm : EditableListVm<EditableUserCreate, EditableUserUpdate, CreateItemVm>
    {
        private readonly IUserQueries _userQueries;

        public UserListVm(IApplicationService applicationService, IUserQueries userQueries) : base(applicationService)
        {
            _userQueries = userQueries ?? throw new ArgumentNullException(nameof(userQueries));
            ChangePasswordCommand = new RelayCommandAsync(ExecuteChangePasswordAsync, () => SelectedItem != null);
            ChangeRoleCommand = new RelayCommandAsync(ExecuteChangeRoleAsync, () => SelectedItem != null);
        }
        
        protected override async Task<IReadOnlyList<EditableUserUpdate>> LoadAsync()
        {
            return await Task.Run(()=> _userQueries.GetAll().Select(a=>new EditableUserUpdate(a)).ToList());
        }

        protected override async Task CreateAsync(EditableUserCreate item)
        {
            await Task.Run(()=>ApplicationService.Command<CreateUser>().Execute(item.Login, item.Password, item.Lastname, item.Firstname, item.Email, UserRole.Guest, item.Signature));
        }

        protected override async Task UpdateAsync(EditableUserUpdate item)
        {
            await Task.Run(() => ApplicationService.Command<UpdateUser>().Execute(item.GetId(), item.Lastname, item.Firstname, item.Email, item.IsEnabled, item.Signature));
        }

        protected override async Task DeleteAsync(EditableUserUpdate item)
        {
            await Task.Run(()=>ApplicationService.Command<DeleteUser>().Execute(item.GetId()));
        }

        protected override void RaiseCanExecuteChanged()
        {
            base.RaiseCanExecuteChanged();
            ChangePasswordCommand.RaiseCanExecuteChanged();
            ChangeRoleCommand.RaiseCanExecuteChanged();
        }

        public RelayCommandAsync ChangePasswordCommand { get; }
        private async Task ExecuteChangePasswordAsync()
        {
            var popup = await ApplicationService.OpenPopup<ChangePasswordWindowVm>();
            if (popup.IsValidated)
            {
                await HandleMessageBoxError.ExecuteAsync(async () => { 
                    await Task.Run(()=>ApplicationService.Command<ChangePassword>().Execute(SelectedItem.GetId(), popup.Password1));
                    MessageBox.Show("Le mot de passe a été changé avec succès !", "Mot de passe changé", MessageBoxButton.OK, MessageBoxImage.Information);
                });
            }
        }

        public RelayCommandAsync ChangeRoleCommand { get; }
        private async Task ExecuteChangeRoleAsync()
        {
            var popup = await ApplicationService.OpenPopup<ChangeRoleWindowVm>(SelectedItem.GetRole());
            if (popup.IsValidated)
            {
                await HandleMessageBoxError.ExecuteAsync(async () => {
                    await Task.Run(() => ApplicationService.Command<ChangeRole>().Execute(SelectedItem.GetId(), popup.Role));
                    await LoadCommand.ExecuteAsync();
                });
            }
        }

        public override string Title => "Liste des utilisateurs";
    }

    public class EditableUserCreate
    {
        [DisplayName("Login")]
        public string Login { get; set; }
        [DisplayName("Nom")]
        public string Lastname { get; set; }
        [DisplayName("Prénom")]
        public string Firstname { get; set; }
        [DataType(DataType.Password)]
        [DisplayName("Mot de passe")]
        public string Password { get; set; }
        [DisplayName("Email")]
        public string Email { get; set; }
        [DisplayName("Signature")]
        [DataType(DataType.MultilineText)]
        public string Signature { get; set; }
    }

    public class EditableUserUpdate : EditableItem
    {
        private readonly UserRole _role;

        public EditableUserUpdate()
        {
            
        }

        public EditableUserUpdate(IUserResult result) : base(result.Id)
        {
            Login = result.Login;
            Lastname = result.Lastname;
            Firstname = result.Firsname;
            Email = result.Email;
            IsEnabled = result.IsEnabled;
            _role = result.Role;
            Signature = result.Signature;

            switch (result.Role)
            {
                case UserRole.Admin:
                    Role = "Administrateur";
                    break;
                case UserRole.Manager:
                    Role = "Gestionnaire formation";
                    break;
                case UserRole.Operator:
                    Role = "Service formation";
                    break;
                case UserRole.Guest:
                    Role = "Invité";
                    break;
                case UserRole.Trainer:
                    Role = "Formateur";
                    break;
                default:
                    Role = "Inconnu";
                    break;
            }
        }

        [DisplayName("Login")]
        public string Login { get; }
        [DisplayName("Nom")]
        public string Lastname { get; set; }
        [DisplayName("Prénom")]
        public string Firstname { get; set; }
        [DisplayName("Actif")]
        public bool IsEnabled { get; set; }
        [DisplayName("Email")]
        public string Email { get; set; }
        [DisplayName("Rôle")]
        public string Role { get; }
        [DisplayName("Signature")]
        [DataType(DataType.MultilineText)]
        public string Signature { get; set; }

        public UserRole GetRole()
        {
            return _role;
        }      
    }
}