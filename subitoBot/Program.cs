using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Net.Mime.MediaTypeNames;

namespace subitoBot
{
    public class Program
    {
        
        public static TelegramBotClient bot { get; set; } = new TelegramBotClient(Configuration.BotToken);

        public static string BotName { get; set; } = "";

        public static List<long> noRepeatList { get; set; } = new List<long>();

        public static string BOTstatus { get; set; } = "Выключен";

        public static List<long> listOfActiveChats = new List<long>();

        public static int _Pages { get; set; }

        public static int ItemMaxPrice { get; set; } = 0;
        public static int ItemMinPrice { get; set; } = 0;

        public static async Task Main()
        {
            if (Configuration.BotToken != "")
            {
                var me = await bot.GetMeAsync();

                me.CanReadAllGroupMessages = true;
                Console.Title = me.Username;

                BotName = me.Username;



                try
                {
                    bot.OnMessage += BotOnMessageResultRecieved;
                    bot.OnCallbackQuery += CallbackProgram;
                    bot.OnReceiveError += BotOnReceiveError;
                }
                catch(Exception e)
                {
                    Console.WriteLine("проблемв с долгим ответом на вопрос");
                }

                bot.StartReceiving(Array.Empty<UpdateType>());
                Console.WriteLine($"Start listening for @{me.Username},{me.Id.ToString()}");

                Console.ReadLine();
                bot.StopReceiving();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Не удается подключиться к боту, проверьте файл keybot.txt на наличие api ключа");
                Console.ForegroundColor = ConsoleColor.White;
                Console.ReadLine();
            }

        }

        private static async void BotOnMessageResultRecieved(object sender, MessageEventArgs message)
        {
            var me = await bot.GetMeAsync();

            message.Message.Text = message.Message.Text.Replace($"@{me.Username}", "").Trim();

            if (message.Message.Text.Contains("!Pages:"))
            {
                try
                {
                    int pages = Convert.ToInt32(message.Message.Text.Replace("!Pages:", "").Trim());

                    _Pages = pages;

                    var inlineKeyboard = new InlineKeyboardMarkup(new[]
                     {

                            new []
                            {

                                InlineKeyboardButton.WithCallbackData("Включить?","/parsepages"),
                                
                            },

                      });

                    await bot.SendTextMessageAsync(message.Message.Chat.Id, $"Я собираюсь спарсить {_Pages} страниц. Включите меня!", replyMarkup: inlineKeyboard);

                }
                catch(Exception e)
                {
                    await FilterUtils.setPages(message.Message, bot,"Введено некорректное число. Попробуйте снова");
                }
             }

            if (message.Message.Text.Contains("!setMinPrice:"))
            {
                try
                {
                    int minPrice = Convert.ToInt32(message.Message.Text.Replace("!setMinPrice:", "").Split('\n')[0].Trim());

                    ItemMinPrice = minPrice;
                }
                catch (Exception e)
                {
                    var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {

                    new []
                    {

                        InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Фильтр Цены:", $"!setMinPrice: {ItemMinPrice}\n" + $"\n!setMaxPrice: {ItemMaxPrice}"),
                        InlineKeyboardButton.WithCallbackData("Меню", "/menu"),

                    },

              });
                    await bot.SendTextMessageAsync(
                        chatId: message.Message.Chat.Id,
                        "Некорректное число у минимальной цены. Попробуйте снова.",
                        replyMarkup: inlineKeyboard

                    );
                }

            }

