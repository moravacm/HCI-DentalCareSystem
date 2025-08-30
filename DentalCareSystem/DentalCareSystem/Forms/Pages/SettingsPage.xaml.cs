using DentalCareSystem.Data.DAO.MySQL;
using DentalCareSystem.Data.DTO;
using DentalCareSystem.Forms.Windows;
using DentalCareSystem.Util;
using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DentalCareSystem.Forms.Pages
{
    public partial class SettingsPage : Page
    {
        private Employee employee;

        public SettingsPage()
        {
            employee = MainWindow.Employee;
            this.DataContext = employee;
            InitializeComponent();

            int selectedTheme = employee.SelectedTheme;
            cbTheme.SelectedIndex = selectedTheme;

            LocalizationProvider.UpdateAllObjects();
        }

        public static void ModifyTheme(int selectedTheme)
        {
            var paletteHelper = new PaletteHelper();
            var theme = paletteHelper.GetTheme();
            var random = new Random();
            switch (selectedTheme)
            {
                // light
                case 0:
                    ThemeExtensions.SetPrimaryColor(theme, Colors.CadetBlue);
                    ThemeExtensions.SetSecondaryColor(theme, Colors.Blue);
                    theme.SetBaseTheme(BaseTheme.Light);
                    break;
                // dark
                case 1:
                    ThemeExtensions.SetPrimaryColor(theme, Colors.CadetBlue);
                    ThemeExtensions.SetSecondaryColor(theme, Colors.Blue);
                    theme.SetBaseTheme(BaseTheme.Dark);
                    break;
                //surprise
                case 2:
                    var primaryColors = new List<Color>
                {
                    Colors.SeaGreen,
                    Colors.Teal,
                    Colors.SlateBlue,
                    Colors.DarkSeaGreen
                };

                    var secondaryColors = new List<Color>
                {
                    Colors.LawnGreen,
                    Colors.MidnightBlue,
                    Colors.DarkSlateBlue,
                    Colors.DarkCyan
                };

                    var baseTheme = random.Next(2) == 0 ? BaseTheme.Light : BaseTheme.Dark;
                    theme.SetBaseTheme(baseTheme);

                    var randomPrimaryColor = primaryColors[random.Next(primaryColors.Count)];
                    var randomSecondaryColor = secondaryColors[random.Next(secondaryColors.Count)];

                    ThemeExtensions.SetPrimaryColor(theme, randomPrimaryColor);
                    ThemeExtensions.SetSecondaryColor(theme, randomSecondaryColor);
                    break;
            }
            paletteHelper.SetTheme(theme);
        }

        private void ComboBoxTheme_DropDownClosed(object sender, EventArgs e)
        {
            ModifyTheme(cbTheme.SelectedIndex);
        }

        private void UpdateAccount_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(EmailTextBox.Text) && !EmailTextBox.Text.Contains("@"))
            {
                SnackbarOne.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("InvalidEmailFormat"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                return;
            }

            if (!string.IsNullOrEmpty(PhoneNumberTextBox.Text) && PhoneNumberTextBox.Text.Any(char.IsLetter))
            {
                SnackbarOne.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("InvalidPhoneFormat"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                return;
            }

            new EmployeeDAOImpl().UpdateEmployee(employee);
            SnackbarOne.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("AccountUpdated"), null, null, null, false, true, TimeSpan.FromSeconds(3));
        }
        private void ComboBoxLanguage_DropDownClosed(object sender, EventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ChangeLanguage(LanguageComboBox.SelectedIndex);
        }

    }
}