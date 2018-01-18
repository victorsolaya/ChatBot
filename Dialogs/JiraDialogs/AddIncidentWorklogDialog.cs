namespace LuisBot.Dialogs
{
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.FormFlow;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Builder.Luis.Models;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    [LuisModel("3307a929-1216-461d-b3d2-66a6cac7d389", "38658286acef4dc7a7528a6b5f17be57")]
    [Serializable]
    public class AddIncidentWorklogDialog : LuisDialog<object>
    {
        EntityRecommendation ticketNumber;

        public AddIncidentWorklogDialog() { }
        public AddIncidentWorklogDialog(EntityRecommendation ticketNumber)
        {
            this.ticketNumber = ticketNumber;
        }

        public override async Task StartAsync(IDialogContext context)
        {
            var addIncidentWorklogDialog = FormDialog.FromForm(this.BuildIncidentWorklogQuery, FormOptions.PromptInStart);

            context.Call(addIncidentWorklogDialog, this.ResumeAfterIncidentWorklogFormDialog);
        }

        private IForm<AddIncidentWorklogQuery> BuildIncidentWorklogQuery()
        {
            OnCompletionAsyncDelegate<AddIncidentWorklogQuery> processBookingQuery = async (context, state) =>
            {
                await context.PostAsync($"Ok...processing details.");
            };

            if (ticketNumber != null)
            {
                return new FormBuilder<AddIncidentWorklogQuery>()
                    .Field(nameof(AddIncidentWorklogQuery.Time))
                    .Field(nameof(AddIncidentWorklogQuery.Comment))
                    .OnCompletion(processBookingQuery)
                    .Build();
            }
            else
            {
                return new FormBuilder<AddIncidentWorklogQuery>()
                    .Field(nameof(AddIncidentWorklogQuery.TicketId))
                    .AddRemainingFields()
                    .OnCompletion(processBookingQuery)
                    .Build();
            }
        }

        private async Task ResumeAfterIncidentWorklogFormDialog(IDialogContext context, IAwaitable<AddIncidentWorklogQuery> result)
        {
            try
            {
                AddIncidentWorklogQuery query = await result;
                if (query.TicketId == null)
                    query.TicketId = ticketNumber.Entity;

                JiraController jc = new JiraController();
                bool isAdded = jc.AddWorkLog(query, query.TicketId.Replace(" ", ""));

                string message = isAdded ? "Worklog item of " + query.Time + "minutes added successfully to incident " + query.TicketId.Replace(" ","") + "." : "There was an error while adding your worklog item. Please contact your administrator";

                await context.PostAsync(message);
            }
            catch (FormCanceledException ex)
            {
                string reply;

                if (ex.InnerException == null)
                {
                    reply = "You have canceled the operation.";
                }
                else
                {
                    reply = $"Oops! Something went wrong :( Technical Details: {ex.InnerException.Message}";
                }

                await context.PostAsync(reply);
            }
            finally
            {
                context.Done<object>(null);
            }
        }
    }
}
