using System;
using System.Threading.Tasks;
using System.Windows;

namespace GestionFormation.App.Core
{
    public static class HandleMessageBoxError
    {
        public static async Task Execute(Func<Task> action)
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
    }
}