using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;

namespace GestionFormation.App.Core
{
    public class RelayCommandAsync : RelayCommand
    {
        private readonly Func<Task> _executeAsync;

        public RelayCommandAsync(Func<Task> executeAsync) : this(executeAsync, () => true)
        {
        }

        public RelayCommandAsync(Func<Task> executeAsync, Func<bool> canExecute) : base(() => { }, canExecute)
        {
            _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
        }

        public override async void Execute(object parameter)
        {
            await ExecuteAsync();
        }

        public async Task ExecuteAsync()
        {
            if (CanExecute(null))
                await _executeAsync();
        }
    }

    public class RelayCommandAsync<T> : RelayCommand<T>
    {
        private readonly Func<T, Task> _executeAsync;

        public RelayCommandAsync(Func<T, Task> executeAsync) : this(executeAsync, (T source) => true)
        {
        }

        public RelayCommandAsync(Func<T, Task> executeAsync, Func<T, bool> canExecute) : base((T source) => { }, canExecute)
        {
            _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
        }

        public override async void Execute(object parameter)
        {
            await ExecuteAsync((T)parameter);
        }

        public async Task ExecuteAsync(T parameter)
        {
            if (CanExecute(parameter))
                await _executeAsync(parameter);
        }
    }
}
