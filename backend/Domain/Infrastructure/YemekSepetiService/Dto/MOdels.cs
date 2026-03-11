using Newtonsoft.Json;

namespace Domain.Infrastructure.YemekSepetiService.Dto;

public partial class YemekSepetiDto
{
    [JsonProperty("status_code")]
    public long status_code { get; set; }

    [JsonProperty("data")]
    public Data data { get; set; }


    public partial class Data
    {
        [JsonProperty("chain")]
        public Chain chain { get; set; }

        [JsonProperty("city")]
        public City city { get; set; }

        [JsonProperty("customer_phone")]
        public string customer_phone { get; set; }

        [JsonProperty("id")]
        public long id { get; set; }

        [JsonProperty("code")]
        public string code { get; set; }

        [JsonProperty("accepts_instructions")]
        public bool accepts_instructions { get; set; }

        [JsonProperty("address")]
        public string address { get; set; }

        [JsonProperty("address_line2")]
        public string address_line2 { get; set; }

        [JsonProperty("budget")]
        public long budget { get; set; }

        [JsonProperty("deals")]
        public List<Deal> deals { get; set; }

        [JsonProperty("constraints")]
        public List<object> constraints { get; set; }

        [JsonProperty("cuisines")]
        public List<City> cuisines { get; set; }

        [JsonProperty("delivery_duration_range")]
        public object delivery_duration_range { get; set; }

        [JsonProperty("pickup_duration_range")]
        public object pickup_duration_range { get; set; }

        [JsonProperty("delivery_fee_source")]
        public string delivery_fee_source { get; set; }

        [JsonProperty("description")]
        public string description { get; set; }

        [JsonProperty("discounts")]
        public List<Discount> discounts { get; set; }

        [JsonProperty("distance")]
        public decimal distance { get; set; }

        [JsonProperty("experiments")]
        public List<Experiment> experiments { get; set; }

        [JsonProperty("favorite")]
        public object favorite { get; set; }

        [JsonProperty("has_delivery_provider")]
        public bool has_delivery_provider { get; set; }

        [JsonProperty("hero_image")]
        public Uri hero_image { get; set; }

        [JsonProperty("hero_listing_image")]
        public Uri hero_listing_image { get; set; }

        [JsonProperty("is_active")]
        public bool is_active { get; set; }

        [JsonProperty("is_delivery_enabled")]
        public bool is_delivery_enabled { get; set; }

        [JsonProperty("is_new_until")]
        public DateTimeOffset is_new_until { get; set; }

        [JsonProperty("is_pickup_enabled")]
        public bool is_pickup_enabled { get; set; }

        [JsonProperty("is_premium")]
        public bool is_premium { get; set; }

        [JsonProperty("is_preorder_enabled")]
        public bool is_preorder_enabled { get; set; }

        [JsonProperty("is_promoted")]
        public bool is_promoted { get; set; }

        [JsonProperty("is_test")]
        public bool is_test { get; set; }

        [JsonProperty("is_vat_disabled")]
        public bool is_vat_disabled { get; set; }

        [JsonProperty("is_vat_included_in_product_price")]
        public bool is_vat_included_in_product_price { get; set; }

        [JsonProperty("is_vat_visible")]
        public bool is_vat_visible { get; set; }

        [JsonProperty("is_vat_included")]
        public bool is_vat_included { get; set; }

        [JsonProperty("is_voucher_enabled")]
        public bool is_voucher_enabled { get; set; }

        [JsonProperty("is_super_vendor")]
        public bool is_super_vendor { get; set; }

        [JsonProperty("latitude")]
        public decimal latitude { get; set; }

        [JsonProperty("location")]
        public string location { get; set; }

        [JsonProperty("location_event")]
        public object location_event { get; set; }

        [JsonProperty("logo")]
        public Uri logo { get; set; }

        [JsonProperty("longitude")]
        public decimal longitude { get; set; }

        [JsonProperty("loyalty_percentage_amount")]
        public long loyalty_percentage_amount { get; set; }

        [JsonProperty("loyalty_program_enabled")]
        public bool loyalty_program_enabled { get; set; }

        [JsonProperty("minimum_delivery_fee")]
        public long minimum_delivery_fee { get; set; }

        [JsonProperty("minimum_delivery_time")]
        public long minimum_delivery_time { get; set; }

