namespace LuisBot
{
    using System;
    using Microsoft.Bot.Builder.FormFlow;

    [Serializable]
    public class TimeSheetsQuery
    {
        [Prompt("Enter jobcode you want to assign timesheet entry to")]
        public string JobCode { get; set; }

        [Prompt("Enter the timesheet entry date")]
        public DateTime EntryDate { get; set; }

        [Prompt("Number of hours to allocate")]
        public string NumberOfHours { get; set; }

        [Prompt("Additional comments")]
        public string Comments { get; set; }
    }
}