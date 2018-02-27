using GestionFormation.App.Core;

namespace GestionFormation.App.Views.Seats
{
    public class ReasonWindowVm : PopupWindowVm
    {
        private string _reason;

        public ReasonWindowVm()
        {
            SetValiderCommandCanExecute(() => !string.IsNullOrWhiteSpace(Reason));
        }
      
        public string Reason
        {
            get => _reason;
            set
            {
                Set(()=>Reason, ref _reason, value);
                ValiderCommand.RaiseCanExecuteChanged();
            }
        }

        public override string Title => "Pourquoi ?";
    }
}