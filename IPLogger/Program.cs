namespace IPLogger
{
    public static class Program
    {
        private static Presenter _presenter;

        public static void Main(string[] args)
        {
            ConfigHandler.ReadConfig();
            
            View view = new View();
            Model model = new Model();
            _presenter = new Presenter(model, view);

            LifeCycle();
        }

        private static void LifeCycle()
        {
            while (true) {
                _presenter.WaitForCommand();
            }
        }
    }
}