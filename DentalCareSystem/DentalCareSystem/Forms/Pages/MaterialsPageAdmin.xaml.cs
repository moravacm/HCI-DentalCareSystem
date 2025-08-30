using CustomMessageBox;
using DentalCareSystem.Data.DAO.Exceptions;
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
    public partial class MaterialsPageAdmin : Page
    {
        private readonly GenericDataGridViewModel<Category> categoryViewModel;
        private readonly GenericDataGridViewModel<Material> materialViewModel;
        private readonly MaterialDAOImpl daoMaterial = new MaterialDAOImpl();
        private readonly CategoryDAOImpl daoCategory = new CategoryDAOImpl();

        public MaterialsPageAdmin()
        {
            InitializeComponent();

            var categories = daoCategory.GetCategories();
            var materials = daoMaterial.GetMaterials();

            categoryViewModel = new GenericDataGridViewModel<Category>()
            {
                Objects = categories,
                Items = new ObservableCollection<Category>(categories)
            };

            materialViewModel = new GenericDataGridViewModel<Material>()
            {
                Objects = materials,
                Items = new ObservableCollection<Material>(materials)
            };

            DataGridCategories.DataContext = categoryViewModel;
            DataGridMaterials.DataContext = materialViewModel;
            LocalizationProvider.UpdateAllObjects();
        }


        private void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            CategoryModal modal = new CategoryModal();
            modal.ShowDialog();
            try
            {
                if (modal.CategoryAdded)
                {
                    var category = modal.GetCategory();
                    if (daoCategory.AddCategory(category))
                    {
                        RefreshCategories();
                        MaterialsSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("CategoryAdded"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                    }
                    else
                    {
                        MaterialsSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("FailedToAddCategory"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                    }
                }
            }
            catch (DataAccessException)
            {
                MaterialsSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("ErrorOccured"), null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
        }

        private void EditCategory_Click(object sender, RoutedEventArgs e)
        {
            var selectedCategory = categoryViewModel.SelectedItem;
            if (selectedCategory == null)
            {
                MaterialsSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("NoSelection"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                return;
            }

            CategoryModal modal = new CategoryModal(selectedCategory);
            modal.ShowDialog();
            try
            {
                if (modal.CategoryUpdated)
                {
                    var updatedCategory = modal.GetCategory();
                    daoCategory.UpdateCategory(updatedCategory);

                    RefreshCategories();
                    RefreshMaterials();
                    MaterialsSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("CategoryUpdated"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                }
            }
            catch (DataAccessException)
            {
                MaterialsSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("ErrorOccured"), null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
        }

        private void DeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            var selectedCategory = categoryViewModel.SelectedItem;
            if (selectedCategory == null)
            {
                MaterialsSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("NoSelection"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                return;
            }

            bool? result = new MessageBoxCustom(LocalizationProvider.GetLocalizedString("ConfirmDelete"), MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();

            if (result == false)
                return;

            try
            {
                if (daoCategory.DeleteCategory(selectedCategory.Id))
                {
                    categoryViewModel.Items.Remove(selectedCategory);
                    categoryViewModel.Objects = categoryViewModel.Items.ToList();

                    var materialsToRemove = materialViewModel.Items.Where(m => m.CategoryId == selectedCategory.Id).ToList();
                    foreach (var material in materialsToRemove)
                    {
                        materialViewModel.Items.Remove(material);
                    }

                    materialViewModel.Objects = materialViewModel.Items.ToList();

                    MaterialsSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("CategoryDeleted"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                }
                else
                {
                    MaterialsSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("ErrorOccured"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                }
            }
            catch (DataAccessException ex)
            {
                if (ex.InnerException?.Message.Contains("foreign key constraint fails") == true)
                {
                    MaterialsSnackbar.MessageQueue?.Enqueue(
                        LocalizationProvider.GetLocalizedString("DeleteNotAllowed"),
                        null, null, null, false, true, TimeSpan.FromSeconds(5));
                }
                else
                {
                    MaterialsSnackbar.MessageQueue?.Enqueue(
                        LocalizationProvider.GetLocalizedString("ErrorOccurred"),
                        null, null, null, false, true, TimeSpan.FromSeconds(5));
                }
            }
        }


        private void AddMaterial_Click(object sender, RoutedEventArgs e)
        {
            MaterialModal modal = new MaterialModal();
            modal.ShowDialog();
            try
            {
                if (modal.MaterialAdded)
                {
                    var newMaterial = modal.GetMaterial();
                    if (daoMaterial.AddMaterial(newMaterial))
                    {
                        RefreshMaterials();
                        MaterialsSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("MaterialAdded"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                    }
                    else
                    {
                        MaterialsSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("ErrorOccured"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                    }
                }
            }
            catch (DataAccessException)
            {
                MaterialsSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("ErrorOccured"), null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
        }

        private void EditMaterial_Click(object sender, RoutedEventArgs e)
        {
            var selectedMaterial = materialViewModel.SelectedItem;
            if (selectedMaterial == null)
            {
                MaterialsSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("NoSelection"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                return;
            }

            MaterialModal modal = new MaterialModal(selectedMaterial);
            modal.ShowDialog();
            try
            {
                if (modal.MaterialUpdated)
                {
                    var updatedMaterial = modal.GetMaterial();
                    daoMaterial.UpdateMaterial(updatedMaterial);

                    RefreshMaterials();
                    MaterialsSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("MaterialUpdated"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                }
            }
            catch (DataAccessException)
            {
                MaterialsSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("ErrorOccured"), null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
        }

        private void DeleteMaterial_Click(object sender, RoutedEventArgs e)
        {
            var selectedMaterial = materialViewModel.SelectedItem;
            if (selectedMaterial == null)
            {
                MaterialsSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("NoSelection"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                return;
            }

            bool? result = new MessageBoxCustom(LocalizationProvider.GetLocalizedString("ConfirmDelete"), MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();

            if (result == false)
                return;

            try
            {
                if (daoMaterial.DeleteMaterial(selectedMaterial.MaterialId))
                {
                    materialViewModel.Items.Remove(selectedMaterial);
                    materialViewModel.Objects = materialViewModel.Items.ToList();
                    MaterialsSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("MaterialDeleted"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                }
                else
                {
                    MaterialsSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("ErrorOccured"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                }
            }
            catch (DataAccessException ex)
            {
                if (ex.InnerException?.Message.Contains("foreign key constraint fails") == true)
                {
                    MaterialsSnackbar.MessageQueue?.Enqueue(
                        LocalizationProvider.GetLocalizedString("DeleteNotAllowed"),
                        null, null, null, false, true, TimeSpan.FromSeconds(5));
                }
                else
                {
                    MaterialsSnackbar.MessageQueue?.Enqueue(
                        LocalizationProvider.GetLocalizedString("ErrorOccurred"),
                        null, null, null, false, true, TimeSpan.FromSeconds(5));
                }
            }
        }


        private void RefreshCategories()
        {
            try
            {
                categoryViewModel.Items.Clear();
                var categories = daoCategory.GetCategories();
                categoryViewModel.Objects = categories;

                foreach (var category in categories)
                {
                    categoryViewModel.Items.Add(category);
                }
            }
            catch (DataAccessException)
            {
                MaterialsSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("ErrorRefreshingData"), null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
        }

        private void RefreshMaterials()
        {
            try
            {
                materialViewModel.Items.Clear();
                var materials = daoMaterial.GetMaterials();
                materialViewModel.Objects = materials;

                foreach (var material in materials)
                {
                    materialViewModel.Items.Add(material);
                }
            }
            catch (DataAccessException)
            {
                MaterialsSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("ErrorRefreshingData"), null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
        }

        public void RefreshData()
        {
            RefreshCategories();
            RefreshMaterials();
            MaterialsSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("Refreshed"), null, null, null, false, true, TimeSpan.FromSeconds(3));
        }


        private void DataGridCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var selectedCategory = categoryViewModel.SelectedItem;

                if (selectedCategory != null)
                {
                    MainTabControl.SelectedIndex = 1;
                    FilterMaterialsByCategory(selectedCategory);
                    MaterialsSnackbar.MessageQueue?.Enqueue(string.Format(LocalizationProvider.GetLocalizedString("Filtered"), selectedCategory.Name), null, null, null, false, true, TimeSpan.FromSeconds(3));
                }
                else
                {
                    ShowAllMaterials();
                }
            }
            catch (DataAccessException)
            {
                MaterialsSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("ErrorApplyingFilter"), null, null, null, false, true, TimeSpan.FromSeconds(3));
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
                materialViewModel.Items.Clear();

                IEnumerable<Material> filteredMaterials = Enumerable.Empty<Material>();

                switch (ComboBox.SelectedIndex)
                {
                    case 0: 
                        filteredMaterials = materialViewModel.Objects
                            .Where(m => m.CurrentStock < m.MinimumStock);
                        break;

                    case 1: 
                        filteredMaterials = materialViewModel.Objects
                            .Where(m => m.CurrentStock >= m.MinimumStock);
                        break;

                    default:
                        filteredMaterials = materialViewModel.Objects;
                        break;
                }

                foreach (var material in filteredMaterials)
                {
                    materialViewModel.Items.Add(material);
                }

                MainTabControl.SelectedIndex = 1;

                MaterialsSnackbar.MessageQueue?.Enqueue(
                    string.Format(LocalizationProvider.GetLocalizedString("Filtered"), materialViewModel.Items.Count),
                    null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
            catch (DataAccessException)
            {
                MaterialsSnackbar.MessageQueue?.Enqueue(
                    LocalizationProvider.GetLocalizedString("ErrorApplyingFilter"),
                    null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
        }


        private void ClearFilterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ComboBox.SelectedItem = null;
                //NameFilterTextBox.Text = string.Empty;

                materialViewModel.Items.Clear();
                foreach (Material material in materialViewModel.Objects)
                {
                    materialViewModel.Items.Add(material);
                }

                MaterialsSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("FilterCleared"), null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
            catch (DataAccessException)
            {
                MaterialsSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("ErrorOccured"), null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
        }


        private void FilterMaterialsByCategory(Category selectedCategory)
        {
            try
            {
                materialViewModel.Items.Clear();

                var filteredMaterials = materialViewModel.Objects.Where(material =>
                    material.CategoryId == selectedCategory.Id ||
                    (material.Category != null && material.Category.Id == selectedCategory.Id)
                );

                foreach (var material in filteredMaterials)
                {
                    materialViewModel.Items.Add(material);
                }

            }
            catch (DataAccessException)
            {
                MaterialsSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("ErrorOccured"), null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
        }

        private void ShowAllMaterials()
        {
            try
            {
                materialViewModel.Items.Clear();
                foreach (var material in materialViewModel.Objects)
                {
                    materialViewModel.Items.Add(material);
                }
                MaterialsSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("ShowingAllMaterials"), null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
            catch (DataAccessException)
            {
                MaterialsSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("ErrorOccured"), null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
        }

        private void ClearFilter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataGridCategories.SelectedItem = null;
                categoryViewModel.SelectedItem = null;

                ShowAllMaterials();
            }
            catch (DataAccessException)
            {
                MaterialsSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("ErrorOccured"), null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            categoryViewModel.Items.Clear();
            categoryViewModel.Objects = daoCategory.GetCategories();
            foreach (Category category in categoryViewModel.Objects)
            {
                categoryViewModel.Items.Add(category);
            }

            materialViewModel.Items.Clear();
            materialViewModel.Objects = daoMaterial.GetMaterials();
            foreach (Material material in materialViewModel.Objects)
            {
                materialViewModel.Items.Add(material);
            }

            categoryViewModel.SelectedItem = null;
            DataGridCategories.SelectedItem = null;

            MaterialsSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("Refreshed"), null, null, null, false, true, TimeSpan.FromSeconds(3));
        }

        private void ApplySortButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = SortComboBox.SelectedItem as ComboBoxItem;
            if (selectedItem == null)
            {
                MaterialsSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("NoSelection"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                return;
            }

            if (materialViewModel == null) return;

            int sortIndex = int.Parse(selectedItem.Tag.ToString());

            IEnumerable<Material> sortedMaterials = materialViewModel.Objects;

            switch (sortIndex)
            {
                case 0: 
                    sortedMaterials = sortedMaterials.OrderBy(m => m.PricePerUnit);
                    break;
                case 1:
                    sortedMaterials = sortedMaterials.OrderByDescending(m => m.PricePerUnit);
                    break;
                case 2: 
                    sortedMaterials = sortedMaterials.OrderBy(m => m.CurrentStock);
                    break;
                case 3: 
                    sortedMaterials = sortedMaterials.OrderByDescending(m => m.CurrentStock);
                    break;
                default:
                    return;
            }

            materialViewModel.Items.Clear();
            foreach (var material in sortedMaterials)
            {
                materialViewModel.Items.Add(material);
            }

            MainTabControl.SelectedIndex = 1;
        }

        private void ClearSortButton_Click(object sender, RoutedEventArgs e)
        {
            SortComboBox.SelectedIndex = -1;
            RefreshMaterials();
        }

    }
}