
using Newtonsoft.Json.Linq;
using System;

namespace LuisBot.Models
{
    public class JiraIncidentModel
    {
        public JiraIncidentModel() { }
        public JiraIncidentModel(string json)
        {

            JObject jObject = JObject.Parse(json);

            self = (string)jObject["self"];
            id = (string)jObject["id"];
            key = (string)jObject["key"];

            this.fields = new Fields();
            JToken jIncident = jObject["fields"];
            fields.created = Convert.ToDateTime(jIncident["created"]);

        }

        public string id;
        public string self;
        public string key;
        public Fields fields;
    }

    public class Fields
    {
        public System.DateTime created;
    }
}