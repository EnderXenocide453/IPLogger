namespace IPLogger
{
    public class Presenter
    {
        private Model _model;
        private View _view;

        public Presenter(Model model, View view)
        {
            _model = model;
            _view = view;
        }

        public void WaitForCommand()
        {
            if (!TryParse(_view.GetInput(), out var commandName, out var commandValue)) {
                return;
            }
            TryExecuteCommand(commandName, commandValue);
        }

        private bool TryParse(string value, out string commandName, out string commandValue)
        {
            var commandPare = value.Split(' ', 2);
            
            commandName = commandPare[0];

            if (commandName == string.Empty) {
                MessageSender.SendMessage("Команда не может быть пустой", MessageType.Warning);

                commandValue = string.Empty;

                return false;
            }

            commandName = commandPare[0];
            commandValue = commandPare.Length == 2 ? commandPare[1] : string.Empty;

            return true;
        }

        private bool TryExecuteCommand(string commandName, string value)
        {
            commandName.ToLower();
            value.ToLower();

            _model.ExecuteCommand(commandName, value);

            return true;
        }
    }
}
