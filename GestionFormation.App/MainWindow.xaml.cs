using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Scheduling;
using GestionFormation.App.Core;
using GestionFormation.CoreDomain.Notifications.Queries;
using GestionFormation.Infrastructure.Notifications.Queries;

namespace GestionFormation.App
{
    public partial class MainWindow 
    {
        public MainWindow()
        {
            InitializeComponent();
            DataControlBase.AllowInfiniteGridSize = true;
            SchedulerControl.AllowInfiniteSize = true;
            DataContext = new MainWindowsVm(Bootstrapper.Start(DocumentGroup), new NotificationQueries());
        }
    }
}
