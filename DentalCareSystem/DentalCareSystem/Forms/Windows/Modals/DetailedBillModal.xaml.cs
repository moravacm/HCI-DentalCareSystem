using DentalCareSystem.Data.DAO.MySQL;
using DentalCareSystem.Util;
using System.Windows;

namespace DentalCareSystem.Forms.Windows.Modals
{
    public partial class DetailedBillModal : Window
    {
        private readonly BillDAOImpl _billDao = new BillDAOImpl();
        private readonly int _billId;

        public DetailedBillModal(int billId)
        {
            InitializeComponent();
            _billId = billId;
            LoadBillDetails();
            LocalizationProvider.UpdateAllObjects();
        }

        private void LoadBillDetails()
        {
            try
            {
                var detailedBill = _billDao.GetBill(_billId);

                if (detailedBill != null)
                {
                    DataGridItems.ItemsSource = detailedBill.Items;

                    TotalPriceLabel.Content = detailedBill.TotalPrice.ToString("F2");

                    DateTimeLabel.Content = detailedBill.DateTime.ToString("dd.MM.yyyy HH:mm");

                    TotalPriceLabel.Content = detailedBill.TotalPrice;

                    MedicalTechnicianLabel.Content = $"{detailedBill.MedichalTechnicianFirstName} {detailedBill.MedichalTechnicianLastName}";

                    PatientLabel.Content = $"{detailedBill.PatientFirstName} {detailedBill.PatientLastName}";
                }
            }
            catch (System.Exception)
            {

            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}