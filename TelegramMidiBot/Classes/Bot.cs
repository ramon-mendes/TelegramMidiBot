using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;

namespace TelegramMidiBot.Classes
{
	public static class Bot
	{
		private const string BOT_API = "925635831:AAEkK38Samii6HNYUAeS3EfJZrk81qlG6Ks";
		private static TelegramBotClient _bot;

		public static TelegramBotClient GetClient()
		{
			if(_bot == null)
			{
				_bot = new TelegramBotClient(BOT_API);
				_bot.StartReceiving();
			}
			return _bot;
		}
	}
}
