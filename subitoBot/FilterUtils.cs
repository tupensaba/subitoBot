using System;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace subitoBot
{
	public class FilterUtils
	{
		public List<string> StopWord
        {
            get
            {
                try
                {

                    var _stop = System.IO.File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "stop.txt")).Split(',');

                    return _stop.ToList();

                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Фильтрация без стоп слов");
                    Console.ForegroundColor = ConsoleColor.White;
                    return new List<string>() { };

                }

            }
            
        }

		public FilterUtils()
		{
		}

        
        public  async Task<List<string>> addStopWord(List<string> words)
        {
            if (System.IO.File.Exists(Path.Combine(Environment.CurrentDirectory, "stop.txt")))
            {
                foreach (var word in words)
                {
                    if (word != "." || !string.IsNullOrWhiteSpace(word))
                        System.IO.File.AppendAllText(Path.Combine(Environment.CurrentDirectory, "stop.txt"), word + ",");
                }
            }
            else
            {
                using (var fileStream = System.IO.File.Create(Path.Combine(Environment.CurrentDirectory, "stop.txt")))
                {

                }
                    foreach (var word in words)
                    {
                        if (word != "." || !string.IsNullOrWhiteSpace(word))
                        System.IO.File.AppendAllText(Path.Combine(Environment.CurrentDirectory, "stop.txt"), word + ",");
                }
                
            }

            return StopWord;
        }
        

        public async Task<List<SubitoItemModel>> getFilteredListByStopWords(List<SubitoItemModel> listAItem)
        {
            var listIdForDel = new List<long>();
            if(!StopWord.Any())
            {
                return listAItem;
            }

            foreach (var aItem in listAItem)
            {
                foreach (var word in StopWord)
                {
                    if(string.IsNullOrWhiteSpace(word))
                    {
                        continue;
                    }
                    if (aItem.Title.ToLower().Contains(word.ToLower().Trim()))
                    {
                        listIdForDel.Add(aItem.Id);
                        continue;
                    }
                }
            }

            listAItem = listAItem.Where(x => !listIdForDel.Contains(x.Id)).ToList();

            return listAItem;
        }

        public async Task<string> prepareRenderText(SubitoItemModel AIM)
        {
            var renderedText = $"{AIM.Title} \n";

            renderedText += $"\nЦена: {AIM.Price} \n";

            if (AIM.City != null)
            renderedText += $"\nГород: {AIM.City} \n\n";

            if (AIM.Discription != null)
                renderedText += $"\nОписание:{AIM.Discription}\n\n\n";

            renderedText += $"\n{AIM.Link}";

            return renderedText;
        }

        public async Task<List<SubitoItemModel>> filterByPrice(List<SubitoItemModel> listAIM,int minPrice, int maxPrice)
        {
            if(minPrice == 0 && maxPrice == 0)
            {
                return listAIM;
            }

            if (listAIM.Count != 0 && listAIM.Select(x => x.Price).Any()) { 

            return listAIM.Where(x => Convert.ToInt32(x.Price.Remove(x.Price.Length - 1).Replace("\u00A0", "").Trim()) >= minPrice && maxPrice >= Convert.ToInt32(x.Price.Remove(x.Price.Length - 1).Replace("\u00A0", "").Trim())).ToList();
            }

            return listAIM;
        }

        public static async Task setPages(Message message, TelegramBotClient bot, string preText = "default")
        {
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                
                    // first row
                    new []
                    {
                        InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Ввести кол-во страниц","!Pages: 10"),

                    },
                    
              });

            var text = "!!Произойдет остановка другого парса!! Вы выбрали парсинг n страниц.Напишите какое кол-во страниц вы хотите проверить. \nДля этого выберите пункт из меню чата\n";
            text += "(Максимальное кол-во стр на сайте 100)";

            await bot.SendTextMessageAsync(
                chatId: message.Chat.Id,
                preText == "default" ? text : preText,
                replyMarkup: inlineKeyboard

            );

        }

    }
}

