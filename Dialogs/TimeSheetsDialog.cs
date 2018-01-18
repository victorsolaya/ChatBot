namespace LuisBot.Dialogs
{
    using LuisBot.Models.SQL;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Luis;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [LuisModel("3307a929-1216-461d-b3d2-66a6cac7d389", "38658286acef4dc7a7528a6b5f17be57")]
    [Serializable]
    public class TimeSheetsDialog : LuisDialog<object>
    {
        public string allocatedTime;

        public TimeSheetsDialog() { }
        public TimeSheetsDialog(string allocatedTime)
        {
            this.allocatedTime = allocatedTime;
        }

        private static SQLConnection sql = new SQLConnection();

        //private IList<string> numberOfHoursOptions = new List<string> { "1", "2", "3", "4", "5", "6", "7" };
        private IList<string> isChargableOptions = new List<string> { "Yes", "No" };
        private IList<string> entryTypeOptions = new List<string> { "Normal", "Overtime (normal)", "Overtime (double)", "Overtime (unpaid)", "Time in Lieu" };
        private IList<string> dateOptions = new List<string> { "Yesterday", "Today" };
        private TimeSheetEntry timeSheetEntry;

        private List<string> peopleOptions = sql.GetUsers();
        private int qIndex;

        private void ShowPeopleOptions(IDialogContext context)
        {
            PromptDialog.Choice(context, this.onPeopleSelected, peopleOptions, "Who are you?");
        }

        private void ShowChargableOptions(IDialogContext context)
        {
            ///This is the first that is initialized. -> So we should put the fullname first for retrieving the user.
            PromptDialog.Choice(context, this.OnChargableOptionSelected, isChargableOptions, "Is it a chargable entry you want to add?", "Not a valid option", 3);
        }

        private void ShowEntryTypeOptions(IDialogContext context)
        {
            PromptDialog.Choice(context, this.OnEntryTypeOptionSelected, entryTypeOptions, "What type of entry is this?", "Not a valid option", 3);
        }

        private void ShowNumberOfHoursOptions(IDialogContext context)
        {
            PromptDialog.Text(context, this.OnNumberOfHoursOptionSelected, "What is the amount of hours you want to allocate?", "Not a valid option", 3);
        }

        private void ShowDatesOptions(IDialogContext context)
        {
            PromptDialog.Choice(context, this.OnDateOptionSelected, dateOptions, "What is the entry date?", "Not a valid option", 3);
        }

        private void AskRemainingQuestions(IDialogContext context)
        {
            switch (qIndex)
            {
                case 0:
                    PromptDialog.Text(context, this.OnRemainingQuestionsAnswered, "What is the job code you want to assign it to?", "Need to enter a value", 3);
                    break;
                case 1:
                    PromptDialog.Text(context, this.OnRemainingQuestionsAnswered, "What is the entry description?", "Need to enter a value", 3);
                    break;
                default:
                    break;
            }
        }

        public async Task StartTimesheetAsync(IDialogContext context)
        {
            this.ShowPeopleOptions(context);
        }

        private async Task OnChargableOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string optionSelected = await result;

                timeSheetEntry = new TimeSheetEntry();
                timeSheetEntry.IsChargable = optionSelected;

                this.ShowEntryTypeOptions(context);
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Ooops! Too many attemps :(. But don't worry, I'm handling that exception and you can try again!");

                context.Done<object>(null);
            }
        }

        private async Task OnEntryTypeOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string optionSelected = await result;
                timeSheetEntry.EntryType = optionSelected;

                if (this.allocatedTime == null)
                {
                    this.ShowNumberOfHoursOptions(context);
                }
                else
                {
                    if (allocatedTime.ToUpper().Contains("HOURS"))
                        timeSheetEntry.NumberOfHours = Convert.ToDecimal(allocatedTime.ToUpper().Replace("HOURS", "").Replace(" ", ""));
                    else
                        timeSheetEntry.NumberOfHours = Convert.ToDecimal(allocatedTime.ToUpper().Replace("MINUTES", "").Replace(" ", "")) / 60;

                    this.ShowDatesOptions(context);
                }
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Ooops! Too many attemps :(. But don't worry, I'm handling that exception and you can try again!");

                context.Done<object>(null);
            }
        }

        private async Task OnNumberOfHoursOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string optionSelected = await result;
                timeSheetEntry.NumberOfHours = Convert.ToDecimal(optionSelected);

                this.ShowDatesOptions(context);
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Ooops! Too many attemps :(. But don't worry, I'm handling that exception and you can try again!");

                context.Done<object>(null);
            }
        }

        private async Task OnDateOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string optionSelected = await result;
                if (optionSelected.ToLower().Equals("yesterday"))
                {
                    timeSheetEntry.EntryDate = DateTime.Today.AddDays(-1);
                }
                else if (optionSelected.ToLower().Equals("today"))
                {
                    timeSheetEntry.EntryDate = DateTime.Today;
                }
                else
                {
                    timeSheetEntry.EntryDate = Convert.ToDateTime(optionSelected);
                }

                qIndex = 0;
                AskRemainingQuestions(context);
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Ooops! Too many attemps :(. But don't worry, I'm handling that exception and you can try again!. {ex.Message}");

                context.Done<object>(null);
            }
        }

        private async Task OnRemainingQuestionsAnswered(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string answer = await result;

                switch (qIndex)
                {
                    case 0:
                        timeSheetEntry.JobCode = answer;
                        break;

                    case 1:
                        timeSheetEntry.Comments = answer;
                        break;

                    default:
                        break;
                }

                if (qIndex == 1)
                {
                    //add code to add timesheet entry here
                    //code code code
                    string chargable = timeSheetEntry.IsChargable.ToUpper().Equals("YES") ? "Chargable" : "Non-Chargable";
                    await context.PostAsync($"Ok. " + chargable + " timesheet entry of " + timeSheetEntry.NumberOfHours + " hours entered under JobCode " + timeSheetEntry.JobCode);

                    context.Done<object>(null);
                }
                else
                {
                    qIndex++;
                    this.AskRemainingQuestions(context);
                }
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Ooops! Too many attemps :(. But don't worry, I'm handling that exception and you can try again!");

                context.Done<object>(null);
            }
        }

        private async Task onPeopleSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string name = await result;
                timeSheetEntry.FullName = name;
                await context.PostAsync(name);
                this.ShowChargableOptions(context);
            }
            catch (Exception ex)
            {
                throw new Exception($"Please contact with your administrator. { ex.Message }");
            }
        }

    }
}
