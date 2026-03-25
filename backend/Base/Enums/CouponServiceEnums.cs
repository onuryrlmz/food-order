namespace Base.Enums;

public static class CouponServiceEnums
{
    #region Coupon Type Enums

    public enum CouponTypeEnums : short
    {
        Percentage = 1, // Yüzde indirim
        FixedAmount = 2, // Sabit tutar indirimi
        BuyXGetY = 3 // X al Y öde
    }

    public static short GetCouponTypeEnumId(CouponTypeEnums couponTypeEnums)
    {
        return (short)couponTypeEnums;
    }

    #endregion

    #region Coupon Applicable Type Enums

    public enum CouponApplicableTypeEnums : short
    {
        AllItems = 1, // Tüm ürünlerde geçerli
        SpecificMenus = 2, // Belirli menülerde geçerli
        SpecificCategories = 3 // Belirli kategorilerde geçerli
    }

    public static short GetCouponApplicableTypeEnumId(CouponApplicableTypeEnums couponApplicableTypeEnums)
    {
        return (short)couponApplicableTypeEnums;
    }

    #endregion
}