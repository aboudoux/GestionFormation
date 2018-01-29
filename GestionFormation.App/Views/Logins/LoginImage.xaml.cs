using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GestionFormation.App.Views.Logins
{
    /// <summary>
    /// Logique d'interaction pour LoginImage.xaml
    /// </summary>
    public partial class LoginImage
    {
        public static DependencyProperty ConnectingProperty = DependencyProperty.Register("Connecting", typeof(bool), typeof(LoginImage), new PropertyMetadata(){ PropertyChangedCallback = ConnectingChanged});

        private static void ConnectingChanged(DependencyObject @do, DependencyPropertyChangedEventArgs dp)
        {
            if (@do is LoginImage control)
            {
                var val = (bool) dp.NewValue;
                var sb = control.FindResource("Storyboard_connecting") as Storyboard;
                if(val)
                    sb.Begin();
                else
                    sb.Stop();
            }
        }

        public LoginImage()
        {
            InitializeComponent();
        }

        public bool Connecting
        {
            get => (bool)GetValue(ConnectingProperty);
            set => SetValue(ConnectingProperty, value);
        }
    }
}
