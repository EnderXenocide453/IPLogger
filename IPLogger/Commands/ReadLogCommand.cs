namespace IPLogger.Commands
{
    /// <summary>
    /// Команда считывания журнала доступа
    /// </summary>
    public class ReadLogCommand : Command
    {
        private LogFileProcessor _logProcessor;

        protected override Dictionary<string, Action<string>> _parameters
            => new Dictionary<string, Action<string>>()
            {
                { "file-log", SetInputFilePath },
                { "file-output", SetOutputFilePath },
                { "time-start", SetLowerDate },
                { "time-end", SetUpperDate },
                { "address-start", SetLowerAddress },
                { "address-mask", SetUpperAddress }
            };

        protected override bool _allowExecution
        {
            get
            {
                return (_logProcessor.FileLog?.Length > 0 && _logProcessor.FileOutput?.Length > 0);
            }
        }

        public override string Description => "Команда чтения журнала доступа\n" +
            "Параметры:\n" +
            $"\tfile-log - путь к файлу журнала. По умолчанию {ConfigHandler.GetConfigValue(ConfigName.fileLog)}\n" +
            $"\tfile-output - путь к файлу вывода отчёта. По умолчанию {ConfigHandler.GetConfigValue(ConfigName.fileLog)}\n" +
            "\ttime-start - нижняя граница врменного промежутка доступа\n" +
            "\ttime-end - верхняя граница врменного промежутка доступа\n" +
            "\taddress-start - нижняя граница ip-адреса\n" +
            "\taddress-mask - верхняя граница ip-адреса. Может быть задана только после нижней";

        private void SetUpperAddress(string address) => _logProcessor.AddressMask = new IPComparable(Utils.ParseAddress(address));

        private void SetLowerAddress(string address) => _logProcessor.AddressStart = new IPComparable(Utils.ParseAddress(address));

        private void SetUpperDate(string date) => _logProcessor.TimeEnd = Utils.ParseDate(date);

        private void SetLowerDate(string date) => _logProcessor.TimeStart = Utils.ParseDate(date);

        private void SetOutputFilePath(string path) => _logProcessor.FileOutput = path;

        private void SetInputFilePath(string path) => _logProcessor.FileLog = path;

        protected override void OnExecute()
        {
            _logProcessor.ProcessLogFile();
        }

        protected override void PreExecute()
        {
            _logProcessor = new LogFileProcessor();
        }
    }
}
//read-log --file-log C:\log.txt --file-output C:\output.json --time-start 05.04.2024 --time-end 06.04.2024 --address-start 128.125.12.6 --address-mask 130.45.78.7