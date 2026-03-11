namespace Domain.Infrastructure.GetirService.Dto;

public class GetirProductDetail
{
    public class RootObject
    {
        public Data data { get; set; }
        public Result result { get; set; }
    }

    public class Data
    {
        public Product product { get; set; }
    }

    public class Product
    {
        public string id { get; set; }
        public string name { get; set; }
        public string priceText { get; set; }
        public double price { get; set; }
        public string description { get; set; }
        public string imageURL { get; set; }
        public string fullScreenImageURL { get; set; }
        public OptionCategories[] optionCategories { get; set; }
        public Restaurant restaurant { get; set; }
        public string oldPrice { get; set; }
    }

    public class OptionCategories
    {
        public string id { get; set; }
        public string name { get; set; }
        public int minCount { get; set; }
        public int maxCount { get; set; }
        public int categoryType { get; set; }
        public Options[] options { get; set; }
    }

    public class Options
    {
        public string id { get; set; }
        public string name { get; set; }
        public int type { get; set; }
        public OptionCategories1[] optionCategories { get; set; }
        public bool isPriceApplicable { get; set; }
        public string priceText { get; set; }
        public double price { get; set; }
    }

    public class OptionCategories1
    {
        public string id { get; set; }
        public string name { get; set; }
        public int minCount { get; set; }
        public int maxCount { get; set; }
        public int categoryType { get; set; }
        public Options2[] options { get; set; }
    }

    public class Options2
    {
        public string id { get; set; }
        public string name { get; set; }
        public int type { get; set; }
        public bool isPriceApplicable { get; set; }
        public string priceText { get; set; }
        public double price { get; set; }
    }

    public class Restaurant
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Result
    {
        public int code { get; set; }
        public string message { get; set; }
    }
}