namespace DRTApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Console.Write("Hello, World!");
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}