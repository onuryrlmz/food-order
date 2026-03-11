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
}