namespace LuisBot
{
    using System;

    [Serializable]
    public class TimeSheetEntry
    {
        public string IsChargable { get; set; }

        public string JobCode { get; set; }

        public DateTime EntryDate { get; set; }

        public decimal NumberOfHours { get; set; }

        public string EntryType { get;  set; }

        public string Comments { get;  set; }

        public string FullName { get; set; }
    }
}