        [JsonProperty("minimum_order_amount")]
        public long minimum_order_amount { get; set; }

        [JsonProperty("minimum_pickup_time")]
        public long minimum_pickup_time { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("original_delivery_fee")]
        public long original_delivery_fee { get; set; }

        [JsonProperty("payment_types")]
        public List<object> payment_types { get; set; }

        [JsonProperty("post_code")]
        public string post_code { get; set; }

        [JsonProperty("primary_cuisine_id")]
        public long primary_cuisine_id { get; set; }

        [JsonProperty("rating")]
        public decimal rating { get; set; }

        [JsonProperty("topic_ratings")]
        public List<TopicRating> topic_ratings { get; set; }

        [JsonProperty("review_number")]
        public long review_number { get; set; }

        [JsonProperty("review_with_comment_number")]
        public long review_with_comment_number { get; set; }

        [JsonProperty("schedules")]
        public List<Schedule> schedules { get; set; }

        [JsonProperty("service_fee")]
        public long service_fee { get; set; }

        [JsonProperty("service_fee_percentage_amount")]
        public long service_fee_percentage_amount { get; set; }

        [JsonProperty("short_name")]
        public string short_name { get; set; }

        [JsonProperty("small_order_fee")]
        public object small_order_fee { get; set; }

        [JsonProperty("special_days")]
        public List<object> special_days { get; set; }

        [JsonProperty("tag")]
        public string tag { get; set; }

        [JsonProperty("tags")]
        public List<Tag> tags { get; set; }

        [JsonProperty("trade_register_number")]
        public string trade_register_number { get; set; }

        [JsonProperty("url_key")]
        public string url_key { get; set; }

        [JsonProperty("characteristics")]
        public Characteristics characteristics { get; set; }

        [JsonProperty("vendor_legal_information")]
        public VendorLegalInformation vendor_legal_information { get; set; }

        [JsonProperty("vertical")]
        public string vertical { get; set; }

        [JsonProperty("vertical_segment")]
        public string vertical_segment { get; set; }

        [JsonProperty("vertical_parent")]
        public string vertical_parent { get; set; }

        [JsonProperty("web_path")]
        public Uri web_path { get; set; }

        [JsonProperty("is_partner_cashback_disabled")]
        public bool is_partner_cashback_disabled { get; set; }

        [JsonProperty("vertical_type_ids")]
        public List<string> vertical_type_ids { get; set; }

        [JsonProperty("other_vendors_in_chain")]
        public long other_vendors_in_chain { get; set; }

        [JsonProperty("menus")]
        public List<Menu> menus { get; set; }

        [JsonProperty("metadata")]
        public DataMetadata metadata { get; set; }

        [JsonProperty("loyalty_programs")]
        public List<LoyaltyProgram> loyalty_programs { get; set; }
    }

    public partial class Chain
    {
        [JsonProperty("id")]
        public long id { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("main_vendor_id")]
        public long main_vendor_id { get; set; }

        [JsonProperty("url_key")]
        public string url_key { get; set; }

        [JsonProperty("code")]
        public string code { get; set; }
    }

    public partial class Characteristics
    {
        [JsonProperty("cuisines")]
        public List<City> cuisines { get; set; }

        [JsonProperty("food_characteristics")]
        public List<FoodCharacteristic> food_characteristics { get; set; }

        [JsonProperty("primary_cuisine")]
        public City primary_cuisine { get; set; }
    }

    public partial class City
    {
        [JsonProperty("id")]
        public long id { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("url_key")]
        public string url_key { get; set; }

        [JsonProperty("main", NullValueHandling = NullValueHandling.Ignore)]
        public bool? main { get; set; }
    }

    public partial class FoodCharacteristic
    {
        [JsonProperty("id")]
        public long id { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("is_halal")]
        public bool is_halal { get; set; }

        [JsonProperty("is_vegetarian")]
        public bool is_vegetarian { get; set; }
    }

    public partial class Deal
    {
        [JsonProperty("id")]
        public long id { get; set; }

        [JsonProperty("code")]
        public string code { get; set; }

        [JsonProperty("offer_type")]
        public string offer_type { get; set; }

        [JsonProperty("type")]
        public string type { get; set; }

