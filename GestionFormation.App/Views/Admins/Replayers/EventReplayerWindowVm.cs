using System.Threading.Tasks;
using GestionFormation.App.Core;
using GestionFormation.Applications;
using GestionFormation.EventStore;
using GestionFormation.Kernel;

namespace GestionFormation.App.Views.Admins.Replayers
{
    public class EventReplayerWindowVm : PopupWindowVm
    {
        private int _progressMin;
        private int _progressMax;
        private int _progressValue;
        private bool _isRunning;

        public EventReplayerWindowVm()
        {
            StartCommand = new RelayCommandAsync(ExecuteStartAsync, ()=> !_isRunning);
        }

        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                Set(()=>IsRunning, ref _isRunning, value);
                StartCommand.RaiseCanExecuteChanged();
            }
        }

        public int ProgressMin
        {
            get => _progressMin;
            set { Set(()=>ProgressMin, ref _progressMin, value); }
        }

        public int ProgressMax
        {
            get => _progressMax;
            set { Set(()=>ProgressMax, ref _progressMax, value); }
        }

        public int ProgressValue
        {
            get => _progressValue;
            set { Set(()=>ProgressValue, ref _progressValue, value); }
        }

        public RelayCommandAsync StartCommand { get; }
        private async Task ExecuteStartAsync()
        {
            ProgressMin = 0;
            var replayer = new EventReplayer(new SqlEventStore(new DomainEventJsonEventSerializer(new DomainEventTypeBinder()), new EmptyEventStamping()));
            IsRunning = true;
            await Task.Run(() => { 
                replayer.Execute(Progession);
            });
            IsRunning = false;
        }

        private void Progession(int current, int totalCount )
        {
            if (ProgressMax == 0)
                ProgressMax = totalCount;
            ProgressValue = current;
        }

        public override string Title => "Réidratation de la base";
    }
}