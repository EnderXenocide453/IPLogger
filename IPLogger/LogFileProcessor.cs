using System.Collections;
using System.Net;
using System.Text.Json;

namespace IPLogger
{
    public class LogFileProcessor
    {
        private string _inputPath, _outputPath;
        private DateTime _lowerDate = DateTime.MinValue, _upperDate = DateTime.MaxValue;
        private IPComparable _lowerAddress = new IPComparable(IPAddress.None), 
                             _upperAddress = new IPComparable(IPAddress.Any);

        public LogFileProcessor(string inputPath, string outputPath)
        {
            _inputPath = inputPath;
            _outputPath = outputPath;
        }

        public LogFileProcessor(string inputPath,
                                string outputPath,
                                DateTime lowerDate,
                                DateTime upperDate,
                                IPComparable lowerAddress,
                                IPComparable upperAddress) : this(inputPath, outputPath)
        {
            _inputPath = inputPath;
            _outputPath = outputPath;
            _lowerAddress = lowerAddress;
            _upperAddress = upperAddress;
            _lowerDate = lowerDate;
            _upperDate = upperDate;
        }

        public void ProcessLogFile()
        {
            if (!TryReadLogFile(_inputPath, out var log)) {
                MessageSender.SendMessage($"Ошибка при чтении файла {_inputPath}!\n" +
                    $"Обработка файла прекращена", 
                    MessageType.Error);
                
                return;
            }
            
            ParseAndWriteRequests(log);
        }

        private bool TryReadLogFile(string path, out string log)
        {
            log = string.Empty;

            if (!File.Exists(path))
                return false;

            log = File.ReadAllText(path);

            return true;
        }

        private void ParseAndWriteRequests(string log)
        {
            List<string> lines = new List<string>(log.Split('\n'));
            
            ClearLines(lines);

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

        private void ClearLines(List<string> lines)
        {
            for (int i = lines.Count - 1; i >= 0; i--) {
                lines[i] = lines[i].Trim();
                
                if (lines[i] == string.Empty)
                    lines.RemoveAt(i);
            }
        }

        private bool TryParseLine(string line, out IPAddress address, out DateTime date)
        {
            //Обработка случая с параметрами

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

            return comparableAddress.CompareTo(_lowerAddress) >= 0
                && comparableAddress.CompareTo(_upperAddress) <= 0
                && date >= _lowerDate
                && date <= _upperDate;
        }

        private void WriteLog(Dictionary<string, int> requests)
        {
            var options = new JsonSerializerOptions() { WriteIndented = true };
            string log = JsonSerializer.Serialize(requests, options);

            try {
                File.WriteAllText(_outputPath, log);
                MessageSender.SendMessage("Файл успешно записан!");
            }
            catch (Exception) {
                MessageSender.SendMessage("Ошибка при записи в файл!", MessageType.Error);
            }
        }
    }
}