        [JsonProperty("voucher_type")]
        public string voucher_type { get; set; }

        [JsonProperty("minimum_order_value")]
        public long minimum_order_value { get; set; }

        [JsonProperty("maximum_discount_amount")]
        public long maximum_discount_amount { get; set; }

        [JsonProperty("value")]
        public long value { get; set; }

        [JsonProperty("title")]
        public string title { get; set; }

        [JsonProperty("description")]
        public string description { get; set; }

        [JsonProperty("start_date")]
        public DateTimeOffset start_date { get; set; }

        [JsonProperty("end_date")]
        public DateTimeOffset end_date { get; set; }

        [JsonProperty("is_pro")]
        public bool is_pro { get; set; }

        [JsonProperty("is_new_customer")]
        public bool is_new_customer { get; set; }

        [JsonProperty("seq_priority")]
        public long seq_priority { get; set; }

        [JsonProperty("additional_info")]
        public AdditionalInfo additional_info { get; set; }

        [JsonProperty("tc")]
        public List<string> tc { get; set; }

        [JsonProperty("action")]
        public Action action { get; set; }

        [JsonProperty("source")]
        public string source { get; set; }

        [JsonProperty("conditions")]
        public List<Condition> conditions { get; set; }

        [JsonProperty("quantity")]
        public long quantity { get; set; }

        [JsonProperty("used_quantity")]
        public long used_quantity { get; set; }

        [JsonProperty("is_full_basket_applicable")]
        public bool is_full_basket_applicable { get; set; }

        [JsonProperty("construct_id")]
        public string construct_id { get; set; }

        [JsonProperty("is_bogo")]
        public bool is_bogo { get; set; }
    }

    public partial class Action
    {
        [JsonProperty("type")]
        public string type { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("description")]
        public string description { get; set; }
    }

    public partial class AdditionalInfo
    {
        [JsonProperty("should_override")]
        public List<string> should_override { get; set; }

        [JsonProperty("calculation")]
        public Calculation calculation { get; set; }
    }

    public partial class Calculation
    {
        [JsonProperty("baseOn")]
        public List<string> baseOn { get; set; }
    }

    public partial class Condition
    {
        [JsonProperty("discount_type")]
        public string discount_type { get; set; }

        [JsonProperty("discount_amount")]
        public decimal discount_amount { get; set; }

        [JsonProperty("condition_type")]
        public string condition_type { get; set; }

        [JsonProperty("condition_object_array")]
        public List<long> condition_object_array { get; set; }

        [JsonProperty("condition_object_names")]
        public List<string> condition_object_names { get; set; }
    }

    public partial class Discount
    {
        [JsonProperty("banner_title")]
        public string banner_title { get; set; }

        [JsonProperty("bogo_product_blocks")]
        public List<object> bogo_product_blocks { get; set; }

        [JsonProperty("bogo_discount_unit")]
        public string bogo_discount_unit { get; set; }

        [JsonProperty("closing_time")]
        public string closing_time { get; set; }

        [JsonProperty("condition_type")]
        public string condition_type { get; set; }

        [JsonProperty("condition_object")]
        public long condition_object { get; set; }

        [JsonProperty("condition_object_array")]
        public List<object> condition_object_array { get; set; }

        [JsonProperty("discount_amount")]
        public long discount_amount { get; set; }

        [JsonProperty("discount_type")]
        public string discount_type { get; set; }

        [JsonProperty("conditions")]
        public List<Condition> conditions { get; set; }

        [JsonProperty("daily_limit")]
        public long daily_limit { get; set; }

        [JsonProperty("description")]
        public string description { get; set; }

        [JsonProperty("discount_text")]
        public string discount_text { get; set; }

        [JsonProperty("end_date")]
        public DateTimeOffset end_date { get; set; }

        [JsonProperty("file_name")]
        public string file_name { get; set; }

        [JsonProperty("id")]
        public long id { get; set; }

        [JsonProperty("maximum_discount_amount")]
        public long maximum_discount_amount { get; set; }

        [JsonProperty("minimum_order_value")]
        public long minimum_order_value { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("opening_time")]
        public string opening_time { get; set; }

        [JsonProperty("platform_ratio")]
        public long platform_ratio { get; set; }

        [JsonProperty("promotional_limit")]
        public long promotional_limit { get; set; }

