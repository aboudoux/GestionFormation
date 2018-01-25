using GestionFormation.App.Core;

namespace GestionFormation.App.Views.Places
{
    public class RaisonWindowVm : PopupWindowVm
    {
        private string _raison;

        public RaisonWindowVm()
        {
            SetValiderCommandCanExecute(() => !string.IsNullOrWhiteSpace(Raison));
        }
      
        public string Raison
        {
            get => _raison;
            set
            {
                Set(()=>Raison, ref _raison, value);
                ValiderCommand.RaiseCanExecuteChanged();
            }
        }       
    }
}