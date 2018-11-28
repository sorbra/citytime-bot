using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Google.Protobuf;
using Google.Cloud.Dialogflow.V2;
using Google.Protobuf.WellKnownTypes;

namespace mybot.Dialogflow
{
    public interface IDialogflowFulfillment
    {
        Task HandleFulfillmentRequest(HttpContext context);
    }

    public class DialogflowFulfillment : IDialogflowFulfillment
    {
        private ILogger<DialogflowFulfillment> _logger;
        private CityTimeZones _cityTimeZones = new CityTimeZones();

        public DialogflowFulfillment(ILogger<DialogflowFulfillment> logger)
        {
            _logger = logger;
        }

        /*
         * 1. Parses POST request body into Dialogflow WebHookRequest
         * 2. Logs WebHookRequest
         * 3. Calls DispatchRequest to create Dialogflow WebHookResponse
         * 4. Writes back the WebHookResponse
         * 5. Logs WebHookResponse
         */
        public async Task HandleFulfillmentRequest(HttpContext context)
        {
            var jsonParser = new JsonParser(JsonParser.Settings.Default.WithIgnoreUnknownFields(true));

            WebhookRequest request;
            using (var reader = new StreamReader(context.Request.Body))
            {
                request = jsonParser.Parse<WebhookRequest>(reader);
            };

            _logger.LogInformation(
                PrettyJson(request.ToString())
            );

            // call the relevant request handler
            WebhookResponse response = DispatchRequest(context, request);

            context.Response.ContentType = "application/json; charset=utf-8";
            await context.Response.WriteAsync(response.ToString(), Encoding.UTF8);

            _logger.LogInformation(
                PrettyJson(response.ToString())
            );
        }

        /*
         * Dispathes WebhookRequest to the relevant handler.
         */
        private WebhookResponse DispatchRequest(HttpContext context, WebhookRequest request)
        {
            switch (request.QueryResult.Action)
            {
                case "time.request":
                    return HandleTimeRequest(context, request);
                default:
                    throw new NotSupportedException($"Unknown Intent {request.QueryResult.Action}.");
            }
        }

        /*
         * Handle WebhookRequest where request.QueryResult.Action equals "time.request"
         */
        private WebhookResponse HandleTimeRequest(HttpContext context, WebhookRequest request)
        {
            try
            {
                var response = new WebhookResponse();

                // The request should include a "city-followup" output context with a cityname:
                var outputContext = request
                                        .QueryResult
                                        .OutputContexts
                                        ?.FirstOrDefault(oc => oc.Name.EndsWith("city-followup"));

                if (outputContext == null) throw new NotSupportedException("Missing context city-followup");

                DateTime now;
                string cityName = request.QueryResult.Parameters.Fields["geo-city"].StringValue;
                try
                {
                    now = TimeZoneInfo.ConvertTime(
                                            DateTime.UtcNow,
                                            _cityTimeZones.GetTimeZoneInfo(cityName)
                                        );
                    // Set the responseText that will be shown to the end user
                    response.FulfillmentText = request
                                                    .QueryResult
                                                    .FulfillmentText
                                                    .Replace("$geo-city", cityName)
                                                    .Replace("$timenow", now.ToString("HH:mm"));

                    // Add timenow field to output context
                    outputContext.Parameters.Fields.Remove("timenow");                    
                    outputContext.Parameters.Fields.Add("timenow", new Value()
                    {
                        StringValue = now.ToString("HH:mm")
                    });

                    outputContext.Parameters.Fields.Remove("timezone");
                    outputContext.Parameters.Fields.Add("timezone", new Value()
                    {
                        StringValue = _cityTimeZones.GetCityTimeZone(cityName)?.TimeZone
                    });

                    // Add the modified output context to the response
                    response.OutputContexts.Add(outputContext);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,ex.Message);
                    response.FulfillmentText = $"Sorry. I don't know the time in {cityName}. Anywhere else?";
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

        /*
         * Return pretty indented version of a json string.
         */
        private string PrettyJson(string jsonString)
        {
            return JValue.Parse(jsonString).ToString(Formatting.Indented);
        }
    }
}