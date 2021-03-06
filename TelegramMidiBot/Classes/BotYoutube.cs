﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;
using YoutubeExplode;
using YoutubeExplode.Converter;
using YoutubeExplode.Videos.Streams;

namespace TelegramMidiBot.Classes
{
	public static class BotYoutube
	{
		public static void StartBot()
		{
			var t = new Thread(async () =>
			{
				await GetAudioURL("https://www.youtube.com/watch?v=do3d6mfUsWU");

				var botClient = Bot.GetClient();

				var me = botClient.GetMeAsync().Result;
				Console.WriteLine(
				  $"Hello, World! I am user {me.Id} and my name is {me.FirstName}."
				);

				botClient.OnMessage += Bot_OnMessage;
				Thread.Sleep(int.MaxValue);
			});
			t.Name = "Telegram bot";
			t.IsBackground = false;
			t.Start();
		}

		private static async void Bot_OnMessage(object sender, MessageEventArgs e)
		{
			if(e.Message.Text == null)
				return;

			var b1 = e.Message.Text.Contains("youtube.com/");
			var b2 = e.Message.Text.Contains("youtu.be/");
			if(b1 || b2)
			{
				string url = e.Message.Text;
				if(b1)
					url = url.Substring(url.IndexOf("youtube.com/"));
				if(b2)
					url = url.Substring(url.IndexOf("youtu.be/"));

				var botClient = Bot.GetClient();

				await botClient.SendTextMessageAsync(
				   chatId: e.Message.Chat,
				   text: "Getting audio from video..." 
				);

				var audioUrl = await GetAudioURL(url);

				if(audioUrl != null)
				{
					var rkm = new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl("Download video audio", audioUrl));

					await botClient.SendTextMessageAsync(
					   chatId: e.Message.Chat,
					   text: "Done.",
					   replyMarkup: rkm
					);
				}
				else
				{
					await botClient.SendTextMessageAsync(
					   chatId: e.Message.Chat,
					   text: "Failed =("
					);
				}
			}
		}

		private static async Task<string> GetAudioURL(string url)
		{
			var client = new YoutubeClient();

			var video = await client.Videos.GetAsync(url);
			var manifest = await client.Videos.Streams.GetManifestAsync(video.Id);
			var audio = manifest.GetAudioOnly().WithHighestBitrate();

			return audio.Url;

			//var output = Environment.CurrentDirectory.Replace('\\', '/') + "/audio.mp4";
			//await client.DownloadMediaStreamAsync(audio, output);
			//var mediaStreamInfos = new MediaStreamInfo[] { audio };
			//await converter.DownloadAndProcessMediaStreamsAsync(mediaStreamInfos, output, "mp3");
		}
	}
}