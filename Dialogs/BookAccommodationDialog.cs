namespace LuisBot.Dialogs
{
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.FormFlow;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Connector;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    [LuisModel("3307a929-1216-461d-b3d2-66a6cac7d389", "38658286acef4dc7a7528a6b5f17be57")]
    [Serializable]
    public class BookAccommodationDialog : LuisDialog<object>
    {
        private string departureDate;
        private string departureLocation;
        private string returnDate;
        private string destinationLocation;

        private IList<string> hotelOptions = new List<string> { "Ace Hotel Chicago", "The Langham Chicago", "Four Seasons Hotel Chicago", "Sofitel Chicago Magnificent Mile", "London House Chicago", "The Drake Hotel" };
        private IList<string> hotelImagesOptions = new List<string>
        {
            "https://exp.cdn-hotels.com/hotels/1000000/20000/15100/15087/15087_14_b.jpg",
            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQ8n7zXTJCTr96AP9zumqCfZes3EZ2OpKd9o6BQDbe9jSuHoOv9",
            "http://media.cntraveler.com/photos/53da8b8f6dec627b149f1b76/master/pass/trump-international-hotel-tower-chicago-chicago-illinois-103990-1.jpg",
            "https://t-ec.bstatic.com/images/hotel/270x200/132/13262985.jpg",
            "https://assets.hipmunk.com/assets/hotel_image/v1/509bfdc066edc84661018de4/0.jpg",
            "http://www.chicagotraveler.com/sites/default/files/conrad-2_0.jpg"
        };
        private IList<string> hotelDescOptions = new List<string>
        {
            "Four Star Hotel - £89 per night",
            "Five Star Hotel - £110 per night",
            "Four Star Hotel - £100 per night",
            "Five Star Hotel - £122 per night",
            "Four Star Hotel - £135 per night",
            "Four Star Hotel - £90 per night"
        };

        private IList<string> hotelNumberOptions = new List<string>
        {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6"
        };

        public BookAccommodationDialog() { }
        public BookAccommodationDialog(string departureDate, string departureLocation, string returnDate, string destinationLocation)
        {
            this.departureDate = departureDate;
            this.departureLocation = departureLocation;
            this.returnDate = returnDate;
            this.destinationLocation = destinationLocation;
        }
        public async Task StartAccommodationAsync(IDialogContext context)
        {
            var bookAccommodationFormDialog = FormDialog.FromForm(this.BuildBookingQuery, FormOptions.PromptInStart);

            context.Call(bookAccommodationFormDialog, this.ResumeAfterBookingFormDialog);
        }

        private IForm<BookAccommodationQuery> BuildBookingQuery()
        {
            OnCompletionAsyncDelegate<BookAccommodationQuery> processBookingQuery = async (context, state) =>
            {
                await context.PostAsync($"Ok...processing details.");
            };

            return new FormBuilder<BookAccommodationQuery>()
                .Field(nameof(BookAccommodationQuery.ProjectCode))
                .OnCompletion(processBookingQuery)
                .Build();
        }

        private void ShowHotelOption(IDialogContext context)
        {
            PromptDialog.Choice(context, this.OnHotelSelected2, hotelOptions, "Which one would you like to select?"/*, "Not a valid option", 3*/);
        }

        private async Task ResumeAfterBookingFormDialog(IDialogContext context, IAwaitable<BookAccommodationQuery> result)
        {
            try
            {
                BookAccommodationQuery query = await result;

                await context.PostAsync($"Ok, Presales means that flights over £500 need to be sent for approval. Your selection will be sent to Stuart Kerr.");

                var resultMessage = context.MakeMessage();
                resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                resultMessage.Attachments = new List<Attachment>();

                for (int i = 0; i < 5; i++)
                {
                    var random = new Random(i);
                    string title = this.hotelOptions[random.Next(0, this.hotelOptions.Count - 1)];
                    ThumbnailCard thumbnailCard = new ThumbnailCard()
                    {
                        Title = title,
                        Text = this.hotelDescOptions[random.Next(0, this.hotelDescOptions.Count - 1)],
                        Images = new List<CardImage>()
                        {
                            new CardImage() { Url = this.hotelImagesOptions[random.Next(0, this.hotelImagesOptions.Count - 1)] }
                        }
                    };

                    resultMessage.Attachments.Add(thumbnailCard.ToAttachment());
                }
                //await context.PostAsync(resultMessage);
                this.ShowHotelOption(context);

                //context.Wait(this.ResumeAfterBookingFormDialog);
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

        private async Task OnHotelSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string s = await result;
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Ooops! Too many attemps :(. But don't worry, I'm handling that exception and you can try again!");

                context.Done<object>(null);
            }
        }

        private async Task OnHotelSelected2(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string s = await result;
                await context.PostAsync($"Ok, you have selected {s}, nice option");
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Ooops! Too many attemps :(. But don't worry, I'm handling that exception and you can try again!");

                context.Done<object>(null);
            }
        }
    }
}
