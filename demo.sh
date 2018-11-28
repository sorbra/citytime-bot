# demo the time-of-day here: https://bot.dialogflow.com/08bcffaa-d10e-4834-8408-1ce4bdac03d8
#demo Google Home

# walk through dialogflow
# show intents and responses

# Show Web Integration
# Show Google Actions
# Show Responses for multiple channels

# Show multiple languages

# remove fulfilment and show what happens.
# re-enable fulfilment
# show fulfilment URL
# show diagnostic info




dotnet new webapi --name mybot
cd mybot
code .

# comment out useHttpsRedirection, useMvc and addMvc in Startup.cs (Ctrl-')

dotnet add package Google.Cloud.Dialogflow.V2 --version=1.0.0-beta02
dotnet restore

# Copy Chat folder from here to new thing
# code walkthrough

# add to Startup.cs as last statement in ConfigureServices():
services.AddScoped<IDialogflowFulfillment, DialogflowFulfillment>();

# add to Startup.cs as last statement in Configure():
app.Run(
    dialogflowFulfillment.HandleFulfillmentRequest
);

dotnet run

# add docker files: Ctrl-Shift-P : Add Docker files to Workspace
# change .dockerignore */bin to bin and */obj to obj

# Build and tag the docker container
docker build -t sorbra/dfdemo:0.1 -f ./Dockerfile .

# Run container locally in Docker
docker run --detach --publish 4900:80 --name mybot sorbra/dfdemo:0.1

# Push container to Docker Hub
docker push sorbra/dfdemo:0.1

...

