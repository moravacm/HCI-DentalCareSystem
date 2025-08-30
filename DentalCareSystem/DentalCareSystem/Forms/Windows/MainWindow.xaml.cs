using DentalCareSystem.Data.DTO;
using DentalCareSystem.Forms.Pages;
using DentalCareSystem.Util;
using System.Windows;

namespace DentalCareSystem.Forms.Windows
{

    public partial class MainWindow : Window
    {
        public static Employee Employee { get; set; }
        private LanguageModel model;

        public MainWindow(Employee employee, LanguageModel model)
        {
            Employee = employee;
            this.model = model;
            this.DataContext = model;
            InitializeComponent();
            LocalizationProvider.UpdateAllObjects();

            Loaded += (s, e) =>
            {
                Dispatcher.BeginInvoke(() =>
                {
                    LocalizationProvider.UpdateAllObjects();
                }, System.Windows.Threading.DispatcherPriority.ApplicationIdle);
            };
        }

        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            var settingsPage = (SettingsPage)SettingsPage.Content;
            int selectedIndex = settingsPage.cbTheme.SelectedIndex;
            string[] lines = System.IO.File.ReadAllLines("./users-theme.txt");
            string[] newArray = new string[lines.Length + 1];
            bool found = false;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Split(":")[0].Equals("" + Employee.JMBG))
                {
                    lines[i] = Employee.JMBG + ":" + selectedIndex;
                    found = true;
                }
                newArray[i] = lines[i];
            }
            if (!found)
            {
                newArray[lines.Length] = Employee.JMBG + ":" + selectedIndex;
            }
            System.IO.File.WriteAllLines("./users-theme.txt", newArray);

            SignIn signIn = new SignIn();
            signIn.Show();
            this.Close();
        }

        public void ChangeLanguage(int index)
        {
            switch (index)
            {
                case 0:
                    model.EnglishLanguageCmd.Execute(null);
                    break;
                case 1:
                    model.SerbianLanguageCmd.Execute(null);
                    break;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AppointmentsTabItem.Visibility = Visibility.Collapsed;
            ExaminationsTabItem.Visibility = Visibility.Collapsed;
            MaterialsTabItem.Visibility = Visibility.Collapsed;
            MaterialsAdminTabItem.Visibility = Visibility.Collapsed;
            PatientsTabItem.Visibility = Visibility.Collapsed;
            DentistsTabItem.Visibility = Visibility.Collapsed;
            MedicalTechniciansTabItem.Visibility = Visibility.Collapsed;
            BillsTabItem.Visibility = Visibility.Collapsed;
            EmployeesTabItem.Visibility = Visibility.Collapsed;
            TreatmentsTabItem.Visibility = Visibility.Collapsed;

            string role = Employee.Role?.Trim() ?? "";
            bool isAdmin = role.Equals("Administrator", StringComparison.OrdinalIgnoreCase) ||
                          role.Equals("Admin", StringComparison.OrdinalIgnoreCase);

            if (isAdmin)
            {
                MaterialsAdminTabItem.Visibility = Visibility.Visible;
                EmployeesTabItem.Visibility = Visibility.Visible;
                SettingsTabItem.Visibility = Visibility.Visible; 
                LogOutTabItem.Visibility = Visibility.Visible; 
                TreatmentsTabItem.Visibility = Visibility.Visible;

                MainTabControl.SelectedItem = EmployeesTabItem;
            }
            else
            {
                AppointmentsTabItem.Visibility = Visibility.Visible;
                ExaminationsTabItem.Visibility = Visibility.Visible;
                MaterialsTabItem.Visibility = Visibility.Visible;
                PatientsTabItem.Visibility = Visibility.Visible;
                DentistsTabItem.Visibility = Visibility.Visible;
                MedicalTechniciansTabItem.Visibility = Visibility.Visible;
                BillsTabItem.Visibility = Visibility.Visible;
                SettingsTabItem.Visibility = Visibility.Visible; 
                LogOutTabItem.Visibility = Visibility.Visible;   

                MainTabControl.SelectedItem = AppointmentsTabItem;
            }

            DentalCareSystem.Forms.Pages.SettingsPage.ModifyTheme(Employee.SelectedTheme);
            LocalizationProvider.UpdateAllObjects();
        }
    }
}
