using LuisBot.Helpers;

namespace LuisBot.Enums
{
    public enum ChoicesTypes
    {
        [EnumStringAttribute("Start accomodation")]
        StartAccomodation,
        [EnumStringAttribute("Hotel reviews")]
        HotelReviews,
        [EnumStringAttribute("Search for hotels")]
        SearchHotels,
        [EnumStringAttribute("Get some support")]
        Support,
        [EnumStringAttribute("Timesheets")]
        Timesheets
    }
}