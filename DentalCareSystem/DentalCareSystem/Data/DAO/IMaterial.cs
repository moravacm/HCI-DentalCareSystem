using DentalCareSystem.Data.DTO;

namespace DentalCareSystem.Data.DAO
{
    interface IMaterial
    {
        List<Material> GetMaterials();
        bool AddMaterial(Material material);

        bool DeleteMaterial(int materialId);

        void UpdateMaterial(Material material);
    }
}
