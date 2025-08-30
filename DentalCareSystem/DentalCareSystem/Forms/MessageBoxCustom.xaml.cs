using System.Windows;
using System.Windows.Media;
using DentalCareSystem.Util;

namespace CustomMessageBox
{
    public partial class MessageBoxCustom : Window
    {
        public MessageBoxCustom(string messageKey, MessageType type, MessageButtons buttons)
        {
            InitializeComponent();
            //txtMessage.Content = message;

            txtMessage.Content = LocalizationProvider.GetLocalizedString(messageKey);

            switch (type)
            {
                case MessageType.Info:
                    txtTitle.Text = LocalizationProvider.GetLocalizedString("InfoTitle");
                    changeBackgroundThemeColor(Color.FromArgb(255, 117, 169, 216));
                    break;
                case MessageType.Confirmation:
                    txtTitle.Text = LocalizationProvider.GetLocalizedString("ConfirmationTitle");
                    changeBackgroundThemeColor(Color.FromArgb(255, 117, 169, 216));
                    break;
                case MessageType.Success:
                    txtTitle.Text = LocalizationProvider.GetLocalizedString("SuccessTitle");
                    changeBackgroundThemeColor(Colors.Green);
                    break;
                case MessageType.Warning:
                    txtTitle.Text = LocalizationProvider.GetLocalizedString("WarningTitle");
                    changeBackgroundThemeColor(Colors.Orange);
                    break;
                case MessageType.Error:
                    txtTitle.Text = LocalizationProvider.GetLocalizedString("ErrorTitle");
                    changeBackgroundThemeColor(Colors.Red);
                    break;
            }

            switch (buttons)
            {
                case MessageButtons.OkCancel:
                    btnOk.Visibility = Visibility.Visible;
                    btnCancel.Visibility = Visibility.Visible;
                    btnYes.Visibility = Visibility.Collapsed;
                    btnNo.Visibility = Visibility.Collapsed;
                    break;
                case MessageButtons.YesNo:
                    btnOk.Visibility = Visibility.Collapsed;
                    btnCancel.Visibility = Visibility.Collapsed;
                    btnYes.Visibility = Visibility.Visible;
                    btnNo.Visibility = Visibility.Visible;
                    break;
                case MessageButtons.Ok:
                    btnOk.Visibility = Visibility.Visible;
                    btnCancel.Visibility = Visibility.Collapsed;
                    btnYes.Visibility = Visibility.Collapsed;
                    btnNo.Visibility = Visibility.Collapsed;
                    break;
            }
            LocalizationProvider.UpdateAllObjects();
        }

        public void changeBackgroundThemeColor(Color newColor)
        {
            cardHeader.Background = new SolidColorBrush(newColor);
            btnClose.Foreground = new SolidColorBrush(newColor);
            btnYes.Background = new SolidColorBrush(newColor);
            btnNo.Background = new SolidColorBrush(newColor);
            btnOk.Background = new SolidColorBrush(newColor);
            btnCancel.Background = new SolidColorBrush(newColor);
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

    public enum MessageType
    {
        Info,
        Confirmation,
        Success,
        Warning,
        Error,
    }

    public enum MessageButtons
    {
        Ok,
        YesNo,
        OkCancel,
    }
}