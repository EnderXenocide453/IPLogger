using System.Net;

namespace IPLogger
{
    public static class Utils
    {
        public static DateTime ParseDate(string dateString)
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

        public static IPAddress ParseAddress(string addressString)
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

        public static void ClearLines(List<string> lines)
        {
            for (int i = lines.Count - 1; i >= 0; i--) {
                lines[i] = lines[i].Trim();

                if (lines[i] == string.Empty)
                    lines.RemoveAt(i);
            }
        }

        public static bool TryReadFile(string path, out string filecontent)
        {
            filecontent = string.Empty;

            if (!File.Exists(path))
                return false;

            filecontent = File.ReadAllText(path);

            return true;
        }
    }
}
