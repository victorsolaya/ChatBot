namespace LuisBot
{
    using System;
    using Microsoft.Bot.Builder.FormFlow;

    [Serializable]
    public class MachineSupportQuery
    {
        [Prompt("What is the error message displayed?")]
        public string ErrorMessage { get; set; }

        [Prompt("Is your machine switching on?")]
        public bool IsSwitchingOn { get; set; }

        [Prompt("Please write some details on what is your issue")]
        public string Details { get; set; }
    }
}