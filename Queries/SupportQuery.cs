namespace LuisBot
{
    using System;
    using Microsoft.Bot.Builder.FormFlow;

    [Serializable]
    public class SupportQuery
    {
        [Prompt("When did this issue start? (DD/MM/YYYY)")]
        public DateTime DateStarted { get; set; }

        [Prompt("Which email address can I contact you on?")]
        public string EmailAddress { get; set; }

        [Prompt("Please, write some details on what is your issue")]
        public string Details { get; set; }

        [Prompt("Please, write your ticket number")]
        public string TicketNumber { get; set; }
    }
}