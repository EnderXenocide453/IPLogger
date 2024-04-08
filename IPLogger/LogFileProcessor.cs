using System.IO;
using System.Net;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IPLogger
{
    public class LogFileProcessor
    {
        private string _fileLog, _fileOutput;
        private DateTime _timeStart, _timeEnd;
        private IPComparable _addressStart, _addressMask;

        private bool _addressStartExists = false;

        public string FileLog 
        { 
            get => _fileLog; 
            set
            {
                _fileLog = string.Empty;

                if (value == string.Empty || value == null) {
                    MessageSender.SendMessage(
                        "Пустой путь журнала!\n" +
                        "Операция не будет выполнена!", MessageType.Error);

                    return;
                }

                if (!File.Exists(value)) {
                    MessageSender.SendMessage(
                        $"Файла с именем {value} не существует!\n" +
                        "Операция не будет выполнена!", MessageType.Error);

                    return;
                }

                _fileLog = value;
            } 
        }
        public string FileOutput 
        { 
            get => _fileOutput; 
            set
            {
                if (value == string.Empty || value == null) {
                    MessageSender.SendMessage(
                        "Пустой путь результирующего файла!\n" +
                        "Операция не будет выполнена!", MessageType.Error);

                    return;
                }

                _fileOutput = value;
            } 
        }
        public DateTime TimeStart { get => _timeStart; set => _timeStart = value; }
        public DateTime TimeEnd { get => _timeEnd; set => _timeEnd = value; }
        public IPComparable AddressStart 
        { 
            get => _addressStart;
            set
            {
                _addressStart = value;
                _addressStartExists = true;
            }
        }
        public IPComparable AddressMask 
        { 
            get => _addressMask; 
            set 
            {
                if (!_addressStartExists) {
                    MessageSender.SendMessage("Верхняя граница адреса может быть задана исключительно после нижней!\n" +
                        "Параметр будет проигнорирован", 
                        MessageType.Warning);

                    _addressMask = IPComparable.MaxValue;
                    return;
                }

                _addressMask = value;
            }
        }

        public LogFileProcessor()
        {
            _fileLog = ConfigHandler.GetConfigValue(ConfigName.fileLog);
            _fileOutput = ConfigHandler.GetConfigValue(ConfigName.fileOutput);
            _timeStart = Utils.ParseDate(ConfigHandler.GetConfigValue(ConfigName.timeStart));
            _timeEnd = Utils.ParseDate(ConfigHandler.GetConfigValue(ConfigName.timeEnd));
            _addressStart = new IPComparable(Utils.ParseAddress(ConfigHandler.GetConfigValue(ConfigName.addressStart)));
            _addressMask = new IPComparable(Utils.ParseAddress(ConfigHandler.GetConfigValue(ConfigName.addressMask)));

            _addressStartExists = false;
        }

        public void ProcessLogFile()
        {
            if (!Utils.TryReadFile(FileLog, out var log)) {
                MessageSender.SendMessage($"Ошибка при чтении файла {FileLog}!\n" +
                    $"Обработка файла прекращена", 
                    MessageType.Error);
                
                return;
            }

            VerifyBounds();
            ParseAndWriteRequests(log);
        }

        private void VerifyBounds()
        {
            if (_addressStart.CompareTo(_addressMask) > 0) {
                MessageSender.SendMessage("Верхняя граница адреса должна быть больше нижней.\n" +
                    "Границы адреса будут проигнорированы",
                    MessageType.Warning);

                _addressStart = IPComparable.MinValue;
                _addressMask = IPComparable.MaxValue;
            }

            if (_timeStart > _timeEnd) {
                MessageSender.SendMessage("Верхняя граница даты должна быть больше нижней.\n" +
                    "Границы даты будут проигнорированы",
                    MessageType.Warning);

                _timeStart = DateTime.MinValue;
                _timeEnd = DateTime.MaxValue;
            }
        }

        private void ParseAndWriteRequests(string log)
        {
            List<string> lines = new List<string>(log.Split('\n'));
            
            Utils.ClearLines(lines);

            Dictionary<string, int> requests = new Dictionary<string, int>();
            int count = 0;

            foreach (string line in lines) {
                if (TryParseLine(line, out var address, out var date) && CheckBounds(address, date)) {
                    string ip = address.ToString();
                    count++;

                    if (requests.ContainsKey(ip)) {
                        requests[ip]++;
                        continue;
                    }

                    requests.Add(ip, 1);
                }
            }

            MessageSender.SendMessage($"Найдено записей: {count}.");

            WriteLog(requests);
        }

        private bool TryParseLine(string line, out IPAddress address, out DateTime date)
        {
            var pair = line.Split(':', 2);
            address = IPAddress.None;
            date = DateTime.MinValue;
            
            if (pair.Length < 2)
                return false;

            if (!IPAddress.TryParse(pair[0].Trim(), out address)) {
                MessageSender.SendMessage($"{pair[0]} не является ip адресом", MessageType.Warning);
                return false;
            }

            if (!DateTime.TryParse(pair[1].Trim(), out date)) {
                MessageSender.SendMessage($"{pair[1]} не является датой", MessageType.Warning);
                return false;
            }

            return true;
        }

        private bool CheckBounds(IPAddress address, DateTime date)
        {
            IPComparable comparableAddress = new IPComparable(address);

            return comparableAddress.CompareTo(AddressStart) >= 0
                && comparableAddress.CompareTo(AddressMask) <= 0
                && date >= TimeStart
                && date <= TimeEnd;
        }

        private void WriteLog(Dictionary<string, int> requests)
        {
            var options = new JsonSerializerOptions() { WriteIndented = true };
            string log = JsonSerializer.Serialize(requests, options);

            try {
                File.WriteAllText(FileOutput, log);
                MessageSender.SendMessage("Файл успешно записан!");
            }
            catch (Exception) {
                MessageSender.SendMessage("Ошибка при записи в файл!", MessageType.Error);
            }
        }
    }
}
