using DentalCareSystem.Data.DAO.Exceptions;
using DentalCareSystem.Data.DAO.MySQL;
using DentalCareSystem.Data.DTO;
using DentalCareSystem.Util;
using DentalCareSystem.ViewModel;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace DentalCareSystem.Forms.Pages
{
    public partial class MaterialsPage : Page
    {
        private readonly GenericDataGridViewModel<Material> materialsViewModel;
        private readonly MaterialDAOImpl dao = new MaterialDAOImpl();
        private readonly CategoryDAOImpl categoryDao = new CategoryDAOImpl();

        public MaterialsPage()
        {
            InitializeComponent();

            List<Material> materials = dao.GetMaterials();


            this.materialsViewModel = new GenericDataGridViewModel<Material>()
            {
                Objects = materials,
                Items = new ObservableCollection<Material>(materials)
            };

            DataGridMaterials.DataContext = materialsViewModel;

            LoadCategories();
            LocalizationProvider.UpdateAllObjects();
            ExaminationPage.ExaminationAdded += OnExaminationAdded;
        }

        private void OnExaminationAdded()
        {
            Dispatcher.Invoke(() =>
            {
                UpdateMaterialsDisplayData();
            });
        }

        ~MaterialsPage()
        {
            ExaminationPage.ExaminationAdded -= OnExaminationAdded;
        }

        public void UpdateMaterialsDisplayData()
        {
            try
            {
                List<Material> materials = dao.GetMaterials();

                materialsViewModel.Objects = materials;

                materialsViewModel.Items.Clear();

                foreach (Material material in materials)
                {
                    materialsViewModel.Items.Add(material);
                }

                LoadCategories();
            }
            catch (DataAccessException)
            {
                Snackbar.MessageQueue?.Enqueue(
                    LocalizationProvider.GetLocalizedString("ErrorRefreshingData"),
                    null, null, null, false, true, TimeSpan.FromSeconds(3));

            }
            catch (Exception ex)
            {
                
                Snackbar.MessageQueue?.Enqueue(
                    LocalizationProvider.GetLocalizedString("ErrorOccured"),
                    null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
        }

        private void LoadCategories()
        {
            try
            {
                List<Category> categories = categoryDao.GetCategories();
                CategoryComboBox.ItemsSource = categories.Select(c => c.Name).ToList();
            }
            catch (DataAccessException)
            {
                Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("ErrorLoadingCategories"), null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
        }


        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilterButton_Click(sender, e);
        }

        private void ApplyFilterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string selectedCategory = CategoryComboBox.SelectedItem as string;

                if (!string.IsNullOrEmpty(selectedCategory)) 
                {
                    materialsViewModel.Items.Clear();

                    var filtered = materialsViewModel.Objects.Where(material =>
                        (string.IsNullOrEmpty(selectedCategory) || material.Category.Name == selectedCategory) //&&
                                                                                                               //(string.IsNullOrEmpty(nameFilter) || material.Name.Contains(nameFilter, StringComparison.InvariantCultureIgnoreCase))
                    );

                    foreach (Material material in filtered)
                    {
                        materialsViewModel.Items.Add(material);
                    }

                    Snackbar.MessageQueue?.Enqueue(string.Format(LocalizationProvider.GetLocalizedString("Filtered"), materialsViewModel.Items.Count), null, null, null, false, true, TimeSpan.FromSeconds(3));
                }
                else
                {
                    materialsViewModel.Items.Clear();
                    foreach (Material material in materialsViewModel.Objects)
                    {
                        materialsViewModel.Items.Add(material);
                    }
                    Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("FilterCleared"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                }
            }
            catch (DataAccessException)
            {
                Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("ErrorApplyingFilter"), null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var materials = dao.GetMaterials();

                materialsViewModel.Items.Clear();
                materialsViewModel.Objects = materials;

                foreach (Material material in materialsViewModel.Objects)
                {
                    materialsViewModel.Items.Add(material);
                }

                LoadCategories();
                Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("Refreshed"), null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
            catch (DataAccessException)
            {
                Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("ErrorRefreshingData"), null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
        }

        private void ClearFilterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CategoryComboBox.SelectedItem = null;

                materialsViewModel.Items.Clear();
                foreach (Material material in materialsViewModel.Objects)
                {
                    materialsViewModel.Items.Add(material);
                }

                Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("FilterCleared"), null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
            catch (DataAccessException)
            {
                Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("ErrorOccured"), null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
        }
    }
}