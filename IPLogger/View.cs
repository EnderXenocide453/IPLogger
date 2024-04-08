namespace IPLogger
{
    /// <summary>
    /// Класс взаимодействия с пользователем
    /// </summary>
    public class View
    {
        public string GetInput()
        {
            Console.Write("Ожидание команды: ");

            return Console.ReadLine() ?? string.Empty;
        }
    }
}
