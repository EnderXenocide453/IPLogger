using IPLogger.Commands;
using IPLogger.Messages;

namespace IPLogger
{
    public class Model
    {
        public Dictionary<string, Command> Commands { get; private set; }

        public Model()
        {
            Commands = new Dictionary<string, Command>()
            {
                { "read-log", new ReadLogCommand() },
                { "help", new HelpCommand(this) }
            };
        }

        public void ExecuteCommand(string command, string parameters)
        {
            command.ToLower();

            if (!Commands.ContainsKey(command)) {
                MessageSender.SendMessage($"Команды {command} не существует!", MessageType.Error);
                return;
            }

            Commands[command].Execute(parameters);
        }
    }
}
