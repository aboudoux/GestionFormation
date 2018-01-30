using System;
using System.Threading.Tasks;

namespace GestionFormation.App.Core
{
    public interface IPopupVm
    {
        Task Init();

        event EventHandler OnClose;

        bool IsValidated { get; }
        object Item { get; }

    }
}