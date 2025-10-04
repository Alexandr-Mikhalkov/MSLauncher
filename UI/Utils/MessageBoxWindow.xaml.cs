using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace MSLauncher.UI.Utils
{
    public partial class MessageBoxWindow : Window
    {
        private MessageBoxWindow(string title, string message, MessageIcon icon)
        {
            InitializeComponent();

            DataContext = new
            {
                Title = title,
                Message = message,
                Icon = icon
            };
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            Close();
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }

        public static MessageBoxResult Show(string message, string title = "", MessageBoxButton buttons = MessageBoxButton.OK, MessageIcon icon = MessageIcon.Info)
        {
            var dialog = new MessageBoxWindow(title, message, icon);

            dialog.Owner = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.IsActive) ?? Application.Current.MainWindow;

            switch (buttons)
            {
                case MessageBoxButton.OK:
                    dialog.OkButton.Visibility = Visibility.Visible;
                    dialog.YesButton.Visibility = Visibility.Collapsed;
                    dialog.NoButton.Visibility = Visibility.Collapsed;
                    dialog.CancelButton.Visibility = Visibility.Collapsed;
                    break;
                case MessageBoxButton.OKCancel:
                    dialog.OkButton.Visibility = Visibility.Visible;
                    dialog.CancelButton.Visibility = Visibility.Visible;
                    dialog.YesButton.Visibility = Visibility.Collapsed;
                    dialog.NoButton.Visibility = Visibility.Collapsed;
                    break;
                case MessageBoxButton.YesNo:
                    dialog.YesButton.Visibility = Visibility.Visible;
                    dialog.NoButton.Visibility = Visibility.Visible;
                    dialog.OkButton.Visibility = Visibility.Collapsed;
                    dialog.CancelButton.Visibility = Visibility.Collapsed;
                    break;
                case MessageBoxButton.YesNoCancel:
                    dialog.YesButton.Visibility = Visibility.Visible;
                    dialog.NoButton.Visibility = Visibility.Visible;
                    dialog.CancelButton.Visibility = Visibility.Visible;
                    dialog.OkButton.Visibility = Visibility.Collapsed;
                    break;
            }

            bool? dialogResult = dialog.ShowDialog();

            switch (dialogResult)
            {
                case true:
                    return (buttons == MessageBoxButton.OK || buttons == MessageBoxButton.OKCancel)
                           ? MessageBoxResult.OK
                           : MessageBoxResult.Yes;
                case false:
                    return (buttons == MessageBoxButton.YesNo || buttons == MessageBoxButton.YesNoCancel)
                           ? MessageBoxResult.No
                           : MessageBoxResult.Cancel;
                default:
                    return MessageBoxResult.None;
            }
        }
    }
}