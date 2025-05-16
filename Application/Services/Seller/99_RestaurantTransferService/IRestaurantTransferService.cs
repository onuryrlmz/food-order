namespace Application.Services.Seller._99_RestaurantTransferService;

public interface IRestaurantTransferService
{
    Task SaveData(string getirRestaurantId, Guid restaurantId);
}