using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace SteamItemSeller.Util;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum enumValue)
    {
        var field = enumValue.GetType().GetField(enumValue.ToString());
        var attribute = field!.GetCustomAttributes<DisplayAttribute>().FirstOrDefault();
        return attribute?.Name ?? enumValue.ToString();
    }
}