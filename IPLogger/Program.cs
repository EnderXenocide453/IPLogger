namespace IPLogger
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            ConfigHandler.ReadConfig();
            
            View view = new View();
            Model model = new Model();

            LifeCycle(new Presenter(model, view));
        }

        private static void LifeCycle(Presenter presenter)
        {
            while (true) {
                presenter.WaitForCommand();
            }
        }
    }
}