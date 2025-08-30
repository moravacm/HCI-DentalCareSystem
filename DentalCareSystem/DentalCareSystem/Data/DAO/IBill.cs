using DentalCareSystem.Data.DTO;

namespace DentalCareSystem.Data.DAO
{
    interface IBill
    {
        int CreateBill(int orderId);
        List<Bill> GetBills();
        DetailedBill GetBill(int billId);
    }
}
