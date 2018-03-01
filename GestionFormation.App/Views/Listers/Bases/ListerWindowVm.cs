using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Xpf.Bars.Native;
using GalaSoft.MvvmLight;
using GestionFormation.App.Core;

namespace GestionFormation.App.Views.Listers.Bases
{
    public abstract class ListerWindowVm<TItem> : ViewModelBase, ILoadableVm
    {
        private readonly IApplicationService _applicationService;
        private bool _isLoading;
        private ObservableCollection<TItem> _items;
        public abstract string Title { get; }

        protected ListerWindowVm(IApplicationService applicationService)
        {
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
            LoadCommand = new RelayCommandAsync(ExecuteLoadAsync);
            OpenStatisticsCommand = new RelayCommandAsync(ExecuteOpenStatisticsAsync);
        }        

        public bool IsLoading
        {
            get => _isLoading;
            set { Set(() => IsLoading, ref _isLoading, value); }
        }

        public ObservableCollection<TItem> Items
        {
            get => _items;
            set { Set(()=>Items, ref _items, value); }
        }

        public RelayCommandAsync LoadCommand { get; }
        private async Task ExecuteLoadAsync()
        {
            try
            {
                IsLoading = true;
                Items = new ObservableCollection<TItem>(await LoadAsync());
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public RelayCommandAsync OpenStatisticsCommand { get; }
        private Task ExecuteOpenStatisticsAsync()
        {
            var vm = _applicationService.OpenDocument<StatisticWindowVm>(Title);            
            vm.Items = new ObservableCollection<object>((IEnumerable<object>)Items);
            return Task.CompletedTask;            
        }

        protected abstract Task<IEnumerable<TItem>> LoadAsync();
    }
}
