using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GestionFormation.App.Core;
using GestionFormation.Applications.Utilisateurs;
using GestionFormation.CoreDomain.Utilisateurs;
using GestionFormation.CoreDomain.Utilisateurs.Queries;

namespace GestionFormation.App.Views.EditableLists.Utilisateurs
{
    public class UtilisateurListVm : EditableListVm<EditableUtilisateurCreate, EditableUtilisateurUpdate, CreateItemVm>
    {
        private readonly IUtilisateurQueries _utilisateurQueries;

        public UtilisateurListVm(IApplicationService applicationService, IUtilisateurQueries utilisateurQueries) : base(applicationService)
        {
            _utilisateurQueries = utilisateurQueries ?? throw new ArgumentNullException(nameof(utilisateurQueries));
            ChangePasswordCommand = new RelayCommandAsync(ExecuteChangePasswordAsync, () => SelectedItem != null);
            ChangeRoleCommand = new RelayCommandAsync(ExecuteChangeRoleAsync, () => SelectedItem != null);
        }
        
        protected override async Task<IReadOnlyList<EditableUtilisateurUpdate>> LoadAsync()
        {
            return await Task.Run(()=> _utilisateurQueries.GetAll().Select(a=>new EditableUtilisateurUpdate(a)).ToList());
        }

        protected override async Task CreateAsync(EditableUtilisateurCreate item)
        {
            await Task.Run(()=>ApplicationService.Command<CreateUtilisateur>().Execute(item.Login, item.Password, item.Nom, item.Prenom, item.Email, UtilisateurRole.Invité));
        }

        protected override async Task UpdateAsync(EditableUtilisateurUpdate item)
        {
            await Task.Run(() => ApplicationService.Command<UpdateUtilisateur>().Execute(item.GetId(), item.Nom, item.Prenom, item.Email, item.IsEnabled));
        }

        protected override async Task DeleteAsync(EditableUtilisateurUpdate item)
        {
            await Task.Run(()=>ApplicationService.Command<DeleteUtilisateur>().Execute(item.GetId()));
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

    public class EditableUtilisateurCreate
    {
        public string Login { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }     
        public string Email { get; set; }
                
    }

    public class EditableUtilisateurUpdate : EditableItem
    {
        private UtilisateurRole _role;

        public EditableUtilisateurUpdate()
        {
            
        }

        public EditableUtilisateurUpdate(IUtilisateurResult result) : base(result.Id)
        {
            Login = result.Login;
            Nom = result.Nom;
            Prenom = result.Prenom;
            Email = result.Email;
            IsEnabled = result.IsEnabled;
            _role = result.Role;

            switch (result.Role)
            {
                case UtilisateurRole.Admin:
                    Role = "Administrateur";
                    break;
                case UtilisateurRole.GestionnaireFormation:
                    Role = "Gestionnaire formation";
                    break;
                case UtilisateurRole.ServiceFormation:
                    Role = "Service formation";
                    break;
                case UtilisateurRole.Invité:
                    Role = "Invité";
                    break;
                default:
                    Role = "Inconnu";
                    break;
            }
        }

        public string Login { get; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public bool IsEnabled { get; set; }
        public string Email { get; set; }
        public string Role { get; }

        public UtilisateurRole GetRole()
        {
            return _role;
        }
    }
}