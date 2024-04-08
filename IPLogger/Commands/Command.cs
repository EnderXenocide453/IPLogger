using IPLogger.Messages;

namespace IPLogger.Commands
{
    public abstract class Command
    {
        private const string ParamsSplitter = "--";
        private Parameter[] _parametersQueue = Array.Empty<Parameter>();

        /// <summary>
        /// Параметры и ссылки на выполняемые ими методы
        /// </summary>
        protected abstract Dictionary<string, Action<string>> _parameters { get; }
        protected abstract bool _allowExecution { get; }

        public void Execute(string paramsString)
        {
            PreExecute();
            
            ParseParameters(paramsString);
            ApplyParameters();

            if (!_allowExecution) {
                MessageSender.SendMessage("Команда не была выполнена в связи с возникшими ошибками!", MessageType.Error);

                return;
            }

            OnExecute();
        }

        public bool TryGetParameterAction(string paramName, out Action<string> action)
        {
            return _parameters.TryGetValue(paramName, out action);
        }

        private void ApplyParameters()
        {
            foreach (var parameter in _parametersQueue) {
                _parameters[parameter.name].Invoke(parameter.value);
            }
        }

        private void ParseParameters(string paramsString)
        {
            HashSet<Parameter> tmp = new HashSet<Parameter>();
            var parameterPairs = paramsString.Split(ParamsSplitter);

            foreach (var parameter in parameterPairs) {
                var parsedPair = parameter.Split(' ', 2);

                string name = parsedPair[0];

                if (name == string.Empty)
                    continue;

                string value = parsedPair.Length == 2 ? parsedPair[1] : string.Empty;

                if (!_parameters.ContainsKey(name)) {
                    MessageSender.SendMessage($"У этой команды не существует параметра {name}.\n" +
                        $"Выполнение команды продолжится без учёта этого параметра",
                        MessageType.Warning);

                    continue;
                }
                    
                tmp.Add(new Parameter { name = name, value = value });
            }

            _parametersQueue = tmp.ToArray();
        }

        /// <summary>
        /// Выполняется перед считыванием параметров и выполнением
        /// </summary>
        protected abstract void PreExecute();
        /// <summary>
        /// Выполнить команду. Вызывается после обработки параметров
        /// </summary>
        protected abstract void OnExecute();

        private struct Parameter : IEquatable<Parameter>
        {
            public string name;
            public string value;

            public bool Equals(Parameter other)
            {
                return name == other.name;
            }
        }
    }
}
