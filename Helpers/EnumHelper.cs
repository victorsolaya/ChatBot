using LuisBot.Enums;
using LuisBot.Enums.Support;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuisBot.Helpers
{
    public class EnumHelper
    {
        //You have to create a Enum...AttributeSttring to translate with description given in enum class
        #region TRANSLATE ENUM
        public IEnumerable<string> GetEnumChoiceAttributeString()
        {
            //This return the custom attribute string value.
            string[] choices = Enum.GetNames(typeof(LuisBot.Enums.ChoicesTypes));
            List<string> choicesList = new List<string>();
            foreach (string choice in choices)
            {
                ChoicesTypes onechoice = (ChoicesTypes)Enum.Parse(typeof(ChoicesTypes), choice);
                //This return the custom attribute string value.
                choicesList.Add(onechoice.GetStringValue());
            }
            IEnumerable<string> choicesEnum = choicesList.AsEnumerable<string>();
            return choicesEnum;
        }

        public IEnumerable<string> GetEnumSupportAttributeString()
        {
            //This return the custom attribute string value.
            string[] choices = Enum.GetNames(typeof(LuisBot.Enums.Support.SupportOptions));
            List<string> choicesList = new List<string>();
            foreach (string choice in choices)
            {
                SupportOptions onechoice = (SupportOptions)Enum.Parse(typeof(SupportOptions), choice);
                //This return the custom attribute string value.
                choicesList.Add(onechoice.GetStringValue());
            }
            IEnumerable<string> choicesEnum = choicesList.AsEnumerable<string>();
            return choicesEnum;
        }

        public IEnumerable<string> GetEnumGetTicketAttributeString()
        {
            //This return the custom attribute string value.
            string[] choices = Enum.GetNames(typeof(LuisBot.Enums.Support.GetTicketOptions));
            List<string> choicesList = new List<string>();
            foreach (string choice in choices)
            {
                GetTicketOptions onechoice = (GetTicketOptions)Enum.Parse(typeof(GetTicketOptions), choice);
                //This return the custom attribute string value.
                choicesList.Add(onechoice.GetStringValue());
            }
            IEnumerable<string> tickets = choicesList.AsEnumerable<string>();
            return tickets;
        }

        #endregion
    }

    public class EnumStringAttribute : Attribute
    {
        // This is for getting a single value.
        public EnumStringAttribute(string stringValue)
        {
            this.stringValue = stringValue;
        }
        private string stringValue;
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}