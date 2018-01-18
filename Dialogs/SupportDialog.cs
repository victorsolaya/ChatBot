namespace LuisBot.Dialogs
{
    using LuisBot.Helpers;
    using LuisBot.Models;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.FormFlow;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Connector;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [LuisModel("3307a929-1216-461d-b3d2-66a6cac7d389", "38658286acef4dc7a7528a6b5f17be57")]
    [Serializable]
    public class SupportDialog : LuisDialog<object>
    {
        string ticketnumber = string.Empty;
        public async Task StartSupportAsync(IDialogContext context)
        {
            //var supportFormDialog = FormDialog.FromForm(this.BuildSupportQuery, FormOptions.PromptInStart);
            //context.Call(supportFormDialog, this.ResumeAfterSupportFormDialog);
            IEnumerable<LuisBot.Enums.Support.SupportOptions> supportOptions = (IEnumerable<LuisBot.Enums.Support.SupportOptions>)Enum.GetValues(typeof(LuisBot.Enums.Support.SupportOptions));
            EnumHelper enumhelper = new EnumHelper();
            IEnumerable<string> supportDisplay = enumhelper.GetEnumSupportAttributeString();
            PromptDialog.Choice(context, this.ResumeAfterChooseOption, supportDisplay, "Please, choose an option");
        }

        /*private IForm<SupportQuery> BuildSupportQuery()
        {
            string ticketnumber = string.Empty;
            OnCompletionAsyncDelegate<SupportQuery> processSupportQuery = async (context, state) =>
            {
                await context.PostAsync($"Ok...processing details.");
            };

            return new FormBuilder<SupportQuery>()
                .Field(nameof(SupportQuery.TicketNumber))
                   
                //.AddRemainingFields()
                .Message("You have written ticket number {TicketNumber}")
                .OnCompletion(processSupportQuery)
                .Build();
        }*/

        public async Task ResumeAfterSupportFormDialog(IDialogContext context, IAwaitable<string> ticketId = null)
        {
            try
            {
                if (ticketnumber == string.Empty)
                {
                    ticketnumber = await ticketId;
                }
                EnumHelper enumhelper = new EnumHelper();
                IEnumerable<string> getTicketsDisplay = enumhelper.GetEnumGetTicketAttributeString();
                PromptDialog.Choice(context, this.ResumeAfterChooseGetTicket, getTicketsDisplay, "Please, choose what you want to get");
                //SupportQuery query = await result;
                /*JiraController jira = new JiraController();
                JiraGetCommentModel ticketnumber = jira.GetLastTicketComment(ticketId);
                await context.PostAsync(ticketnumber.body);*/
                // raise ticket code here
                //bla bla

                /*var random = new Random();
                int ticketNumber = random.Next(100);
                await context.PostAsync("Your ticket number is '" + ticketNumber.ToString() + "'. A member from support team will contact you shortly.");*/
            }
            catch (FormCanceledException ex)
            {
                string reply = String.Empty;

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
            /*finally
            {
                context.Done<object>(null);
            }*/
        }

        private async Task ResumeAfterChooseOption(IDialogContext context, IAwaitable<string> choice)
        {
            if (await choice == LuisBot.Enums.Support.SupportOptions.GetTicket.GetStringValue())
            {
                GetTickets(context);
            }
            else if (await choice == LuisBot.Enums.Support.SupportOptions.CreateTicket.GetStringValue())
            {
                DoSomeStuff(context, "Great, you did a connection for creating tickets");
            }
        }

        private async Task ResumeAfterChooseGetTicket(IDialogContext context, IAwaitable<string> choice)
        {
            try
            {
                JiraController jira = new JiraController();

                if (await choice == LuisBot.Enums.Support.GetTicketOptions.GetLastComment.GetStringValue())
                {
                    JiraGetCommentModel ticket = jira.GetLastTicketComment(ticketnumber);
                    await context.PostAsync(ticket.body);
                }
                else if (await choice == LuisBot.Enums.Support.GetTicketOptions.GetUrl.GetStringValue())
                {
                    JiraGetCommentModel ticket = jira.GetLastTicketComment(ticketnumber);
                    await context.PostAsync(ticket.self);
                }
                else if (await choice == LuisBot.Enums.Support.GetTicketOptions.GetResolution.GetStringValue())
                {
                    JiraGetResolutionModel ticket = jira.GetTicketResolution(ticketnumber);
                    await context.PostAsync($"The resolution of your ticket is: {ticket.resolution}.");
                }
                else if (await choice == LuisBot.Enums.Support.GetTicketOptions.GetStatus.GetStringValue())
                {
                    JiraGetStatusModel ticket = jira.GetTicketStatus(ticketnumber);
                    await context.PostAsync($"Your status ticket is: {ticket.status}.");
                }
            }
            catch (Exception ex)
            {
                await context.PostAsync($"There was an error. Please contact your administrator. - {ex.Message}");
            }
            finally
            {
                PromptDialog.Confirm(context, this.CallChooseTicketAgain, "Do you want to know anything else about this ticket?");
                //context.Done<object>(null);
            }
        }

        private async Task CallChooseTicketAgain(IDialogContext context, IAwaitable<bool> confirm)
        {
            if (await confirm)
            {
                await ResumeAfterSupportFormDialog(context);
            }
            else
            {
                PromptDialog.Confirm(context, this.CallChooseAnotherTicket, "Do you want to know anything else about other ticket?");
            }
        }

        private async Task CallChooseAnotherTicket(IDialogContext context, IAwaitable<bool> confirm)
        {
            if (await confirm)
            {
                await GetTickets(context);
            }
            else
            {
                PromptDialog.Confirm(context, this.ChooseAllEligibleOptions, "May I help you with anything else?");
                //context.Done<object>(null);
            }
        }

        private async Task ChooseAllEligibleOptions(IDialogContext context, IAwaitable<bool> confirm)
        {
            if (await confirm)
            {
                var starting = new StartDialog();
                await context.PostAsync("Please, type 'OK' and press enter to continue...");
                await starting.StartAsync(context);
            }
            else
            {
                await context.PostAsync("Looking forward to hearing from you again");
                context.Done<object>(null);
            }
        }

        private async Task GetTickets(IDialogContext context)
        {
            PromptDialog.Text(context, this.ResumeAfterSupportFormDialog, "Could you write your ticket number, please?");
        }

        private async void DoSomeStuff(IDialogContext context, string message)
        {
            await context.PostAsync(message);
        }
    }
}