        [JsonProperty("start_date")]
        public DateTimeOffset start_date { get; set; }

        [JsonProperty("vendor_id")]
        public long vendor_id { get; set; }

        [JsonProperty("labels")]
        public List<string> labels { get; set; }

        [JsonProperty("label_metadata")]
        public LabelMetadata label_metadata { get; set; }

        [JsonProperty("additional_info")]
        public AdditionalInfo additional_info { get; set; }
    }

    public partial class LabelMetadata
    {
        [JsonProperty("special_menu")]
        public SpecialMenu special_menu { get; set; }
    }

    public partial class SpecialMenu
    {
        [JsonProperty("disco_aggregate")]
        public bool disco_aggregate { get; set; }

        [JsonProperty("partners")]
        public List<PartnerElement> partners { get; set; }
    }

    public partial class PartnerElement
    {
        [JsonProperty("menu_category_id")]
        public string menu_category_id { get; set; }

        [JsonProperty("partner_id")]
        public string partner_id { get; set; }

        [JsonProperty("product_id")]
        public string product_id { get; set; }
    }

    public partial class Experiment
    {
        [JsonProperty("experiment_id")]
        public string experiment_id { get; set; }

        [JsonProperty("experiment_variation")]
        public string experiment_variation { get; set; }

        [JsonProperty("is_participating")]
        public bool is_participating { get; set; }
    }

    public partial class LoyaltyProgram
    {
        [JsonProperty("program")]
        public string program { get; set; }

        [JsonProperty("config")]
        public List<Config> config { get; set; }
    }

    public partial class Config
    {
        [JsonProperty("field_id")]
        public string field_id { get; set; }

        [JsonProperty("field_type")]
        public string field_type { get; set; }

        [JsonProperty("field_value")]
        public bool field_value { get; set; }
    }

    public partial class Menu
    {
        [JsonProperty("ab_sorting_applied")]
        public bool ab_sorting_applied { get; set; }

        [JsonProperty("id")]
        public long id { get; set; }

        [JsonProperty("code")]
        public string code { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("description")]
        public string description { get; set; }

        [JsonProperty("type")]
        public string type { get; set; }

        [JsonProperty("opening_time")]
        public DateTimeOffset opening_time { get; set; }

        [JsonProperty("closing_time")]
        public DateTimeOffset closing_time { get; set; }

        [JsonProperty("menu_categories")]
        public List<MenuCategory> menu_categories { get; set; }

        [JsonProperty("toppings")]
        public Dictionary<string, Topping> toppings { get; set; }

        [JsonProperty("tags")]
        public Tags tags { get; set; }

        [JsonProperty("default_sold_out_options")]
        public List<DefaultSoldOutOption> default_sold_out_options { get; set; }
    }

    public partial class DefaultSoldOutOption
    {
        [JsonProperty("default")]
        public bool @default { get; set; }

        [JsonProperty("option")]
        public string option { get; set; }

        [JsonProperty("text")]
        public string text { get; set; }
    }

    public partial class MenuCategory
    {
        [JsonProperty("id")]
        public long id { get; set; }

        [JsonProperty("code")]
        public Guid code { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("description")]
        public string description { get; set; }

        [JsonProperty("products")]
        public List<ProductElement> products { get; set; }

        [JsonProperty("partner", NullValueHandling = NullValueHandling.Ignore)]
        public MenuCategoryPartner partner { get; set; }

        [JsonProperty("is_popular_category")]
        public bool is_popular_category { get; set; }
    }

    public partial class MenuCategoryPartner
    {
        [JsonProperty("id")]
        public long id { get; set; }

        [JsonProperty("code")]
        public string code { get; set; }

        [JsonProperty("title")]
        public string title { get; set; }

        [JsonProperty("images")]
        public Images images { get; set; }
    }

    public partial class Images
    {
        [JsonProperty("category")]
        public Category category { get; set; }
    }

    public partial class Category
    {
        [JsonProperty("web")]
        public Uri web { get; set; }

        [JsonProperty("mobile")]
        public Uri mobile { get; set; }
    }

    public partial class ProductElement
    {
        [JsonProperty("id")]
        public long id { get; set; }

        [JsonProperty("code")]
        public Guid code { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("description")]
        public string description { get; set; }

