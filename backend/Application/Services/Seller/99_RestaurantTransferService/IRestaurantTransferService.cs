namespace Application.Services.Seller._99_RestaurantTransferService;

public interface IRestaurantTransferService
{
    Task TransferDataFromGetir(string getirRestaurantId, Guid restaurantId);
    Task TransferDataFromYemekSepeti(string ysRestaurantId, Guid restaurantId);
}