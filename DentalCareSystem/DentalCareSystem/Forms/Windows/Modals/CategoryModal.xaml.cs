using DentalCareSystem.Data.DTO;
using DentalCareSystem.Util;
using System.Windows;

namespace DentalCareSystem.Forms.Windows.Modals
{
    public partial class CategoryModal : Window
    {
        public bool CategoryAdded { get; set; } = false;
        public bool CategoryUpdated { get; set; } = false;
        private readonly bool isEditMode;
        private readonly Category newCategory = new Category();

        public CategoryModal()
        {
            InitializeComponent();
            isEditMode = false;
            this.DataContext = newCategory;
            LocalizationProvider.UpdateAllObjects();
            ModalTitle.Content = LocalizationProvider.GetLocalizedString("AddCategoryTitle");
        }

        public CategoryModal(Category category)
        {
            InitializeComponent();
            isEditMode = true;
            this.DataContext = category;
            LocalizationProvider.UpdateAllObjects();
            ModalTitle.Content = LocalizationProvider.GetLocalizedString("UpdateCategoryTitle");
        }

        public Category GetCategory()
        {
            return this.DataContext as Category;
        }

        private void AddOrUpdate_Click(object sender, RoutedEventArgs e)
        {
            var category = this.DataContext as Category;
            if (string.IsNullOrEmpty(category.Name))
            {
                CategoryModalSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("FillFields"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                return;
            }

            if (isEditMode)
            {
                CategoryUpdated = true;
            }
            else
            {
                CategoryAdded = true;
            }

            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            CategoryAdded = false;
            CategoryUpdated = false;
            this.Close();
        }
    }
}