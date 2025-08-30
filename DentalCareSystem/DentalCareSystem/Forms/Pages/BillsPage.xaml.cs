using DentalCareSystem.Data.DAO.MySQL;
using DentalCareSystem.Data.DTO;
using DentalCareSystem.Forms.Windows.Modals;
using DentalCareSystem.Util;
using DentalCareSystem.ViewModel;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace DentalCareSystem.Forms.Pages
{
    public partial class BillsPage : Page
    {
        private readonly GenericDataGridViewModel<Bill> billsViewModel;
        private readonly BillDAOImpl dao = new BillDAOImpl();

        public BillsPage()
        {
            InitializeComponent();
            List<Bill> bills = dao.GetBills();
            this.billsViewModel = new GenericDataGridViewModel<Bill>()
            {
                Objects = bills,
                Items = new ObservableCollection<Bill>(bills)
            };
            this.DataContext = billsViewModel;
            LocalizationProvider.UpdateAllObjects();
            ExaminationPage.ExaminationAdded += OnExaminationAdded;
        }

        private void OnExaminationAdded()
        {
            Dispatcher.Invoke(() =>
            {
                UpdateBillsDisplayData();
            });
        }

        ~BillsPage()
        {
            ExaminationPage.ExaminationAdded -= OnExaminationAdded;
        }

        public void UpdateBillsDisplayData()
        {
            List<Bill> bills = dao.GetBills();
            billsViewModel.Items.Clear();
            foreach (Bill bill in bills)
            {
                billsViewModel.Items.Add(bill);
            }
            billsViewModel.Objects = bills;
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime? selectedDate = DatePicker.SelectedDate;
                if (selectedDate.HasValue)
                {
                    billsViewModel.Items.Clear();
                    IEnumerable<Bill> filtered = billsViewModel.Objects
                        .Where(bill => bill.DateTime.Date == selectedDate.Value.Date);

                    int filteredCount = 0;
                    foreach (Bill bill in filtered)
                    {
                        billsViewModel.Items.Add(bill);
                        filteredCount++;
                    }

                    Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("Filtered"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                }
                else
                {
                    Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("FilteredErrorDate"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                }
                DatePicker.SelectedDate = null;
            }
            catch (Exception)
            {
                Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("ErrorOccured"), null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                billsViewModel.Items.Clear();
                billsViewModel.Objects = dao.GetBills();
                foreach (Bill bill in billsViewModel.Objects)
                {
                    billsViewModel.Items.Add(bill);
                }
                Snackbar.MessageQueue?.Enqueue(
                    LocalizationProvider.GetLocalizedString("Refreshed"),
                    null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
            catch (Exception)
            {
                Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("ErrorOccured"), null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
        }

        private void ViewBill_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Bill selectedBill = DataGridBills.SelectedItem as Bill;

                if (selectedBill != null)
                {

                    DetailedBillModal detailedBillModal = new DetailedBillModal(selectedBill.BillId);

                    detailedBillModal.ShowDialog();
                }
                else
                {
         
                    Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("SelectBillToView"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                }
            }
            catch (Exception)
            {
                Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("ErrorOccured"), null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
        }

    }
}