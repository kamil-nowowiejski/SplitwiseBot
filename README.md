# SplitwiseBot

## Configuration

To correctly configure SpltiwiseBot follow this steps:
1. Go to [Splitwise applications](https://secure.splitwise.com/apps) and register your app.
2. Paste the Consumer Key and Consumer Secret to appsettings.json.
3. Optionally change name of the group and your local currency.

## Testing
### Running bot locally
Bot can be hosted locally like every other ASP .NET Core application. To start interaction with the bot download [BotFramework Emulator](https://github.com/Microsoft/BotFramework-Emulator) and start conversation by opening SplitwiseBot.bot file.

### Exposing bot to internet
Bot can exposed to outer world using [ngrok](https://ngrok.com) or Microsoft Azure.
