namespace LuisBot
{
    using System;
    using Microsoft.Bot.Builder.FormFlow;

    [Serializable]
    public class AddIncidentCommentQuery
    {
        [Prompt("What is your comment?")]
        public string Comment { get; set; }

        [Prompt("Which ticket you want to assign this worklog to?")]
        [Optional]
        public string TicketId { get; set; }
    }
}