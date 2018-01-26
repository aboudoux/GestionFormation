using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Animation;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Core;

namespace GestionFormation.App.Views.Logins
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow
    {
        private LoginWindowsVm _context;

        public LoginWindow()
        {
            InitializeComponent();
            LoginTextEdit.Focus();
        }

        private void LoginWindow_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _context = (LoginWindowsVm) e.NewValue;
            if(_context != null)
                _context.PropertyChanged += ContextOnPropertyChanged;
        }

        private void ContextOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == nameof(_context.Connecting))
            {
                var sb = FindResource("Storyboard_connecting") as Storyboard;
                if (_context.Connecting)                    
                    sb.Begin();
                else
                    sb.Stop();
            }
        }
    }

    
}
