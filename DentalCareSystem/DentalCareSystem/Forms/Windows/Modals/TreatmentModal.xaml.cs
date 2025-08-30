using DentalCareSystem.Data.DTO;
using DentalCareSystem.Util;
using System.Windows;

namespace DentalCareSystem.Forms.Windows.Modals
{
    public partial class TreatmentModal : Window
    {
        public Treatment Treatment { get; private set; } = new Treatment();
        private bool isEditMode = false;
        private int originalTreatmentId;

        public TreatmentModal()
        {
            InitializeComponent();
            this.DataContext = Treatment;
            isEditMode = false;
            ModalTitle.Content = LocalizationProvider.GetLocalizedString("AddNewTreatment");
            LocalizationProvider.UpdateAllObjects();
        }

        public TreatmentModal(Treatment treatment)
        {
            InitializeComponent();
            this.Treatment = new Treatment
            {
                TreatmentId = treatment.TreatmentId,
                Name = treatment.Name,
                Price = treatment.Price
            };
            this.DataContext = treatment;
            isEditMode = true;
            ModalTitle.Content = LocalizationProvider.GetLocalizedString("UpdateTreatment");
            LocalizationProvider.UpdateAllObjects();
        }

        private void AddOrUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) || string.IsNullOrWhiteSpace(PriceTextBox.Text))
            {
                TreatmentModalSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("FillFields"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                return;
            }

            if (!decimal.TryParse(PriceTextBox.Text, out decimal price) || price <= 0)
            {
                TreatmentModalSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("InvalidPriceFormat"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                return;
            }

            Treatment.Name = NameTextBox.Text;
            Treatment.Price = price;

            this.DialogResult = true; 
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}