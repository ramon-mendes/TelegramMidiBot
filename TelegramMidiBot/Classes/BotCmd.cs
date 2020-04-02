using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramMidiBot.Classes
{
	public static class BotCmd
	{
		public static void StartBot()
		{
			var botClient = Bot.GetClient();
			botClient.OnMessage += Bot_OnMessage;
		}

		private static async void Bot_OnMessage(object sender, MessageEventArgs e)
		{
			var botClient = Bot.GetClient();

			if(e.Message.Text == null)
				return;
			if(e.Message.Text.StartsWith("/"))
			{
				InlineKeyboardButton urlButton = new InlineKeyboardButton();
				urlButton.Text = "Go URL";
				urlButton.Url = "https://google.com";
				// InlineKeyboardButton.WithUrl("asda", "sdasdasd")

				var list = new List<InlineKeyboardButton>();
				list.Add(urlButton);

				var rkm = new InlineKeyboardMarkup(urlButton);
				
				await botClient.SendTextMessageAsync(
				   chatId: e.Message.Chat,
				   text: "Trying *all the parameters* of `sendMessage` method.",
				   replyMarkup: rkm
				);
			}
		}
	}
}