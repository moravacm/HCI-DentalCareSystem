namespace DentalCareSystem.Data.DTO
{
    public class Material
    {
        public int MaterialId { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal MinimumStock { get; set; }
        public decimal CurrentStock { get; set; }

        public override string ToString()
        {
            return $"{MaterialId}, {Category}, {Name}, {Unit}, {PricePerUnit}, {MinimumStock}, {CurrentStock}";
        }
    }
 }
