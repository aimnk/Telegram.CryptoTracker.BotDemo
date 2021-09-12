using NLog;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.CryptoTracker.Bot.Services.Commands;

namespace Telegram.CryptoTracker.Bot.Services
{
    public class UpdateService : IUpdateService
    {
        private readonly IBotService _botService;
        private static List<Command> _commandsList;
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        public static IReadOnlyList<Command> Commands => _commandsList.AsReadOnly();

        public UpdateService(IBotService botService)
        {
            _botService = botService;
        }
       
        public async Task EchoAsync(Update update)
        {
            
            _commandsList = new List<Command>();
            _commandsList.Add(new GetPriceCrypto(_botService));
            _commandsList.Add(new AddCryptoToken(_botService));
            _commandsList.Add(new GetPortfolio(_botService));
            _commandsList.Add(new DeletePortfolio(_botService));
            _commandsList.Add(new Help(_botService));
            _commandsList.Add(new Start(_botService));
            _commandsList.Add(new Back(_botService));

            var message = update.Message;

            var commands = UpdateService.Commands;

            switch (update.Type)
            {

                case UpdateType.Message:

                    _logger.Info($"Received Message from {message.Chat.Id} Message: {message.Text}");

                    foreach (var command in commands)
                    {
                        if (command.Contains(message))
                        {
                                await command.Execute(update, _botService);
                        }
                    }

                    break;

                case UpdateType.CallbackQuery:
                    {

                        _logger.Info($"Received callback from {update.CallbackQuery.Message.Chat.Id} callback_data: {update.CallbackQuery.Data}");

                        if (update.CallbackQuery != null)
                        {
                            foreach (var command in commands)
                            {
                                command.Oncallback(update, _botService);
                            }
                            break;
                        }
                        else
                            break;
                    }
            }
           
        }
    }
}
