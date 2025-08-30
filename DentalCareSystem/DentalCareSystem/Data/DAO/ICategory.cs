using DentalCareSystem.Data.DTO;

namespace DentalCareSystem.Data.DAO
{
    interface ICategory
    {
        List<Category> GetCategories();
        bool AddCategory(Category category);
        void UpdateCategory(Category category);
        bool DeleteCategory(int categoryId);
    }
}
