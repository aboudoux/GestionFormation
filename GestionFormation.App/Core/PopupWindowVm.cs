using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace GestionFormation.App.Core
{
    public class PopupWindowVm : ViewModelBase, IPopupVm
    {
        public PopupWindowVm()
        {
            AnnulerCommand = new RelayCommand(Close);
            ValiderCommand = new RelayCommandAsync(ExecuteValiderAsync);
        }

        protected void SetValiderCommandCanExecute(Func<bool> canExecute)
        {
            ValiderCommand = new RelayCommandAsync(ExecuteValiderAsync, canExecute);
        }

        public virtual Task Init()
        {
            return Task.CompletedTask;
        }

        public event EventHandler OnClose;

        protected virtual void Close()
        {
            OnClose?.Invoke(this, EventArgs.Empty);
        }

        public RelayCommand AnnulerCommand { get; }
        public RelayCommandAsync ValiderCommand { get; private set; }
        protected virtual Task ExecuteValiderAsync()
        {
            IsValidated = true;
            Close();
            return Task.CompletedTask;
        }

        public bool IsValidated { get; private set; }
    }
}