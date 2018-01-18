using LuisBot.Helpers;

namespace LuisBot.Enums.Support
{
    public enum GetTicketOptions
    {
        [EnumStringAttribute("Get last comment")]
        GetLastComment,
        [EnumStringAttribute("Get url")]
        GetUrl,
        [EnumStringAttribute("Get status")]
        GetStatus,
        [EnumStringAttribute("Get resolution")]
        GetResolution
    }
}