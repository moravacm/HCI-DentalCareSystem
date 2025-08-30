using DentalCareSystem.Data.DAO.MySQL;
using DentalCareSystem.Data.DTO;
using DentalCareSystem.Util;
using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;
using DentalCareSystem.Forms.Windows;

namespace DentalCareSystem
{

    public partial class SignIn : Window
    {
        public LanguageModel model;

        public SignIn()
        {
            model = new LanguageModel();
            DataContext = model;
            InitializeComponent();
            model.EnglishLanguageCmd.Execute(null);
            LocalizationProvider.UpdateAllObjects();
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(UsernameTextBox.Text) || String.IsNullOrEmpty(PasswordBox.Password))
            {
                Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("FillFields"), null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
            else
            {
                Employee employee = MySQLUtil.SignIn(UsernameTextBox.Text, PasswordBox.Password);
                if (string.IsNullOrEmpty(employee.JMBG))
                {
                    Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("InvalidCredentials"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                    return;
                }

                string[] lines = System.IO.File.ReadAllLines("./users-theme.txt");
                string selectedIndex = "0";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Split(":")[0].Equals("" + employee.JMBG))
                    {
                        selectedIndex = lines[i].Split(":")[1];
                        break;
                    }
                }
                employee.SelectedTheme = Int32.Parse(selectedIndex);

                MainWindow mainWindow = new MainWindow(employee, model);
                mainWindow.Show();
                this.Close();
            }
        }
    }
}