        [JsonProperty("master_category_id")]
        public long master_category_id { get; set; }

        [JsonProperty("file_path")]
        public string file_path { get; set; }

        [JsonProperty("is_sold_out")]
        public bool is_sold_out { get; set; }

        [JsonProperty("is_express_item")]
        public bool is_express_item { get; set; }

        [JsonProperty("additives")]
        public List<object> additives { get; set; }

        [JsonProperty("is_alcoholic_item")]
        public bool is_alcoholic_item { get; set; }

        [JsonProperty("sold_out_options")]
        public object sold_out_options { get; set; }

        [JsonProperty("dietary_attributes")]
        public DietaryAttributes dietary_attributes { get; set; }

        [JsonProperty("product_variations")]
        public List<PurpleProductVariation> product_variations { get; set; }

        [JsonProperty("half_type")]
        public string half_type { get; set; }

        [JsonProperty("tags", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> tags { get; set; }

        [JsonProperty("is_bundle")]
        public bool is_bundle { get; set; }

        [JsonProperty("most_liked", NullValueHandling = NullValueHandling.Ignore)]
        public MostLiked most_liked { get; set; }
    }

    public partial class DietaryAttributes
    {
    }

    public partial class MostLiked
    {
        [JsonProperty("likes")]
        public long likes { get; set; }

        [JsonProperty("total_count")]
        public long total_count { get; set; }

        [JsonProperty("percentage")]
        public long percentage { get; set; }
    }

    public partial class PurpleProductVariation
    {
        [JsonProperty("id")]
        public long id { get; set; }

        [JsonProperty("code")]
        public Guid code { get; set; }

        [JsonProperty("remote_code")]
        public string remote_code { get; set; }

        [JsonProperty("container_price")]
        public long container_price { get; set; }

        [JsonProperty("price")]
        public decimal price { get; set; }

        [JsonProperty("price_before_discount", NullValueHandling = NullValueHandling.Ignore)]
        public long? price_before_discount { get; set; }

        [JsonProperty("topping_ids")]
        public List<long> topping_ids { get; set; }

        [JsonProperty("topping_properties")]
        public List<ToppingProperty> topping_properties { get; set; }

        [JsonProperty("unit_pricing")]
        public object unit_pricing { get; set; }

        [JsonProperty("total_price")]
        public long total_price { get; set; }

        [JsonProperty("dietary_attributes")]
        public DietaryAttributes dietary_attributes { get; set; }
    }

    public partial class ToppingProperty
    {
        [JsonProperty("id")]
        public long id { get; set; }

        [JsonProperty("use_original_price", NullValueHandling = NullValueHandling.Ignore)]
        public bool? use_original_price { get; set; }
    }

    public partial class Tags
    {
        [JsonProperty("popular")]
        public Popular popular { get; set; }
    }

    public partial class Popular
    {
        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("translation_keys")]
        public TranslationKeys translation_keys { get; set; }

        [JsonProperty("elements")]
        public List<string> elements { get; set; }

        [JsonProperty("metadata")]
        public PopularMetadata metadata { get; set; }
    }

    public partial class PopularMetadata
    {
        [JsonProperty("sorting")]
        public List<long> sorting { get; set; }
    }

    public partial class TranslationKeys
    {
        [JsonProperty("description")]
        public string description { get; set; }

        [JsonProperty("title")]
        public string title { get; set; }
    }

    public partial class Topping
    {
        [JsonProperty("id")]
        public long id { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("description")]
        public string description { get; set; }

        [JsonProperty("quantity_minimum")]
        public int quantity_minimum { get; set; }

        [JsonProperty("quantity_maximum")]
        public int quantity_maximum { get; set; }

        [JsonProperty("options")]
        public List<Option> options { get; set; }

        [JsonProperty("type")]
        public string type { get; set; }
    }

    public partial class Option
    {
        [JsonProperty("id")]
        public long id { get; set; }

        [JsonProperty("product_id")]
        public long product_id { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("description")]
        public string description { get; set; }

        [JsonProperty("price")]
        public long price { get; set; }

        [JsonProperty("remote_code")]
        public string remote_code { get; set; }

        [JsonProperty("product", NullValueHandling = NullValueHandling.Ignore)]
        public OptionProduct product { get; set; }

