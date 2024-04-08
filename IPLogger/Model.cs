using IPLogger.Commands;
using IPLogger.Messages;

namespace IPLogger
{
    public class Model
    {
        private Dictionary<string, Command> _commands = new Dictionary<string, Command>()
        {
            { "read-log", new ReadLogCommand() }
        };

        public void ExecuteCommand(string command, string parameters)
        {
            command.ToLower();

            if (!_commands.ContainsKey(command)) {
                MessageSender.SendMessage($"Команды {command} не существует!", MessageType.Error);
                return;
            }

            _commands[command].Execute(parameters);
        }
    }
}
