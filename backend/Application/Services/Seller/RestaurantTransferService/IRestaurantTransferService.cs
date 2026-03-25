namespace Application.Services.Seller.RestaurantTransferService;

public interface IRestaurantTransferService
{
    Task TransferDataFromGetir(string getirRestaurantId, Guid restaurantId);
    Task TransferDataFromYemekSepeti(string ysRestaurantId, Guid restaurantId);
}