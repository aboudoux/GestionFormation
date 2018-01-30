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
            PasswordEdit.Focus();
        }    
    }    
}
