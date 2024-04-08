namespace IPLogger
{
    public class View
    {
        public string GetInput()
        {
            Console.Write("Ожидание команды: ");

            return Console.ReadLine() ?? string.Empty;
        }
    }
}
