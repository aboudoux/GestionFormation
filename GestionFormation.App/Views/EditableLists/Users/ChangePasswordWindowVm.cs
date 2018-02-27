using System.Threading.Tasks;
using System.Windows;
using GestionFormation.App.Core;

namespace GestionFormation.App.Views.EditableLists.Users
{
    public class ChangePasswordWindowVm : PopupWindowVm
    {
        private string _password1;
        private string _password2;

        public ChangePasswordWindowVm()
        {
            SetValiderCommandCanExecute(()=>!string.IsNullOrWhiteSpace(Password1) && !string.IsNullOrWhiteSpace(Password2));
        }

        public string Password1
        {
            get => _password1;
            set
            {
                Set(()=>Password1, ref _password1, value);
                ValiderCommand.RaiseCanExecuteChanged();
            }
        }

        public string Password2
        {
            get => _password2;
            set
            {
                Set(()=>Password2, ref _password2, value);
                ValiderCommand.RaiseCanExecuteChanged();
            }
        }

        protected override async Task ExecuteValiderAsync()
        {
            if( Password1 != Password2)
            { 
                MessageBox.Show("Le mot de passe ne correspond pas à la confirmation.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            await base.ExecuteValiderAsync();
        }

        public override string Title => "Changement du mot de passe";
    }
}