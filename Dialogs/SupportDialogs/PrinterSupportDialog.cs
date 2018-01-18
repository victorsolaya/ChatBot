namespace LuisBot.Dialogs
{
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Luis;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;

    [LuisModel("3307a929-1216-461d-b3d2-66a6cac7d389", "38658286acef4dc7a7528a6b5f17be57")]
    [Serializable]
    public class PrinterSupportDialog : LuisDialog<object>
    {
        private IList<string> yesNoOptions = new List<string> { "Yes", "No" };

        private PrinterSupportQuery printerSupportQuery;
     
        private void ShowIsPrinterAvailableOptions(IDialogContext context)
        {
            PromptDialog.Choice(context, this.OnPrinterAvailableOptionSelected, yesNoOptions, "Do you see your printer there?", "Not a valid option", 3);
        }

        private void ShowIsFixedAfterTroubleshootOptions(IDialogContext context)
        {
            PromptDialog.Choice(context, this.OnTroubleshootOptionSelected, yesNoOptions, "Did this fix your issue?", "Not a valid option", 3);
        }

        private void EnterDescriptionOption(IDialogContext context)
        {
            PromptDialog.Text(context, this.OnDescriptionEntered, "I know of 2 printers that are available there - Both appear to be online can you give me some more information about your issue.", "Not a valid option", 3);
        }


        public override async Task StartAsync(IDialogContext context)
        {
            this.EnterDescriptionOption(context);
        }

        private async Task OnDescriptionEntered(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string optionSelected = await result;

                printerSupportQuery = new PrinterSupportQuery();
                printerSupportQuery.Description = optionSelected;

                await context.PostAsync("66% of the people who reported this issue, solved it by following these steps. Please go to 'Printers and Scanners' in Control Panel.");

                //string controlpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System),"control.exe");
                //Process.Start(controlpath, "/name Microsoft.DevicesAndPrinters");

                this.ShowIsPrinterAvailableOptions(context);
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Ooops! Too many attemps :(. But don't worry, I'm handling that exception and you can try again!");

                context.Done<object>(null);
            }
        }

        private async Task OnPrinterAvailableOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string optionSelected = await result;

                printerSupportQuery.IsPrinterAvailable = optionSelected.ToLower().Equals("yes") ? true:false;

                if (printerSupportQuery.IsPrinterAvailable)
                {
                    await context.PostAsync("Right click on your printer and click on 'Troubleshoot.");
                    this.ShowIsFixedAfterTroubleshootOptions(context);
                }
                else
                {
                    await context.PostAsync("Please re-install the printer and try again. Goodbye");
                    context.Done<object>(null);
                }
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Ooops! Too many attemps :(. But don't worry, I'm handling that exception and you can try again!");

                context.Done<object>(null);
            }
        }

        private async Task OnTroubleshootOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string optionSelected = await result;
                printerSupportQuery.FixedAfterTroubleshoot = optionSelected.ToLower().Equals("yes") ? true : false;

                if (!printerSupportQuery.FixedAfterTroubleshoot)
                {
                    await context.PostAsync($"OK, I will raise a support call and get the service desk to contact you.");
                    context.Done<object>(null);
                }
                else
                {
                    await context.PostAsync($"I'm glad this fixed your issue. Goodbye :)");
                    context.Done<object>(null);
                }
                
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Ooops! Too many attemps :(. But don't worry, I'm handling that exception and you can try again!");

                context.Done<object>(null);
            }
        }
    }
}
