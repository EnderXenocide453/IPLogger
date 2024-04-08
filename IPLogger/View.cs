namespace IPLogger
{
    /// <summary>
    /// Класс взаимодействия с пользователем
    /// </summary>
    public class View
    {
        public string GetInput()
        {
            Console.Write("Ожидание команды (введите help для справки)...\n");

            return Console.ReadLine() ?? string.Empty;
        }
    }
}
