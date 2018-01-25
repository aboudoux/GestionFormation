using GestionFormation.App.Core;
using GestionFormation.CoreDomain.Utilisateurs;

namespace GestionFormation.App.Views.EditableLists.Utilisateurs
{
    public class ChangeRoleWindowVm : PopupWindowVm
    {
        private UtilisateurRole _role;

        public ChangeRoleWindowVm(UtilisateurRole role)
        {
            Role = role;
        }

        public UtilisateurRole Role
        {
            get => _role;
            set { Set(()=>Role, ref _role, value); }
        }
    }
}