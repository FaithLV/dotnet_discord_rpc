using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotnet_discord_rpc;

namespace TestApplication
{
    class Program
    {
        private static string AppID = "ENTER ME PLEASE";

        static void Main(string[] args)
        {
            Console.WriteLine("Hello! Press ENTER to start Rich Presence.");
            Console.WriteLine("Don't forget to add this process to \"Discord Games\" if is not working.");
            Console.ReadLine();

            DiscordRPC client = new DiscordRPC(AppID);
            client.State = "Testing!";
            client.Details = "Presence in C#";

            Console.WriteLine("Rich Presence should now be working!");
            Console.ReadLine();
        }
    }
}
