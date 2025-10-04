using System.Windows;
using System.Windows.Controls;

namespace MSLauncher.UI.Utils
{
    public static class PasswordBoxHelper
    {
        public static readonly DependencyProperty PasswordLengthProperty =
            DependencyProperty.RegisterAttached(
                "PasswordLength",
                typeof(int),
                typeof(PasswordBoxHelper),
                new PropertyMetadata(0));

        public static int GetPasswordLength(DependencyObject obj)
        {
            return (int)obj.GetValue(PasswordLengthProperty);
        }

        public static void SetPasswordLength(DependencyObject obj, int value)
        {
            obj.SetValue(PasswordLengthProperty, value);
        }

        public static readonly DependencyProperty IsMonitoringProperty =
            DependencyProperty.RegisterAttached(
                "IsMonitoring",
                typeof(bool),
                typeof(PasswordBoxHelper),
                new UIPropertyMetadata(false, OnIsMonitoringChanged));

        public static bool GetIsMonitoring(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsMonitoringProperty);
        }

        public static void SetIsMonitoring(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMonitoringProperty, value);
        }

        private static void OnIsMonitoringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox pb)
            {
                if ((bool)e.NewValue)
                {
                    pb.PasswordChanged += PasswordBox_PasswordChanged;
                }
                else
                {
                    pb.PasswordChanged -= PasswordBox_PasswordChanged;
                }
            }
        }

        static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox pb)
            {
                SetPasswordLength(pb, pb.Password.Length);
            }
        }
    }
}