        [JsonProperty("price_map", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, long> price_map { get; set; }

        [JsonProperty("vat_percentage", NullValueHandling = NullValueHandling.Ignore)]
        public long? vat_percentage { get; set; }

        [JsonProperty("variation_tag_map", NullValueHandling = NullValueHandling.Ignore)]
        public VariationTagMap variation_tag_map { get; set; }
    }

    public partial class OptionProduct
    {
        [JsonProperty("id")]
        public long id { get; set; }

        [JsonProperty("code")]
        public Guid code { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("description")]
        public string description { get; set; }

        [JsonProperty("master_category_id")]
        public long master_category_id { get; set; }

        [JsonProperty("file_path")]
        public string file_path { get; set; }

        [JsonProperty("is_sold_out")]
        public bool is_sold_out { get; set; }

        [JsonProperty("is_express_item")]
        public bool is_express_item { get; set; }

        [JsonProperty("additives")]
        public List<object> additives { get; set; }

        [JsonProperty("is_alcoholic_item")]
        public bool is_alcoholic_item { get; set; }

        [JsonProperty("sold_out_options")]
        public object sold_out_options { get; set; }

        [JsonProperty("dietary_attributes")]
        public DietaryAttributes dietary_attributes { get; set; }

        [JsonProperty("product_variations")]
        public List<FluffyProductVariation> product_variations { get; set; }

        [JsonProperty("half_type")]
        public string half_type { get; set; }

        [JsonProperty("is_bundle")]
        public bool is_bundle { get; set; }

        [JsonProperty("tags", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> tags { get; set; }
    }

    public partial class FluffyProductVariation
    {
        [JsonProperty("id")]
        public long id { get; set; }

        [JsonProperty("code")]
        public Guid code { get; set; }

        [JsonProperty("remote_code")]
        public string remote_code { get; set; }

        [JsonProperty("container_price")]
        public long container_price { get; set; }

        [JsonProperty("price")]
        public decimal price { get; set; }

        [JsonProperty("topping_ids")]
        public List<long> topping_ids { get; set; }

        [JsonProperty("topping_properties")]
        public List<ToppingProperty> topping_properties { get; set; }

        [JsonProperty("unit_pricing")]
        public object unit_pricing { get; set; }

        [JsonProperty("total_price")]
        public long total_price { get; set; }

        [JsonProperty("dietary_attributes")]
        public DietaryAttributes dietary_attributes { get; set; }
    }

    public partial class VariationTagMap
    {
        [JsonProperty("popular")]
        public List<long> popular { get; set; }
    }

    public partial class DataMetadata
    {
        [JsonProperty("is_delivery_available")]
        public bool is_delivery_available { get; set; }

        [JsonProperty("is_pickup_available")]
        public bool is_pickup_available { get; set; }

        [JsonProperty("available_in")]
        public string available_in { get; set; }

        [JsonProperty("has_discount")]
        public bool has_discount { get; set; }

        [JsonProperty("timezone")]
        public string timezone { get; set; }

        [JsonProperty("events")]
        public List<object> events { get; set; }

        [JsonProperty("close_reasons")]
        public List<object> close_reasons { get; set; }

        [JsonProperty("is_flood_feature_closed")]
        public bool is_flood_feature_closed { get; set; }
    }

    public partial class Schedule
    {
        [JsonProperty("id")]
        public long id { get; set; }

        [JsonProperty("weekday")]
        public long weekday { get; set; }

        [JsonProperty("opening_type")]
        public string opening_type { get; set; }

        [JsonProperty("opening_time")]
        public string opening_time { get; set; }

        [JsonProperty("closing_time")]
        public string closing_time { get; set; }
    }

    public partial class Tag
    {
        [JsonProperty("code")]
        public string code { get; set; }

        [JsonProperty("text")]
        public string text { get; set; }
    }

    public partial class TopicRating
    {
        [JsonProperty("topic")]
        public string topic { get; set; }

        [JsonProperty("score")]
        public decimal score { get; set; }
    }

    public partial class VendorLegalInformation
    {
        [JsonProperty("legal_name")]
        public string legal_name { get; set; }

        [JsonProperty("address_line_1")]
        public string address_line_1 { get; set; }

        [JsonProperty("trade_register_number")]
        public string trade_register_number { get; set; }
    }
}