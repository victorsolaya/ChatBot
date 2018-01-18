namespace LuisBot.Dialogs
{
    using LuisBot.Helpers;
    using LuisBot.Models.SQL;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Connector;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [LuisModel("3307a929-1216-461d-b3d2-66a6cac7d389", "38658286acef4dc7a7528a6b5f17be57")]
    [Serializable]
    public class StartDialog : LuisDialog<object>
    {
        List<string> choicesselected = new List<string>();

        public override async Task StartAsync(IDialogContext context)
        {
            /// Bot start from here
           // var dialog = new SQLConnection();
           // dialog.GetConnection();
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IActivity> argument)
        {
            /**
             * This have been done because if you put the code which is between case "ActivityTypes.Message:", you are getting duplicate promptdialog.choice. 
             * Just for avoiding that, the best option is doing a switch. So we can get more advantage of the message of the activty types.
             */
            var activity = await argument;
            switch (activity.Type)
            {
                case ActivityTypes.Message:
                    IEnumerable<LuisBot.Enums.ChoicesTypes> choices = (IEnumerable<LuisBot.Enums.ChoicesTypes>)Enum.GetValues(typeof(LuisBot.Enums.ChoicesTypes));
                    EnumHelper enumhelper = new EnumHelper();
                    IEnumerable<string> choicesDisplay = enumhelper.GetEnumChoiceAttributeString();
                    PromptDialog.Choice(context,this.SelectChoice, choicesDisplay, "Please, choose any option.");
                    break;
                default:
                    context.Wait(MessageReceivedAsync);
                    break;
            }
        }

        private async Task SelectChoice(IDialogContext context, IAwaitable<string> choice)
        {
           // Changed from switch for using the enum variables we have.
           // As we don't have many enum variables he efficience is the same.
            if(await choice == LuisBot.Enums.ChoicesTypes.StartAccomodation.GetStringValue())
            {
                var bookAccommodation = new LuisBot.Dialogs.BookAccommodationDialog();
                await bookAccommodation.StartAccommodationAsync(context);
            } else if (await choice == LuisBot.Enums.ChoicesTypes.SearchHotels.GetStringValue())
            {
                var searchHotels = new LuisBot.Dialogs.SearchHotelsDialog();
                await searchHotels.StartSearchHotelsAsync(context);
            }
            else if (await choice == LuisBot.Enums.ChoicesTypes.HotelReviews.GetStringValue())
            {
                var hotelReviews = new LuisBot.Dialogs.HotelReviewsDialog();
                await hotelReviews.StartHotelReviewsAsync(context);
            }
            else if (await choice == LuisBot.Enums.ChoicesTypes.Support.GetStringValue())
            {
                var support = new LuisBot.Dialogs.SupportDialog();
                await support.StartSupportAsync(context);
            }
            else if(await choice == LuisBot.Enums.ChoicesTypes.Timesheets.GetStringValue())
            {
                var timesheet = new LuisBot.Dialogs.TimeSheetsDialog();
                await timesheet.StartTimesheetAsync(context);
            }
        }
    }
}
