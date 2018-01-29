using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GestionFormation.App.Core;

namespace GestionFormation.App.Views.Listers
{
    public abstract class ListerWindowVm<TItem> : ViewModelBase, ILoadableVm
    {
        private bool _isLoading;
        private ObservableCollection<TItem> _items;
        public abstract string Title { get; }

        protected ListerWindowVm()
        {
            LoadCommand = new RelayCommandAsync(ExecuteLoadAsync);
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

        protected abstract Task<IEnumerable<TItem>> LoadAsync();
    }
}
