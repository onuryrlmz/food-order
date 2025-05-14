namespace Base.Enums;

public static class FoodCatalogServiceEnums
{
    #region Product Type Enums

    public enum ProductTypeEnums : short
    {
        Master = 1,
        Sub = 2
    }

    public static short GetProductTypeEnumId(ProductTypeEnums productTypeEnums)
    {
        return (short)productTypeEnums;
    }

    #endregion

    #region ScheduleJobEnums

    public enum ScheduleJobEnums
    {
        RenewRestaurantInformation = 1,
        RenewMenuInformation = 2
    }

    public static int GetScheduleJobEnumId(ScheduleJobEnums scheduleJobEnums)
    {
        return (int)scheduleJobEnums;
    }

    #endregion
}