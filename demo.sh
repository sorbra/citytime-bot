# demo the time-of-day here: https://bot.dialogflow.com/08bcffaa-d10e-4834-8408-1ce4bdac03d8

# walk throug dialogflow
# show intents and responses
# demo with "don't know the time"

dotnet new webapi --name mybot
cd mybot
code .

# comment out useHttpsRedirection in Startup.cs (Ctrl-')

dotnet run
# open browser on http://localhost:5000/api/values

dotnet add package Google.Cloud.Dialogflow.V2 --version=1.0.0-beta02
dotnet restore

# create new file DialogflowFulfillment.cs 
# copy contents of this DialogflowFulfillment.cs
# ctrl-K ctrl-0 to collapse all
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
docker build -t sorbra/dfdemo:1.0 -f ./Dockerfile .

# Run container locally in Docker
docker run --detach --publish 4900:80 --name mybot sorbra/dfdemo:1.0

# Push container to Docker Hub
docker push sorbra/dfdemo:1.0

kubectl config use-context DIMA-Dev-Cluster
...

