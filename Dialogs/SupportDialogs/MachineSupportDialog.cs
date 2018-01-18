namespace LuisBot.Dialogs
{
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.FormFlow;
    using Microsoft.Bot.Builder.Luis;
    using System;
    using System.Threading.Tasks;

    [LuisModel("3307a929-1216-461d-b3d2-66a6cac7d389", "38658286acef4dc7a7528a6b5f17be57")]
    [Serializable]
    public class MachineSupportDialog : LuisDialog<object>
    {
        public override async Task StartAsync(IDialogContext context)
        {
            var supportFormDialog = FormDialog.FromForm(this.BuildSupportQuery, FormOptions.PromptInStart);

            context.Call(supportFormDialog, this.ResumeAfterSupportFormDialog);
        }

        private IForm<MachineSupportQuery> BuildSupportQuery()
        {
            OnCompletionAsyncDelegate<MachineSupportQuery> processSupportQuery = async (context, state) =>
            {
                await context.PostAsync($"Ok...processing details.");
            };

            return new FormBuilder<MachineSupportQuery>()
                .Field(nameof(MachineSupportQuery.IsSwitchingOn))
                .AddRemainingFields()
                .OnCompletion(processSupportQuery)
                .Build();
        }

        private async Task ResumeAfterSupportFormDialog(IDialogContext context, IAwaitable<MachineSupportQuery> result)
        {
            try
            {
                MachineSupportQuery query = await result;

                // raise ticket code here
                //bla bla

                var random = new Random();
                int ticketNumber = random.Next(100);
                await context.PostAsync("Your ticket number is '" + ticketNumber.ToString() + "'. A member from support team will contact you shortly.");
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
