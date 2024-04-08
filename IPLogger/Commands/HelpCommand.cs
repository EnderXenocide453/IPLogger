
using IPLogger.Messages;

namespace IPLogger.Commands
{
    internal class HelpCommand : Command
    {
        private Model _model;
        private string _output;

        private string _defaultOutout
        {
            get
            {
                string output = "Список доступных команд:\n";
                foreach (var command in _model.Commands.Keys)
                    output += $"\t{command}\n";
                output += "Чтобы подробнее узнать о команде введите help --command <имя команды> без треугольных скобок";

                return output;
            }
        }

        protected override Dictionary<string, Action<string>> _parameters => new Dictionary<string, Action<string>>() 
        {
            { "command", SetCommandHelp }
        };

        protected override bool _allowExecution => true;

        public override string Description => "Справка.\n" +
            "Чтобы подробнее узнать о конкретной команде введите help --command <имя команды> без треугольных скобок";

        public HelpCommand(Model model)
        {
            _model = model;
        }

        private void SetCommandHelp(string name)
        {
            if (!_model.Commands.ContainsKey(name)) {
                MessageSender.SendMessage($"команды с именем {name} не существует!", MessageType.Warning);
                return;
            }

            _output = _model.Commands[name].Description;
        }

        protected override void OnExecute()
        {
            MessageSender.SendMessage(_output);
        }

        protected override void PreExecute()
        {
            _output = _defaultOutout;
        }
    }
}
