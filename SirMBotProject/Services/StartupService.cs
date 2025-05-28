using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SirMBotProject.Services
{
    public class StartupService
    {
        private readonly CommandHandler _commandHandler;
        private readonly InteractionHandler _interactionHandler;

        public StartupService(CommandHandler commandHandler, InteractionHandler interactionHandler)
        {
            _commandHandler = commandHandler;
            _interactionHandler = interactionHandler;

        }

        public async Task InitializeAsync()
        {
            await _commandHandler.InitializeAsync();
            await _interactionHandler.InitializeAsync();
        }
    }
}
