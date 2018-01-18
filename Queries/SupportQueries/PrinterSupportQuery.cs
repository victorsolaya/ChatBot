namespace LuisBot
{
    using System;
    using Microsoft.Bot.Builder.FormFlow;

    [Serializable]
    public class PrinterSupportQuery
    {
        public bool IsPrinterAvailable { get; set; }

        public bool FixedAfterTroubleshoot { get; set; }

        public string Description { get; set; }
    }
}