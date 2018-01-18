namespace LuisBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using LuisBot.Enums;
    using LuisBot.Enums.Support;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.FormFlow;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Builder.Luis.Models;
    using Microsoft.Bot.Connector;

    [LuisModel("4300110c-0b2a-49ee-a65a-aaa757a2ded9", "b64738093c3040cd9ea98df99feda8c9")]
    [Serializable]
    public class RootLuisDialog : LuisDialog<object>
    {
        private const string EntityGeographyCity = "builtin.geography.city";

        [LuisIntent("Greeting")]
        public override async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("How may I help you?");
            context.Call(new StartDialog(), this.ResumeAfterDialog);
        }

        [LuisIntent("Support")]
        public async Task SupportInit(IDialogContext context, LuisResult result)
        {

            if (result.Entities.Count == 0)
            {
                await context.PostAsync("Sorry, I didn't get what your problem is about.");
                context.Call(new SupportDialog(), this.ResumeAfterDialog);
            }
            else
            {
                foreach (EntityRecommendation entity in result.Entities)
                {
                    SupportTypes typeOfSupport = getSupportType(entity);

                    if (typeOfSupport == SupportTypes.NotResolved)
                    {
                        await context.PostAsync("I understand you have a problem but unfortunately can't understand what you mean by '" + entity.Entity + "'.");
                        context.Call(new SupportDialog(), this.ResumeAfterDialog);
                    }
                    else
                    {
                        string greeting = DateTime.Now.TimeOfDay > new TimeSpan(12, 0, 0) ? "Good afternoon" : "Good morning";
                        await context.PostAsync(greeting + " Andy. I see you are in the Tontine building today. ");
                        if (typeOfSupport == SupportTypes.MachineRelated)
                        {
                            await context.PostAsync("I understand you're having issue with your computer. Please answer the following questions: ");
                            context.Call(new MachineSupportDialog(), this.ResumeAfterDialog);
                        }
                        else if (typeOfSupport == SupportTypes.Printer)
                        {
                            context.Call(new PrinterSupportDialog(), this.ResumeAfterDialog);
                        }
                    }
                }
            }
        }

        private SupportTypes getSupportType(EntityRecommendation entity)
        {
            if (entity.Entity.ToLower().Equals("machine") || entity.Entity.ToLower().Equals("laptop") || entity.Entity.ToLower().Equals("computer"))
                return LuisBot.Enums.SupportTypes.MachineRelated;
            else if (entity.Entity.ToLower().Equals("printer") || entity.Entity.ToLower().Contains("xerox"))
                return SupportTypes.Printer;
            else
                return SupportTypes.NotResolved;
        }

        private async Task ResumeAfterDialog(IDialogContext context, IAwaitable<object> result)
        {
            //var ticketNumber = await result;

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry, I did not understand '{result.Query}'. Type 'help' if you need assistance.";

            await context.PostAsync(message);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("SearchHotels")]
        public async Task Search(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;
            await context.PostAsync($"Welcome to the Hotels finder! We are analyzing your message: '{message.Text}'...");

            var hotelsQuery = new HotelsQuery();

            EntityRecommendation cityEntityRecommendation;

            if (result.TryFindEntity(EntityGeographyCity, out cityEntityRecommendation))
            {
                cityEntityRecommendation.Type = "Destination";
            }

            var hotelsFormDialog = new FormDialog<HotelsQuery>(hotelsQuery, this.BuildHotelsForm, FormOptions.PromptInStart, result.Entities);

            context.Call(hotelsFormDialog, this.ResumeAfterHotelsFormDialog);
        }
        #region search hotels
        private async Task ResumeAfterHotelsFormDialog(IDialogContext context, IAwaitable<HotelsQuery> result)
        {
            try
            {
                var searchQuery = await result;

                var hotels = await this.GetHotelsAsync(searchQuery);

                await context.PostAsync($"I found {hotels.Count()} hotels:");

                var resultMessage = context.MakeMessage();
                resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                resultMessage.Attachments = new List<Attachment>();

                foreach (var hotel in hotels)
                {
                    HeroCard heroCard = new HeroCard()
                    {
                        Title = hotel.Name,
                        Subtitle = $"{hotel.Rating} starts. {hotel.NumberOfReviews} reviews. From ${hotel.PriceStarting} per night.",
                        Images = new List<CardImage>()
                        {
                            new CardImage() { Url = hotel.Image }
                        },
                        Buttons = new List<CardAction>()
                        {
                            new CardAction()
                            {
                                Title = "More details",
                                Type = ActionTypes.OpenUrl,
                                Value = $"https://www.bing.com/search?q=hotels+in+" + HttpUtility.UrlEncode(hotel.Location)
                            }
                        }
                    };

                    resultMessage.Attachments.Add(heroCard.ToAttachment());
                }

                await context.PostAsync(resultMessage);
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

        private async Task<IEnumerable<Hotel>> GetHotelsAsync(HotelsQuery searchQuery)
        {
            var hotels = new List<Hotel>();

            // Filling the hotels results manually just for demo purposes
            for (int i = 1; i <= 5; i++)
            {
                var random = new Random(i);
                Hotel hotel = new Hotel()
                {
                    Name = $"{searchQuery.Destination ?? searchQuery.AirportCode} Hotel {i}",
                    Location = searchQuery.Destination ?? searchQuery.AirportCode,
                    Rating = random.Next(1, 5),
                    NumberOfReviews = random.Next(0, 5000),
                    PriceStarting = random.Next(80, 450),
                    Image = $"https://placeholdit.imgix.net/~text?txtsize=35&txt=Hotel+{i}&w=500&h=260"
                };

                hotels.Add(hotel);
            }

            hotels.Sort((h1, h2) => h1.PriceStarting.CompareTo(h2.PriceStarting));

            return hotels;
        }

        private IForm<HotelsQuery> BuildHotelsForm()
        {
            OnCompletionAsyncDelegate<HotelsQuery> processHotelsSearch = async (context, state) =>
            {
                var message = "Searching for hotels";
                if (!string.IsNullOrEmpty(state.Destination))
                {
                    message += $" in {state.Destination}...";
                }
                else if (!string.IsNullOrEmpty(state.AirportCode))
                {
                    message += $" near {state.AirportCode.ToUpperInvariant()} airport...";
                }

                await context.PostAsync(message);
            };

            return new FormBuilder<HotelsQuery>()
                .Field(nameof(HotelsQuery.Destination), (state) => string.IsNullOrEmpty(state.AirportCode))
                .Field(nameof(HotelsQuery.AirportCode), (state) => string.IsNullOrEmpty(state.Destination))
                .OnCompletion(processHotelsSearch)
                .Build();
        }
        #endregion

        [LuisIntent("ShowHotelsReviews")]
        public async Task Reviews(IDialogContext context, LuisResult result)
        {
            context.Call(new HotelReviewsDialog(result), this.ResumeAfterDialog);
        }

        [LuisIntent("Help")]
        public async Task Help(IDialogContext context, LuisResult result)
        {
            string helpText = string.Format("Hi! Try asking me things like:" + Environment.NewLine + "- search hotels in Seattle," + Environment.NewLine + "- search hotels near LAX airport," + Environment.NewLine + "- show me the reviews of The Bot Resort, " + Environment.NewLine + "- Timesheets, " + Environment.NewLine + " - My printer is not working" + Environment.NewLine + "- book accommodation");

            await context.PostAsync(helpText);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("TimeSheets")]
        public async Task TimeSheets(IDialogContext context, LuisResult result)
        {
            EntityRecommendation allocatedTime;
            result.TryFindEntity("TimeSheetAllocatedTime", out allocatedTime);

            string timeEntered = allocatedTime != null ? allocatedTime.Entity : null;
            context.Call(new TimeSheetsDialog(timeEntered), this.ResumeAfterDialog);
        }

        [LuisIntent("BookAccommodation")]
        public async Task BookAccommodation(IDialogContext context, LuisResult result)
        {
            EntityRecommendation departureDate;
            result.TryFindEntity("BookingDepartureDate", out departureDate);

            EntityRecommendation departureLocation;
            result.TryFindEntity("BookingDepartureLocation", out departureLocation);

            EntityRecommendation destinationLocation;
            result.TryFindEntity("BookingDestinationLocation", out destinationLocation);

            EntityRecommendation returnDate;
            result.TryFindEntity("BookingReturnDate", out returnDate);

            context.Call(new BookAccommodationDialog(departureDate.Entity, departureLocation.Entity, returnDate.Entity, destinationLocation.Entity), this.ResumeAfterDialog);
        }

        [LuisIntent("Jira")]
        public async Task Jira(IDialogContext context, LuisResult result)
        {
            EntityRecommendation ticketNumber;

            EntityRecommendation actionRequested;
            result.TryFindEntity("JiraTicketActionRequested", out actionRequested);

            EntityRecommendation projectRequested;
            result.TryFindEntity("JiraProjectRequested", out projectRequested);

            EntityRecommendation actionDate;
            result.TryFindEntity("JiraActionDate", out actionDate);

            JiraController jc = new JiraController();
            if (actionRequested != null)
            {
                if ((actionRequested.Entity.ToUpper().Equals("UPDATE") || actionRequested.Entity.ToUpper().Equals("COMMENT")))
                {
                    if (result.TryFindEntity("JiraTicketNumber", out ticketNumber))
                    {
                        ticketNumber.Type = "TicketId";
                    }

                    if (ticketNumber != null)
                    {
                        //JiraGetCommentModel jcm = jc.GetLastTicketComment(ticketNumber.Entity.Replace(" ", ""));

                        //await context.PostAsync("Last comment was made by " + jcm.author.displayName + " on " + Convert.ToDateTime(jcm.created).ToString("dd/MM/yyyy HH:mm:ss") + ": " + jcm.body);
                    }
                    else
                    {
                        await context.PostAsync("Please provide a ticket number. Example: 'What is the last update on ticket TAR-282?'");
                    }
                }
                else if ((actionRequested.Entity.ToUpper().Equals("GET") || actionRequested.Entity.ToUpper().Equals("RETURN") || actionRequested.Entity.ToUpper().Equals("HOW MANY TICKETS") || actionRequested.Entity.ToUpper().Equals("HOW MANY INCIDENTS"))
                    && projectRequested != null && projectRequested.Entity != null)
                {
                    string dateFrom = actionDate != null ? actionDate.Entity : null;

                    string count = jc.GetProjectTicketCount(projectRequested.Entity, dateFrom);

                    await context.PostAsync(count);
                }
                else if (actionRequested.Entity.ToUpper().Equals("ADD WORKLOG"))
                {
                    if (result.TryFindEntity("JiraTicketNumber", out ticketNumber))
                    {
                        ticketNumber.Type = "TicketId";
                    }

                    context.Call(new AddIncidentWorklogDialog(ticketNumber), this.ResumeAfterDialog);
                }
                else if (actionRequested.Entity.ToUpper().Contains("ADD") && actionRequested.Entity.ToUpper().Contains("COMMENT"))
                {
                    if (result.TryFindEntity("JiraTicketNumber", out ticketNumber))
                    {
                        ticketNumber.Type = "TicketId";
                    }

                    context.Call(new AddIncidentCommentDialog(ticketNumber), this.ResumeAfterDialog);
                }

            }
        }
    }
}
