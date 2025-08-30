using DentalCareSystem.Data.DAO.MySQL;
using DentalCareSystem.Data.DTO;
using DentalCareSystem.Util;
using System.Windows;
using System.Collections.Generic;

namespace DentalCareSystem.Forms.Windows.Modals
{
    public partial class MaterialModal : Window
    {
        public bool MaterialAdded { get; set; } = false;
        public bool MaterialUpdated { get; set; } = false;
        private readonly bool isEditMode;

        public List<Category> Categories { get; set; }
        public List<string> AvailableUnits { get; set; }

        public MaterialModal()
        {
            InitializeComponent();
            isEditMode = false;
            LoadCategories();
            LoadAvailableUnits();
            this.DataContext = new Material();
            LocalizationProvider.UpdateAllObjects();
            ModalTitle.Content = LocalizationProvider.GetLocalizedString("AddMaterialTitle");
        }

        public MaterialModal(Material material)
        {
            InitializeComponent();
            isEditMode = true;
            LoadCategories();
            LoadAvailableUnits();

            if (material.Category != null && Categories != null)
            {
                var matchingCategory = Categories.FirstOrDefault(c => c.Id == material.Category.Id);
                if (matchingCategory != null)
                {
                    material.Category = matchingCategory;
                }
            }

            this.DataContext = material;
            LocalizationProvider.UpdateAllObjects();
            ModalTitle.Content = LocalizationProvider.GetLocalizedString("UpdateMaterialTitle");
        }

        private void LoadCategories()
        {
            CategoryDAOImpl categoryDao = new CategoryDAOImpl();
            Categories = categoryDao.GetCategories();
            CategoryComboBox.ItemsSource = Categories;
        }

        private void LoadAvailableUnits()
        {
            AvailableUnits = new List<string>
            {
                "ml", "l", "g", "kg", "kom", "pak", "kutija", "flaša"
            };

            UnitComboBox.ItemsSource = AvailableUnits;
        }

        public Material GetMaterial()
        {
            return this.DataContext as Material;
        }

        private void AddOrUpdate_Click(object sender, RoutedEventArgs e)
        {
            var material = this.DataContext as Material;

            NameTextBox.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty)?.UpdateSource();
            UnitComboBox.GetBindingExpression(System.Windows.Controls.ComboBox.SelectedItemProperty)?.UpdateSource();
            CategoryComboBox.GetBindingExpression(System.Windows.Controls.ComboBox.SelectedItemProperty)?.UpdateSource();
            PricePerUnitTextBox.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty)?.UpdateSource();
            MinimumStockTextBox.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty)?.UpdateSource();
            CurrentStockTextBox.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty)?.UpdateSource();

            if (!decimal.TryParse(PricePerUnitTextBox.Text, out decimal pricePerUnit) ||
                !decimal.TryParse(MinimumStockTextBox.Text, out decimal minimumStock) ||
                !decimal.TryParse(CurrentStockTextBox.Text, out decimal currentStock))
            {
                MaterialModalSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("InvalidFormat"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                return;
            }

            material.Name = NameTextBox.Text;
            material.Unit = UnitComboBox.SelectedItem?.ToString() ?? UnitComboBox.Text;
            material.Category = CategoryComboBox.SelectedItem as Category;
            material.CategoryId = material.Category?.Id ?? 0;
            material.PricePerUnit = pricePerUnit;
            material.MinimumStock = minimumStock;
            material.CurrentStock = currentStock;

            if (string.IsNullOrEmpty(material.Name) || string.IsNullOrEmpty(material.Unit) ||
                material.Category == null || material.PricePerUnit <= 0 ||
                material.MinimumStock <= 0 || material.CurrentStock < 0)
            {
                MaterialModalSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("FillFields"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                return;
            }

            if (isEditMode)
            {
                MaterialUpdated = true;
            }
            else
            {
                MaterialAdded = true;
            }

            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            MaterialAdded = false;
            MaterialUpdated = false;
            this.Close();
        }
    }
}