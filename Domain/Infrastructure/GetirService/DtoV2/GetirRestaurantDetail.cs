namespace Domain.Infrastructure.GetirService.DtoV2;

public class GetirRestaurantDetail
{
    public class RootObject
    {
        public Data data { get; set; }
        public Result result { get; set; }
    }

    public class Data
    {
        public Restaurant restaurant { get; set; }
        public ProductCategories[] productCategories { get; set; }
    }

    public class Restaurant
    {
        public string id { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public bool isChain { get; set; }
        public string brandName { get; set; }
        public string brandSlug { get; set; }
        public string imageURL { get; set; }
        public double ratingPoint { get; set; }
        public string ratingCount { get; set; }
        public bool isFavorite { get; set; }
        public Cuisines[] cuisines { get; set; }
        public string openClosedTime { get; set; }
        public string openingClosingDate { get; set; }
        public bool isReviewEnabled { get; set; }
        public PaymentOptions[] paymentOptions { get; set; }
        public string[] infoItems { get; set; }
        public bool hasRealPhoto { get; set; }
        public string distance { get; set; }
        public string deliveryFee { get; set; }
    }

    public class Cuisines
    {
        public string id { get; set; }
        public string name { get; set; }
        public string imageURL { get; set; }
        public bool isVisibleRestaurantDetail { get; set; }
        public bool isVisibleCuisineCategory { get; set; }
    }

    public class PaymentOptions
    {
        public string title { get; set; }
        public string imageURL { get; set; }
        public bool isActive { get; set; }
    }

    public class ProductCategories
    {
        public string id { get; set; }
        public string name { get; set; }
        public Products[] products { get; set; }
    }

    public class Products
    {
        public string id { get; set; }
        public string name { get; set; }
        public string priceText { get; set; }
        public double price { get; set; }
        public string struckPriceText { get; set; }
        public double struckPrice { get; set; }
        public string description { get; set; }
        public string imageURL { get; set; }
        public string fullScreenImageURL { get; set; }
        public bool isAvailable { get; set; }
        public string restaurant { get; set; }
    }

    public class Result
    {
        public int code { get; set; }
        public string message { get; set; }
    }
}