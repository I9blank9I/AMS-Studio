using System;
using System.Windows.Forms;
using ApprenticeManagement;
using ConsoleApp1;

namespace ConsoleApp1;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        
        // Auto-load our JSON data before the UI starts
        Database.Load(); 
        
        // Start with the Login Screen
        Application.Run(new LoginForm());
    }
}