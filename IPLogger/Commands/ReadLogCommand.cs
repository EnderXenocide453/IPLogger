using System.Net;

namespace IPLogger.Commands
{
    internal class ReadLogCommand : Command
    {
        private string _inputPath;
        private string _outputPath;
        private DateTime _lowerDate;
        private DateTime _upperDate;
        private IPComparable _lowerAddress = IPComparable.MinValue;
        private IPComparable _upperAddress = IPComparable.MaxValue;

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
                return (_inputPath?.Length > 0 && _outputPath?.Length > 0);
            }
        }

        private void SetUpperAddress(string address)
        {
            if (_lowerAddress.Address == IPAddress.None) {
                MessageSender.SendMessage("Верхняя граница адреса может быть задана исключительно после нижней!\n" +
                    "Параметр будет проигнорирован", 
                    MessageType.Warning);

                return;
            }

            _upperAddress.Address = ParseAddress(address);
        }

        private void SetLowerAddress(string address) => _lowerAddress.Address = ParseAddress(address);

        private IPAddress ParseAddress(string addressString)
        {
            addressString = addressString.Replace(" ", string.Empty);

            if (!IPAddress.TryParse(addressString, out var address)) {
                MessageSender.SendMessage(
                    $"{addressString} является неправильной формой записи IPv4 адреса!",
                    MessageType.Error);
                address = IPAddress.None;
            }

            return address;
        }

        private void SetUpperDate(string date) =>  _upperDate = ParseDate(date);

        private void SetLowerDate(string date) => _lowerDate = ParseDate(date);

        private DateTime ParseDate(string dateString)
        {
            if (!DateTime.TryParse(dateString, out var date)) {
                MessageSender.SendMessage(
                    $"{dateString} не может быть преобразован в дату. " +
                    $"Используйте форму записи 'dd.MM.yyyy'.\n" +
                    $"Эта дата будет проигнорирована",
                    MessageType.Warning);

                date = DateTime.MaxValue;
            }

            return date;
        }

        private void SetOutputFilePath(string path)
        {
            if (path == string.Empty || path == null) {
                MessageSender.SendMessage(
                    "Не задан путь результирующего файла!\n" +
                    "Операция не будет выполнена!", MessageType.Error);

                return;
            }

            _outputPath = path;
        }

        private void SetInputFilePath(string path)
        {
            if (path == string.Empty || path == null) {
                MessageSender.SendMessage(
                    "Не задан путь файла логов!\n" +
                    "Операция не будет выполнена!", MessageType.Error);

                return;
            }

            if (!File.Exists(path)) {
                MessageSender.SendMessage(
                    $"Файла с именем {path} не существует!\n" +
                    "Операция не будет выполнена!", MessageType.Error);

                return;
            }

            _inputPath = path;
        }

        private void VerifyBounds()
        {
            if (_lowerAddress.CompareTo(_upperAddress) > 0) {
                MessageSender.SendMessage("Верхняя граница адреса должна быть больше нижней.\n" +
                    "Границы адреса будут проигнорированы",
                    MessageType.Warning);

                _lowerAddress = IPComparable.MinValue;
                _upperAddress = IPComparable.MaxValue;
            }

            if (_lowerDate > _upperDate) {
                MessageSender.SendMessage("Верхняя граница даты должна быть больше нижней.\n" +
                    "Границы даты будут проигнорированы",
                    MessageType.Warning);

                _lowerDate = DateTime.MinValue;
                _upperDate = DateTime.MaxValue;
            }
        }

        protected override void OnExecute()
        {
            VerifyBounds();

            LogFileProcessor logProcessor = new LogFileProcessor(
                _inputPath, _outputPath, 
                _lowerDate, _upperDate, 
                _lowerAddress, _upperAddress);

            logProcessor.ProcessLogFile();
        }

        protected override void PreExecute()
        {
            _inputPath = string.Empty;
            _outputPath = string.Empty;
            _lowerDate = DateTime.MinValue;
            _upperDate = DateTime.MaxValue;
            _lowerAddress = IPComparable.MinValue;
            _upperAddress = IPComparable.MaxValue;
        }
    }
}
//read-log --file-log C:\log.txt --file-output C:\output.json --time-start 05.04.2024 --time-end 06.04.2024 --address-start 128.125.12.6 --address-mask 130.45.78.7