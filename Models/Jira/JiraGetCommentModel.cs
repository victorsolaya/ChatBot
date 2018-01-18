
using Newtonsoft.Json.Linq;

namespace LuisBot.Models
{
    public class JiraGetCommentModel
    {
        public JiraGetCommentModel() { }
        public JiraGetCommentModel(string json, bool isJson = true)
        {
            if (isJson)
            {
                JObject jObject = JObject.Parse(json);

                self = (string)jObject["self"];
                id = (string)jObject["id"];
                body = (string)jObject["body"];
                created = (string)jObject["created"];
                resolution = (string)jObject["resolution"];
                status = (string)jObject["status"];

                this.author = new Author();
                JToken jUser = jObject["author"];
                author.name = (string)jUser["name"];
                author.displayName = (string)jUser["displayName"];
            }
            else
            {
                body = json;
            }
        }

        public string self;
        public string resolution;
        public string status;
        public string id;
        public string body;
        public string created;
        public string updated;
        public Author updateAuthor;
        public Author author;
    }

    public class JiraGetStatusModel
    {
        public JiraGetStatusModel() { }
        public JiraGetStatusModel(string json, bool isJson = true)
        {
            if (isJson)
            {
                JObject jObject = JObject.Parse(json);
                status = (string)jObject["status"]["name"];
            }
        }
        public string status;
    }

    public class JiraGetResolutionModel
    {
        public JiraGetResolutionModel() { }
        public JiraGetResolutionModel(string json, bool isJson = true)
        {
            if (isJson)
            {
                JObject jObject = JObject.Parse(json);
                resolution = (string)jObject["resolution"]["name"];
            }
        }
        public string resolution;
    }

    public class Author
    {
        public string self;
        public string name;
        public string key;
        public string accountId;
        public string emailAddress;
        public string displayName;
        public string activity;
        public string timeZone;
    }
}