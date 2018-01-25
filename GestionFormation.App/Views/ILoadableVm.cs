using GestionFormation.App.Core;

namespace GestionFormation.App.Views
{
    public interface ILoadableVm
    {
        RelayCommandAsync LoadCommand { get; }
    }
}