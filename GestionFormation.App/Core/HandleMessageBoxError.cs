using System;
using System.Threading.Tasks;
using System.Windows;

namespace GestionFormation.App.Core
{
    public static class HandleMessageBoxError
    {
        public static async Task ExecuteAsync(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void Execute(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}