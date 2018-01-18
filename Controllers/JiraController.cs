namespace LuisBot
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Configuration;
    using System.Web.Http;
    using Dialogs;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Services;
    using System.IO;
    using System.Text;
    using Newtonsoft.Json.Linq;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using LuisBot.Models;

    [BotAuthentication]
    public class JiraController : ApiController
    {
        private string RunQuery(string argument, string data = null, string method = "GET")
        {
            try
            {
                HttpWebRequest newRequest = WebRequest.Create(argument) as HttpWebRequest;
                newRequest.ContentType = "application/json";
                newRequest.Method = method;
                if (data != null)
                {
                    using (StreamWriter writer = new StreamWriter(newRequest.GetRequestStream()))
                    {
                        writer.Write(data);
                    }
                }
                string base64Credentials = this.GetEncodedCredentials();
                newRequest.Headers.Add("Authorization", "Basic " + base64Credentials);
                HttpWebResponse response = newRequest.GetResponse() as HttpWebResponse;
                string result = string.Empty;
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                }
                newRequest = null;
                response = null;
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string GetEncodedCredentials()
        {
            string mergedCredentials = string.Format("{0}:{1}", "victor.sanchez@incrementalgroup.co.uk", "new18VS");
            byte[] byteCredentials = UTF8Encoding.UTF8.GetBytes(mergedCredentials);
            return Convert.ToBase64String(byteCredentials);
        }

        public JiraGetCommentModel GetLastTicketComment(string ticketID)
        {
            string jsonData = this.RunQuery("https://incrementalgroup.atlassian.net/rest/api/latest/issue/" + ticketID);
            if (string.IsNullOrEmpty(jsonData))
            {
                return new JiraGetCommentModel("Ticket does not exist or there was an error in finding it.", false);
            }
            else
            {
                var details = JObject.Parse(jsonData);
                string lastComment = details["fields"]["comment"]["comments"].Last.ToString();

                return new JiraGetCommentModel(lastComment);
            }
        }

        public JiraGetResolutionModel GetTicketResolution(string ticketID)
        {
            string jsonData = this.RunQuery("https://incrementalgroup.atlassian.net/rest/api/latest/issue/" + ticketID);
            if (string.IsNullOrEmpty(jsonData))
            {
                return new JiraGetResolutionModel("Ticket does not exist or there was an error in finding it.", false);
            }
            else
            {
                var details = JObject.Parse(jsonData);
                string resolution = details["fields"].ToString();

                return new JiraGetResolutionModel(resolution);
            }
        }

        public JiraGetStatusModel GetTicketStatus(string ticketID)
        {
            string jsonData = this.RunQuery("https://incrementalgroup.atlassian.net/rest/api/latest/issue/" + ticketID);
            if (string.IsNullOrEmpty(jsonData))
            {
                return new JiraGetStatusModel("Ticket does not exist or there was an error in finding it.", false);
            }
            else
            {
                var details = JObject.Parse(jsonData);
                string status = details["fields"].ToString();


                return new JiraGetStatusModel(status);
            }
        }

        public string GetProjectTicketCount(string project, string fromDate = null)
        {
            string jsonData = this.RunQuery("https://incrementalgroup.atlassian.net/rest/api/2/search?jql=project=" + project + "&maxResults=9000");
            if (string.IsNullOrEmpty(jsonData))
            {
                return "Project does not exist or there was an error in finding it.";
            }
            else
            {
                var details = JObject.Parse(jsonData);

                if (fromDate == null)
                    return details["total"].ToString();
                else
                {
                    List<JiraIncidentModel> incidents = JsonConvert.DeserializeObject<List<JiraIncidentModel>>(details["issues"].ToString());

                    if (fromDate.ToUpper().Equals("TODAY"))
                    {
                        return incidents.FindAll(x => x.fields.created > DateTime.Today).Count.ToString();
                    }
                    else if (fromDate.ToUpper().Equals("YESTERDAY"))
                    {
                        return incidents.FindAll(x => x.fields.created > DateTime.Today.AddDays(-1) && x.fields.created < DateTime.Today).Count.ToString();
                    }
                    else if (fromDate.ToUpper().Equals("THIS WEEK"))
                    {
                        return incidents.FindAll(x => x.fields.created > DateTime.Today.AddDays(-7) && x.fields.created <= DateTime.Today).Count.ToString();
                    }
                    else if (fromDate.ToUpper().Equals("THIS MONTH"))
                    {
                        return incidents.FindAll(x => x.fields.created > Convert.ToDateTime(DateTime.Today.Year + "-" + DateTime.Today.Month + "-01") && x.fields.created <= DateTime.Today).Count.ToString();
                    }
                    else if (fromDate.ToUpper().Equals("THIS YEAR"))
                    {
                        return incidents.FindAll(x => x.fields.created > Convert.ToDateTime(DateTime.Today.AddYears(-1).Year + "-" + DateTime.Today.Month + "-" + DateTime.Today.Day) && x.fields.created <= DateTime.Today).Count.ToString();
                    }
                }

                return "Date criteria [" + fromDate + "] not matched in system.";
            }
        }

        public bool AddWorkLog(AddIncidentWorklogQuery query, string ticketId)
        {
            JiraWorkLogModel model = new JiraWorkLogModel();
            model.timeSpent = query.Time.ToString();
            model.comment = query.Comment;
            model.started = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.000+0000");

            string worklogText = JsonConvert.SerializeObject(model);
            string jsonData = this.RunQuery("https://incrementalgroup.atlassian.net/rest/api/latest/issue/" + ticketId + "/worklog", worklogText, "POST");

            return jsonData.ToLower().Contains("statuscode") ? false : true;
        }

        public bool AddComment(AddIncidentCommentQuery query, string ticketId)
        {
            JiraPostCommentModel model = new JiraPostCommentModel();
            model.body = query.Comment;

            string worklogText = JsonConvert.SerializeObject(model);
            string jsonData = this.RunQuery("https://incrementalgroup.atlassian.net/rest/api/latest/issue/" + ticketId + "/comment", worklogText, "POST");

            return jsonData.ToLower().Contains("statuscode") ? false : true;
        }

        public string CreateTicket(string project, string fromDate = null)
        {
            string ticket = string.Empty;
            string data = @"{ 'fields': { 'project' : { 'key' : 'JTL' }, 'issuetype' : { 'name' : 'BUG' }";
            string jsonData = this.RunQuery("https://incrementalgroup.atlassian.net/rest/api/latest/issue", data);
            try
            {
                ticket = "ticket.self = url";
            }
            catch (Exception ex)
            {
                throw new Exception("There was something wrong when you were trying to create a ticket. Please, contact your administrator. " + ex.Message);
            }
            return ticket;
        }
    }
}