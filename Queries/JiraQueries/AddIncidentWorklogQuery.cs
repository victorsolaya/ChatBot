namespace LuisBot
{
    using System;
    using Microsoft.Bot.Builder.FormFlow;

    [Serializable]
    public class AddIncidentWorklogQuery
    {
        [Prompt("What is the time you spent working on this incident? (in minutes)")]
        public int Time { get; set; }

        [Prompt("Do you want to leave a comment?")]
        public string Comment { get; set; }

        [Prompt("Which ticket you want to assign this worklog to?")]
        [Optional]
        public string TicketId { get; set; }
    }
}