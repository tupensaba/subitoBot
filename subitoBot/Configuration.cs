using System;
using System.Configuration;
using System.IO;

namespace subitoBot
{
    public class Configuration
    {

        public static string BotToken
        {
            get
            {
                try
                {

                    var apikey = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "keybot.txt")).Trim();

                    Console.WriteLine(apikey);

                    return apikey;

                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Не найден keybot.txt\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return "";

                }
            }
        }

        public List<string> chats
        {
            get
            {
                try
                {

                    var chats = System.IO.File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "chats.txt")).Trim().Split('\n');

                    return chats.ToList();
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Не найден chats.txt\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return new List<string>();

                }
            }
           
        }

        public static List<string> focusUrls
        {
            get
            {
                try
                {

                    var Url = System.IO.File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, "url.txt"));

                    return Url.Where(x => !String.IsNullOrWhiteSpace(x)).ToList();
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Не найден url.txt\n");
                    Console.ForegroundColor = ConsoleColor.White;

                    System.IO.File.CreateText(Path.Combine(Environment.CurrentDirectory, "url.txt"));

                    Console.WriteLine("создан файл url.txt\n");

                    return new List<string>();

                }
            }
            set
            {
               string contentText = "";
               foreach (var url in value)
               {
                    contentText += url + "\n";
               }

               System.IO.File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "url.txt"), contentText + "\n");

            }

        }


        public static long BotChannel
        {
            get
            {

                try
                {

                    var channelid = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "channel.txt")).Trim();

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("ID канала:" + channelid);
                    Console.ForegroundColor = ConsoleColor.White;

                    return Convert.ToInt64(channelid);
                }
                catch (Exception e)
                {



                    return 0;

                }
            }
        }


        public static void createOrAddUsersIdToFile(long userId, string name)
        {
            var conf = new Configuration();

            foreach (var u in conf.chats)
            {
                if (u.Contains(userId.ToString() + $"-{name}",StringComparison.InvariantCultureIgnoreCase))
                {
                    return;
                }
            }
            if (System.IO.File.Exists(Path.Combine(Environment.CurrentDirectory, "chats.txt")))
            {
                    if (userId != 0)
                        System.IO.File.AppendAllText(Path.Combine(Environment.CurrentDirectory, "chats.txt"),userId.ToString() + $"-{name}\r" + "\n");
            }
            else
            {
                using (var fileStream = System.IO.File.Create(Path.Combine(Environment.CurrentDirectory, "chats.txt")))
                {

                }
                if (userId != 0)
                    System.IO.File.AppendAllText(Path.Combine(Environment.CurrentDirectory, "chats.txt"), userId.ToString() + $"-{name}\r" + "\n");

            }

        }

        public void updateChatsId(List<string> chatIds)
        {      
                System.IO.File.WriteAllLines(Path.Combine(Environment.CurrentDirectory, "chats.txt"), chatIds);

        }
    }
}

