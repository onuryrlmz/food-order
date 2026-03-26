namespace Base.Enums;

public static class EnumLookups
{
    public static Dictionary<short, UserRoleEnums> UserRoleEnumList => Enum.GetValues(typeof(UserRoleEnums)).Cast<UserRoleEnums>().ToDictionary(t => (short)t, t => t);

    public static Dictionary<short, UserStatusEnums> UserStatusEnumList => Enum.GetValues(typeof(UserStatusEnums)).Cast<UserStatusEnums>().ToDictionary(t => (short)t, t => t);

    public static Dictionary<short, SexEnums> SexEnumList => Enum.GetValues(typeof(SexEnums)).Cast<SexEnums>().ToDictionary(t => (short)t, t => t);
}
