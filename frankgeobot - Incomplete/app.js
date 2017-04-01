var restify = require('restify');
var builder = require('botbuilder');
var request = require('request');

//=========================================================
// Bot Setup
//=========================================================

// Setup Restify Server
var server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, function () {
    console.log('%s listening to %s', server.name, server.url);
});

// Create chat bot
var connector = new builder.ChatConnector({
    appId: process.env.BOTFRAMEWORK_APPID,
    appPassword: process.env.BOTFRAMEWORK_APPSECRET
});
var bot = new builder.UniversalBot(connector);
server.post('/api/messages', connector.listen());

//luis
const LuisModelUrl = "[ADD LUIS URL HERE]";
var recognizer = new builder.LuisRecognizer(LuisModelUrl);
var intents = new builder.IntentDialog({ recognizers: [recognizer] });
bot.dialog('/', intents);

//=========================================================
// Bots Dialogs
//=========================================================
intents.matches('capital', function (session, args) {
    // Resolve and store entity passed from LUIS.
    var country = builder.EntityRecognizer.findEntity(args.entities, 'builtin.geography.country');
    
    country = country.entity;
    request("https://restcountries.eu/rest/v1/name/" + country, function (error, response, body) {
        if (!error && response.statusCode == 200) {
            body = JSON.parse(body);
            var info = body[0];

            //In case of mulitple results, check for exact matching of country requested. If not found, then go with the first one
            for (var i = 0; i < body.length; ++i)
            {
                if (body[i].name.toLowerCase() == country)
                {
                    info = body[i];
                    i = body.length;
                }
            }

            if (info.capital) {
                session.endDialog(info.name + "'s capital is " + info.capital + " :)");
            } else {
                session.endDialog("Sorry, an error occurred. Please try again! :)");
            }
        } else {
            session.endDialog("Sorry, an error occurred. Please try again! :)");
        }
    });
})

.matches('callingcode', function (session, args) {
    // Resolve and store entity passed from LUIS.
    var country = builder.EntityRecognizer.findEntity(args.entities, 'builtin.geography.country');

    country = country.entity;
    request("https://restcountries.eu/rest/v1/name/" + country, function (error, response, body) {
        if (!error && response.statusCode == 200) {
            body = JSON.parse(body);
            var info = body[0];

            //In case of mulitple results, check for exact matching of country requested. If not found, then go with the first one
            for (var i = 0; i < body.length; ++i)
            {
                if (body[i].name.toLowerCase() == country)
                {
                    info = body[i];
                    i = body.length;
                }
            }

            if (info.capital) {
                session.endDialog(info.name + "'s calling code is " + info.callingCodes[0] + " :)");
            } else {
                session.endDialog("Sorry, an error occurred. Please try again! :)");
            }
        } else {
            session.endDialog("Sorry, an error occurred. Please try again! :)");
        }
    });
})

.matches('None', function (session) {
    session.endDialog("I'm sorry I don't understand. I can only find capitals, continents and calling codes.");
});