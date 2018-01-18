namespace LuisBot
{
    using System;
    using Microsoft.Bot.Builder.FormFlow;

    [Serializable]
    public class BookAccommodationQuery
    {
        [Prompt("When do you want to check in?")]
        public DateTime CheckInDate { get; set; }

        [Prompt("How many nights you want to stay in the hotel?")]
        public int NumberOfNights { get; set; }

        [Prompt("What is the preferred location?")]
        public string Location { get; set; }

        [Prompt("Is this travel for a specific project code? (Type N/A if not applicable)")]
        public string ProjectCode { get; set; }
    }
}