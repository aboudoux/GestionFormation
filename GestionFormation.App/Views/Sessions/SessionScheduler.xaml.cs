using System;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Scheduling;

namespace GestionFormation.App.Views.Sessions
{
    /// <summary>
    /// Logique d'interaction pour SessionScheduler.xaml
    /// </summary>
    public partial class SessionScheduler : DocumentPanel
    {
        public SessionScheduler()
        {
            InitializeComponent();
        }

        private void SchedulerControl_OnAppointmentWindowShowing(object sender, AppointmentWindowShowingEventArgs e)
        {
            e.Cancel = true;
        }

      
        private void Scheduler_OnAppointmentDrop(object sender, AppointmentItemDragDropEventArgs e)
        {
            var vm = DataContext as SessionSchedulerVm;
            var appointmentVm = e.ViewModels[0].Appointment;
            vm?.DropSession.ExecuteAsync(new SessionDropped((Guid)appointmentVm.Id, e.HitInterval.Start, appointmentVm.Duration.Days));
        }

        private void Scheduler_OnPopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if(e.MenuType != ContextMenuType.CellContextMenu)
                e.Cancel = true;
        }       
    }
}
