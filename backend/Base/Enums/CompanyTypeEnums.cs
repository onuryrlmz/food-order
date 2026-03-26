using System.ComponentModel;

namespace Base.Enums;

public enum CompanyTypeEnums : short
{
    [Description("Şahıs")]
    Individual = 1,

    [Description("Şirket")]
    Company = 2
}
