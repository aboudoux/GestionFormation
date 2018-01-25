using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GestionFormation.Applications;
using GestionFormation.EventStore;

namespace GestionFormation.App.Core
{
    public interface IApplicationService
    {
        T OpenDocument<T>(params object[] injectionParameters) where T : ViewModelBase;
        Task<T> OpenPopup<T>(params object[] injectionParameters) where T : ViewModelBase, IPopupVm;
        T Command<T>() where T : ActionCommand;

        ILoggedUser LoggedUser { get; }
    }
}