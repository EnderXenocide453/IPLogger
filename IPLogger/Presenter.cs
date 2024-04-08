using IPLogger.Messages;

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
            //Пробуем получить команду из консоли
            if (!TryParse(_view.GetInput(), out var commandName, out var commandValue)) {
                return;
            }
            //Пробуем выполнить команду
            TryExecuteCommand(commandName, commandValue);
        }

        /// <summary>
        /// Получить команду из строки
        /// </summary>
        /// <param name="value">Строка с командой</param>
        /// <param name="commandName">Имя команды</param>
        /// <param name="commandValue">Параметры команды</param>
        /// <returns>Удачна ли попытка</returns>
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

        /// <summary>
        /// Исполнить команду
        /// </summary>
        /// <param name="commandName">Имя команды</param>
        /// <param name="value">Параметры команды</param>
        private void TryExecuteCommand(string commandName, string value)
        {
            commandName.ToLower();
            value.ToLower();

            _model.ExecuteCommand(commandName, value);
        }
    }
}
