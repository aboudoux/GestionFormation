using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;

namespace GestionFormation.App.Core
{
    public class RelayCommandAsync : RelayCommand
    {
        private readonly Func<Task> _executeAsync;
        private bool _executing;

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
            if (CanExecute(null) && !_executing)
            {
                try
                {
                    _executing = true;
                    var command = _executeAsync();
                    if (command != null)
                        await command;
                }
                finally
                {
                    _executing = false;
                }
            }
        }        
    }

    public class RelayCommandAsync<T> : RelayCommand<T>
    {
        private readonly Func<T, Task> _executeAsync;
        private bool _executing;

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
            if (CanExecute(parameter) && !_executing)
            {
                try
                {
                    _executing = true;
                    await _executeAsync(parameter);
                }
                finally
                {
                    _executing = false;
                }
            }
        }
    }
}