            if (message.Message.Text.Contains("!setMaxPrice:"))
            {
                try
                {
                    int maxPrice = Convert.ToInt32(message.Message.Text.Replace("!setMaxPrice:", "").Split('\n')[1].Trim());

                    ItemMaxPrice = maxPrice;

                    await bot.SendTextMessageAsync(
                        chatId: message.Message.Chat.Id,
                        $"Минимальная цена: {ItemMinPrice}\n Максимальная цена: {ItemMaxPrice}\n");
                }
                catch (Exception e)
                {
                    var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {

                    new []
                    {

                        InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Фильтр Цены:", $"!setMinPrice: {ItemMinPrice}\n" + $"\n!setMaxPrice: {ItemMaxPrice}"),
                        InlineKeyboardButton.WithCallbackData("Меню", "/menu"),

                    },
                   
              });
                    await bot.SendTextMessageAsync(
                        chatId: message.Message.Chat.Id,
                        "Некорректное число у максимальной цены. Попробуйте снова.",
                        replyMarkup: inlineKeyboard

                    );
                }
            }

           

            if (message.Message.Text.Contains("!StopWords:"))
            {
                try
                {
                    var stopwords = message.Message.Text.Replace("!StopWords:", "").Split(',');

                    FilterUtils filteru = new FilterUtils();
                    var listofWords = await filteru.addStopWord(stopwords.ToList());

                    var text = "";

                    var i = 0;

                    foreach (var word in listofWords)
                    {
                        text += word + ",";
                        i++;
                        if (i > 35)
                        {
                            await bot.SendTextMessageAsync(message.Message.Chat.Id, text != "" ? text : "Список пустой");
                            i = 0;
                            text = "";
                        }

                    }

                    if (i < 35 && text != "")
                    {
                        await bot.SendTextMessageAsync(message.Message.Chat.Id, text != "" ? text : "Список пустой");
                    }
                }
                catch (Exception e)
                {
                    var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {

                    new []
                    {

                        InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Добавить стоп-слово(а)", $"!StopWords: ..."),
                        InlineKeyboardButton.WithCallbackData("Меню", "/menu"),

                    },

              });
                    await bot.SendTextMessageAsync(
                        chatId: message.Message.Chat.Id,
                        "Некорректное запись. Попробуйте снова.",
                        replyMarkup: inlineKeyboard

                    );
                }
            }

            if (message.Message.Text.Contains("!AddUrl:"))
            {
                try
                {
                   var lineOfUrls = message.Message.Text.Replace("!AddUrl:", "").Split("\n");
                    string[] newList = Configuration.focusUrls.ToArray();

                    var newList2 = newList.ToList();
                   foreach (var line in lineOfUrls)
                    {
                        if(!String.IsNullOrWhiteSpace(line))
                        {
                           newList2.Add(line);
                        }
                    }
                    Configuration.focusUrls = newList2;

                    string text = "";
                    int i = 0;
                    Configuration.focusUrls.Select(x => { text += $"{i}. {x}\n"; i++; return text; }).ToList();
                    await SendUrlMenu(message.Message, $"Список Ссылок:\n{text}");
                }
                catch (Exception e)
                {
                    await bot.SendTextMessageAsync(
                         chatId: message.Message.Chat.Id,
                         "Некорректный ввод ссылки"

                     );

                    string text = "";
                    int i = 0;
                    Configuration.focusUrls.Select(x => { text += $"{i}. {x}\n"; i++; return text; }).ToList();
                    await SendUrlMenu(message.Message, $"Список Ссылок:\n{text}");

                }
            }
            if (message.Message.Text.Contains("!DelUrl:"))
            {
                try
                {
                    List<int> IndexOfUrls = new List<int>();

                    IndexOfUrls = message.Message.Text.Contains(',') ?
                        message.Message.Text.Replace("!DelUrl:", "").Replace("номера ссылок", "").Replace("*", "").Split(",").Select(x => !String.IsNullOrWhiteSpace(x) ? Convert.ToInt32(x) : -1).ToList()
                        :
                        message.Message.Text.Replace("!DelUrl:", "").Replace("номера ссылок", "").Replace("*", "").Split("\n").Select(x => !String.IsNullOrWhiteSpace(x) ? Convert.ToInt32(x) : -1).ToList();


                    string[] newList = Configuration.focusUrls.ToArray();

                    var newList2 = newList.ToList();

                    var listToDel = new List<string>();

                    foreach(var ind in IndexOfUrls)
                    {
                        if(ind != -1)
                           listToDel.Add(newList2[ind]);
                    }

                    newList2 = newList.Where(x => !listToDel.Contains(x)).ToList();
                    
                    Configuration.focusUrls = newList2;

                    string text = "";
                    int i = 0;
                    Configuration.focusUrls.Select(x => { text += $"{i}. {x}\n"; i++; return text; }).ToList();
                    await SendUrlMenu(message.Message, $"Список Ссылок:\n{text}");

                }
                catch (Exception e)
                {
                    var qe = e;
                    await bot.SendTextMessageAsync(
                         chatId: message.Message.Chat.Id,
                         "Некорректный индексы у ссылки. Возможно ввели несуществуюущий индекс"

                     );

                    string text = "";
                    int i = 0;
                    Configuration.focusUrls.Select(x => { text += $"{i}. {x}\n"; i++;  return text; }).ToList();
                    await SendUrlMenu(message.Message, $"Список Ссылок:\n{text}");

                }
            }

            switch (message.Message.Text.ToLower())
            {
                case "/start":
                    await SendMainKeyboard(message.Message);
                    break;
                case "/menu":
                case "/меню":
                    await SendMenu(message.Message, "Меню");
                    break;

                case "/выкл":
                case "/off":
                    BOTstatus = "Выключен";
                    await bot.SendTextMessageAsync(message.Message.Chat.Id, text: BOTstatus);
                    break;
                    
                case "/status":
                case "/статус":

                    await bot.SendTextMessageAsync(message.Message.Chat.Id, text: BOTstatus);

                    break;
                   
            }
           
        }



        public static async void CallbackProgram(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            var callbackQuery = callbackQueryEventArgs.CallbackQuery;

            try
            {
                await bot.DeleteMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);
            }
            catch(Exception e)
            {
                Console.WriteLine("Сообщение меню и тд удалено");

            }


            switch (callbackQuery.Data.ToLower())
            {
                case "/Error":
                    throw new Exception(message: "Тестовая ошибка");
                case "/menu":
                    await SendMenu(callbackQuery.Message, "Меню");
                    break;

                case "/настройка":
                    await FilterUtils.setPages(callbackQuery.Message, bot);
                    break;

                case "/parsepages":

                    var status = $"парисинг {_Pages} от {callbackQuery.Message.Chat.FirstName}";

                    int count = 1;

                    BOTstatus = status;

                    while (BOTstatus == status && count <= _Pages)
                    {
                         // надо поменять для каждого свои страницы 
                        
                            ThreadStart ts = new ThreadStart(async () =>
                            {
                                subitoParse avitoParse = new subitoParse();
 
                                var listItems = await avitoParse.getListOfItems(page: count++);

                                FilterUtils fu = new FilterUtils();

                                var goodItem = await fu.getFilteredListByStopWords(listItems);

                                goodItem = await fu.filterByPrice(goodItem, ItemMinPrice, ItemMaxPrice);

                                if (noRepeatList != null && noRepeatList.Any())
                                    goodItem = goodItem.Where(x => !noRepeatList.Contains(x.Id)).ToList();

                                foreach (var good in goodItem)
                                {
                                    if (noRepeatList == null || !noRepeatList.Contains(good.Id))
                                        noRepeatList.Add(good.Id);
                                    try
                                    {
                                        if (good.ImgLink != null)
                                        {
                                            await bot.SendPhotoAsync(callbackQuery.Message.Chat.Id, photo: good.ImgLink, caption: await fu.prepareRenderText(good), parseMode: ParseMode.Html);

                                        }
                                        else
                                        {

                                            await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text: await fu.prepareRenderText(good), parseMode: ParseMode.Html);
                                            Console.WriteLine($"отправлено со страницы {count}: {good.Id}");
                                        }
                                    }
                                    catch(Exception e)
                                    {
                                        Console.WriteLine("Телеграмм отклонил спам");
                                        Thread.Sleep(10000);

                                        await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text: await fu.prepareRenderText(good), parseMode: ParseMode.Html);

                                        Console.WriteLine($"(ПОСЛЕ СПАМА)отправлено со страницы {count}: {good.Id}");
                                    }
                                    if (count == _Pages && good.Id == goodItem.LastOrDefault().Id)
                                    {
                                        await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text: $"Проверенно {count} стр");
                                    }
                                }
                               

                            });
                            Thread thread = new Thread(ts);
                            thread.Start();
                            Thread.Sleep(8000);

                     }

                    

                    break;


                case "/проверкановых":
                    try
                    {
                        Configuration.createOrAddUsersIdToFile(callbackQuery.Message.Chat.Id, callbackQuery.Message.Chat.FirstName);

                        var conf = new Configuration();

                        var timestamp = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Local);
                        if (timestamp.Day % 5 == 0 && timestamp.Hour == 1)
                        {
                            noRepeatList.Clear();

                        }

                        if (BOTstatus == "Включен" && listOfActiveChats.Contains(callbackQuery.Message.Chat.Id))
                        {
                            await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text: "Бот уже парсит первую страницу");
                            return;
                        }

                        BOTstatus = "Включен";

                        listOfActiveChats.Add(callbackQuery.Message.Chat.Id);

                        while (BOTstatus == "Включен")
                        {
                            foreach (var sUrl in Configuration.focusUrls)
                            {

                                subitoParse avitoParse = new subitoParse();

                                var listItems = await avitoParse.getListOfItems(1, sUrl);

                                FilterUtils filterUtils = new FilterUtils();

                                var goodItem = await filterUtils.getFilteredListByStopWords(listItems);

                                goodItem = await filterUtils.filterByPrice(goodItem, ItemMinPrice, ItemMaxPrice);

                                if (noRepeatList != null && noRepeatList.Any())
                                    goodItem = goodItem.Where(x => !noRepeatList.Contains(x.Id)).ToList();

                                foreach (var good in goodItem)
                                {
                                    if (noRepeatList == null || !noRepeatList.Contains(good.Id))
                                        noRepeatList.Add(good.Id);

                                    int oountUsers = 0;

                                    foreach (var chatidAndName in conf.chats)
                                    {
                                        var chatid = chatidAndName.Remove(chatidAndName.IndexOf('-'));

                                        try
                                        {
                                            if (good.ImgLink != null)
                                            {
                                                await bot.SendPhotoAsync(chatid, photo: good.ImgLink, caption: await filterUtils.prepareRenderText(good), parseMode: ParseMode.Html);

                                            }
                                            else
                                            {

                                                await bot.SendTextMessageAsync(chatid, text: await filterUtils.prepareRenderText(good), parseMode: ParseMode.Html);

                                            }

                                            oountUsers++;
                                            Console.WriteLine($"отправлено о товаре с 1 стр: {good.Id}");

                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine("Телеграмм отклонил спам");
                                            await Task.Delay(7000);

                                            try
                                            {
                                                await bot.SendTextMessageAsync(chatid, text: await filterUtils.prepareRenderText(good), parseMode: ParseMode.Html);
                                                Console.WriteLine($"(После Спама)отправлено о товаре с 1 стр: {good.Id}");
                                            }
                                            catch
                                            {
                                                Console.WriteLine(chatid + " заблокировал бота");
                                                Console.WriteLine($"Удаляю чат {chatid}...");
                                                var newChatsId = conf.chats;
                                                newChatsId.RemoveAt(oountUsers);

                                                conf.updateChatsId(newChatsId);

                                                continue;
                                            }
                                        }
                                    }

                                }
                                await Task.Delay(45000);

                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());

                        callbackQueryEventArgs.CallbackQuery.Data = "/проверкановых";

                        CallbackProgram(sender, callbackQueryEventArgs);
                    }
                    break;

                case "/stopwords":

                    FilterUtils fu = new FilterUtils();
                    var text = "Cписок:";

                    var i = 0;

                    foreach (var word in fu.StopWord)
                    {
                        text += word + ",";
                        i++;
                        if(i >35)
                        {
                            await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text != "" ? text : "Список пустой");
                            i = 0;
                            text = "";
                        }
                        
                    }

                    if(i<35 && text != "")
                    {
                        await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text != "" ? text : "Список пустой");
                    }
                       
                    

                    break;

                case "/urls":

                    string urlText = $"Наблюдаемые ссылки:\n";

                    List<string> urls = Configuration.focusUrls;

                    if(urls.Any())
                    {
                        int index = 0;
                        foreach(var url in urls)
                        {
                            urlText += $"{index}. {url}\n";
                            index++;
                        }
                    }

                    text = urlText;

                    await SendUrlMenu(callbackQuery.Message, text.Replace("Наблюдаемые ссылки:\n", "") != "" ? text : "Список пустой");
                    break;

                case "/delallurls":
                    Configuration.focusUrls = new List<string>();

                    text = "";
                    Configuration.focusUrls.Select(x => { int i = 0; return text += $"{i}. {x}\n"; }).ToList();
                    await SendUrlMenu(callbackQuery.Message, $"Список Ссылок:{text}");

                    break;

                case "/выкл":
                case "/off":
                    BOTstatus = "Выключен";

                    if (listOfActiveChats.Contains(callbackQuery.Message.Chat.Id))
                        listOfActiveChats.Remove(callbackQuery.Message.Chat.Id);

                    await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text: $"Статус бота: {BOTstatus}");

                    break;
            }


            callbackQuery.Message.Text = callbackQuery.Data;

        }

        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            Console.WriteLine("Received error: {0} — {1}",
                receiveErrorEventArgs.ApiRequestException.ErrorCode,
                receiveErrorEventArgs.ApiRequestException.Message
            );
        }

        public static async Task SendMenu(Message message, string text)
        { 
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                
                    // first row
                    new []
                    {

                        InlineKeyboardButton.WithCallbackData("Парсинг 1-ой стр:","/проверкановых" ),
                        InlineKeyboardButton.WithCallbackData("Парсинг n-стр:","/настройка"),

                    },
                    new []
                    {
                        
                        InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Фильтр Цены:", $"!setMinPrice: {ItemMinPrice}\n" + $"!setMaxPrice: {ItemMaxPrice}"),
                        InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Добавить стоп-слово(а)", $"!StopWords: ..."),
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("Вывести стоп-слова", "/stopwords"),
                        InlineKeyboardButton.WithCallbackData("Ссылки", "/urls"),

                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("Выключить", "/выкл"),

                    }

              });
            await bot.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text,
                replyMarkup: inlineKeyboard

            );

        }

        public static async Task SendUrlMenu(Message message, string text)
        {
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                
                    // first row
                    new []
                    {

                        InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Добавить Ссылку", $"!AddUrl: \n"),
                        

                    },
                    new []
                    {

                        InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Удалить Ссылки по индексу", $"!DelUrl: *номера ссылок* \n"),

                    },
                    new []
                    {

                        InlineKeyboardButton.WithCallbackData("Удалить все ссылки", "/delallurls"),
                       
                    },
                    new []
                    {

                        
                        InlineKeyboardButton.WithCallbackData("Меню", "/menu"),
                    }


              });
            await bot.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text,
                replyMarkup: inlineKeyboard

            );

        }

        public static async Task SendMainKeyboard(Message message)
        {
            var mainKeyboard = new ReplyKeyboardMarkup(
                new KeyboardButton[][]
                {
                        new KeyboardButton[] { "/menu" },
                        new KeyboardButton[] { "/выкл" },
                        new KeyboardButton[] { "/статус" },

                }

             );
            mainKeyboard.ResizeKeyboard = true;
            mainKeyboard.OneTimeKeyboard = true;

            var text = $"Привет {message.Chat.FirstName}!";
            
            await bot.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text,
                replyMarkup: mainKeyboard

            );
        }

    }
}
