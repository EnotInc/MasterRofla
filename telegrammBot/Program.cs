using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

Dictionary<string, string> phoneBook = new Dictionary<string, string>();
Dictionary<DateTime, int> DateToJoke = new Dictionary<DateTime, int>();

string[] ListOfJokes = 
{
    "Маленький Коля смотрит на большую грязную лужу с ледышками и грязными палками в ней и говорит: \n- Ой, какая прелесть, но как жаль, что можно только смотреть!",
    "Есть 10 типов людей: \nте кто понимают двоичную систему счисления и те кто нет",
    "'Я знаю все про Эффект Да́ннинга — Крю́гера'",
    "Что имеен 3 головы и 7 х*ев \n- Трехголовый-семихуй \nby Егор Широбоков?",
    "- How to stop any rape? \nJust by saying 'yes'"
};
string[] VideoJokes =
{
    "https://www.youtube.com/watch?v=WxiB6tA7RP4",
    "https://www.youtube.com/watch?v=XuBydGbCMLg",
    "https://www.youtube.com/watch?v=1Qo35ldlvWo"
};

using var cts = new CancellationTokenSource();
var bot = new TelegramBotClient("7311045436:AAFfH3dv3Aot0jC6hIorL4fLSw4O-5Yf3Vk", cancellationToken: cts.Token);
var me = await bot.GetMeAsync();

bool waitingForName = false;
string name = "";

bot.OnError += OnError;
bot.OnMessage += OnMessage;
bot.OnUpdate += OnUpdate;

Console.WriteLine($"@{me.Username} is running... Press Enter to terminate");
Console.ReadLine();
cts.Cancel(); // stop the bot

// method to handle errors in polling or in your OnMessage/OnUpdate code
async Task OnError(Exception exception, HandleErrorSource source)
{
    Console.WriteLine(exception); // just dump the exception to the console
}

async Task OnMessage(Message msg, UpdateType type)
{
    var replymarkup = new ReplyKeyboardMarkup(true);
    var inlineMarkup = new InlineKeyboardMarkup();

    if (msg.Text == "/start")
    {
        bool phoneExist1 = phoneBook.ContainsKey(Convert.ToString(msg.Chat.Id));
        if (!phoneExist1)
        {
            await bot.SendTextMessageAsync(msg.Chat, "Введите ваше имя");
        }
        else
        {
            await bot.SendTextMessageAsync(msg.Chat, "Вы уже есть в системе");
        }
        waitingForName = true;
    }

    else if (waitingForName)
    {
        name = msg.Text;

        phoneBook.Add(Convert.ToString(msg.Chat.Id), name);
        waitingForName = false;
        
        var replyMarkup = new ReplyKeyboardMarkup(true)
        .AddButtons("Шутка дня", "Случайная шутка")
        .AddNewRow("Видео - прикол", "Это шутка?");

        var send = await bot.SendTextMessageAsync(msg.Chat, $"Здравствуйте, {name}", replyMarkup: replyMarkup);
    }
    else if (msg.Text == "Шутка дня")
    {
        DateTime today = DateTime.Today;
        bool Dated = DateToJoke.ContainsKey(today);

        var rand = new Random();
        int numberOfJoke = rand.Next(0, ListOfJokes.Length - 1);

        if (!Dated)
        {
            DateToJoke.Add(today, numberOfJoke);
        }

        await bot.SendTextMessageAsync(msg.Chat, ListOfJokes[DateToJoke[today]]);

    }
    else if (msg.Text == "Это шутка?")
    {
        await bot.SendTextMessageAsync(msg.Chat,"Если хочешь рассмешить бога - \n расскажи ему о своих планах");
    }
    else if (msg.Text == "Случайная шутка")
    {
        var rand = new Random();
        int numberOfJoke = rand.Next(0,ListOfJokes.Length - 1);

        await bot.SendTextMessageAsync(msg.Chat, ListOfJokes[numberOfJoke]);
    }
    else if (msg.Text == "Видео - прикол")
    {
        var rand = new Random();
        int numberOfJoke = rand.Next(0, VideoJokes.Length - 1);

        await bot.SendTextMessageAsync(msg.Chat, VideoJokes[numberOfJoke]);
    }
    else if (msg.Text == "test")
    {
        var send = await bot.SendTextMessageAsync(msg.Chat, Convert.ToString(msg.Chat.Id));
    }

    if (msg.Text == "Clear")
    {
        phoneBook.Clear();
        await bot.SendTextMessageAsync(msg.Chat, "phoneBook cleared");
    }
}

async Task OnUpdate(Update update)
    {
    if (update is { CallbackQuery: { } query }) // non-null CallbackQuery
    {

    }
}
