using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GestionFormation.App.Core;

namespace GestionFormation.App.Views.EditableLists
{
    public abstract class EditableListVm<TCreateItem, TUpdateItem, TCreateItemVm> : ViewModelBase, ILoadableVm        
     where TCreateItem : new()
    where TCreateItemVm : ViewModelBase, IPopupVm
    {
        protected readonly IApplicationService ApplicationService;
        private ObservableCollection<TUpdateItem> _items;
        private bool _isLoading;
        private TUpdateItem _selectedItem;

        protected EditableListVm(IApplicationService applicationService)
        {
            ApplicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
            LoadCommand = new RelayCommandAsync(ExecuteLoadCommandAsync);
            DeleteCommand = new RelayCommandAsync(ExecuteDeleteCommandAsync, () => SelectedItem != null);
            UpdateCommand = new RelayCommandAsync(ExecuteUpdateCommandAsync);
            CreateCommand = new RelayCommandAsync(ExecuteCreateCommandAsync);
        }

        public ObservableCollection<TUpdateItem> Items
        {
            get => _items;
            set { Set(() => Items, ref _items, value); }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set { Set(() => IsLoading, ref _isLoading, value); }
        }

        public RelayCommandAsync CreateCommand { get; }
        private async Task ExecuteCreateCommandAsync()
        {
            var vm = await ApplicationService.OpenPopup<TCreateItemVm>("Ajouter un element", new TCreateItem());
            if (vm.IsValidated)
            {
                try
                {
                    await CreateAsync((TCreateItem) vm.Item);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    await ExecuteLoadCommandAsync();
                }
            }
        }
        
        public RelayCommandAsync DeleteCommand { get; }
        private async Task ExecuteDeleteCommandAsync()
        {
            if( MessageBoxResult.Yes == MessageBox.Show("Attention : vous êtes sur le point de supprimer la ligne sélectionnée.\r\nVoulez vous vraiment continuer ?", "Suppression", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No))
            {
                try
                {
                    await DeleteAsync(SelectedItem);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    await ExecuteLoadCommandAsync();
                }
            }
        }

        public RelayCommandAsync UpdateCommand { get; }
        private async Task ExecuteUpdateCommandAsync()
        {
            try
            {
                await UpdateAsync(SelectedItem);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                await ExecuteLoadCommandAsync();
            }
        }      

        public TUpdateItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                Set(() => SelectedItem, ref _selectedItem, value);
                RaiseCanExecuteChanged();
            }
        }

        public RelayCommandAsync LoadCommand { get; }
        private async Task ExecuteLoadCommandAsync()
        {
            try
            {
                IsLoading = true;
                Items = new ObservableCollection<TUpdateItem>(await LoadAsync());
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

        protected abstract Task<IReadOnlyList<TUpdateItem>> LoadAsync();

        protected abstract Task CreateAsync(TCreateItem item);
        protected abstract Task UpdateAsync(TUpdateItem item);
        protected abstract Task DeleteAsync(TUpdateItem item);

        public abstract string Title { get; }

        protected virtual void RaiseCanExecuteChanged()
        {
            DeleteCommand.RaiseCanExecuteChanged();
        }
    }

    public abstract class EditableListVm<T> : EditableListVm<T, T, CreateItemVm> where T : new()
    {
        protected EditableListVm(IApplicationService applicationService) : base(applicationService)
        {
        }
    }
}