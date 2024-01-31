##Бот для итальянской биржи объявлений
Стек: С#,.NET 7
Библиотеки: [Telegram.Bot] (https://github.com/TelegramBots/Telegram.Bot)

Запуск
Внутри содержится файл солюшен который можно запустить в VStudio или в VS code с помошью расширения С# dev kit

нужен фреймворк dotnet
Win:
`winget install Microsoft.DotNet.AspNetCore.8`

Linux:
`sudo apt-get update && \
  sudo apt-get install -y dotnet-sdk-8.0`


chats.txt - id юзеров для отправки уведомлений
url.txt - ссылки для парсинга (разделитель \n)
stop.txt - стоп слова для фильтрации 
keybot.txt - api ключ бота (BotFather в телеграмме)
