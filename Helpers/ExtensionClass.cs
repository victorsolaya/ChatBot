using LuisBot.Helpers;
using System;
using System.Reflection;

public static class ExtensionClass
{
    public static string GetStringValue(this Enum value)
    {
        Type type = value.GetType();
        FieldInfo fieldInfo = type.GetField(value.ToString());
        // Get the stringvalue attributes
        EnumStringAttribute[] attribs = fieldInfo.GetCustomAttributes(
             typeof(EnumStringAttribute), false) as EnumStringAttribute[];
        // Return the first if there was a match.
        return attribs.Length > 0 ? attribs[0].StringValue : null;
    }
}