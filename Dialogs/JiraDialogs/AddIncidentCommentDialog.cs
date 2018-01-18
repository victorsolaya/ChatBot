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
    public class AddIncidentCommentDialog : LuisDialog<object>
    {
        EntityRecommendation ticketNumber;

        public AddIncidentCommentDialog() { }
        public AddIncidentCommentDialog(EntityRecommendation ticketNumber)
        {
            this.ticketNumber = ticketNumber;
        }

        public override async Task StartAsync(IDialogContext context)
        {
            var addIncidentCommentDialog = FormDialog.FromForm(this.BuildIncidentCommentQuery, FormOptions.PromptInStart);

            context.Call(addIncidentCommentDialog, this.ResumeAfterIncidentCommentFormDialog);
        }

        private IForm<AddIncidentCommentQuery> BuildIncidentCommentQuery()
        {
            OnCompletionAsyncDelegate<AddIncidentCommentQuery> processBookingQuery = async (context, state) =>
            {
                await context.PostAsync($"Ok...processing details.");
            };

            if (ticketNumber != null)
            {
                return new FormBuilder<AddIncidentCommentQuery>()
                    .Field(nameof(AddIncidentCommentQuery.Comment))
                    .OnCompletion(processBookingQuery)
                    .Build();
            }
            else
            {
                return new FormBuilder<AddIncidentCommentQuery>()
                    .Field(nameof(AddIncidentCommentQuery.TicketId))
                    .AddRemainingFields()
                    .OnCompletion(processBookingQuery)
                    .Build();
            }
        }

        private async Task ResumeAfterIncidentCommentFormDialog(IDialogContext context, IAwaitable<AddIncidentCommentQuery> result)
        {
            try
            {
                AddIncidentCommentQuery query = await result;
                if (query.TicketId == null)
                    query.TicketId = ticketNumber.Entity;

                JiraController jc = new JiraController();
                bool isAdded = jc.AddComment(query, query.TicketId.Replace(" ", ""));

                string message = isAdded ? "Comment added successfully to incident " + query.TicketId.Replace(" ","") + "." : "There was an error while adding your worklog item. Please contact your administrator";

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
