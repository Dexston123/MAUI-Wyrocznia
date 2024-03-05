using Microsoft.Maui.Controls;
using System.IO;
using Application = Microsoft.Maui.Controls.Application;

namespace Wyrocznia
{
    public partial class App : Application
    {
        public static string FolderPath { get; private set; }

        public App()
        {
            InitializeComponent();
            string nazwaAplikacji = "Wyrocznia";
            FolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), nazwaAplikacji);
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }
            MainPage = new AppShell();
        }
    }
}