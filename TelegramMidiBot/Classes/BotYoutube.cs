using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;
using YoutubeExplode;
using YoutubeExplode.Converter;
using YoutubeExplode.Models.MediaStreams;

namespace TelegramMidiBot.Classes
{
	public static class BotYoutube
	{
		public static void StartBot()
		{
			var t = new Thread(async () =>
			{
				await ExtractAudio("https://www.youtube.com/watch?v=do3d6mfUsWU");

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
				   text: "Getting audio from video." 
				);

				await ExtractAudio(url);
			}
		}

		private static async Task ExtractAudio(string url)
		{
			var id = YoutubeClient.ParseVideoId(url);
			var client = new YoutubeClient();
			var converter = new YoutubeConverter(client);

			var video = await client.GetVideoAsync(id);
			var mediaStreamInfoSet = await client.GetVideoMediaStreamInfosAsync(id);
			var audioStreamInfo = mediaStreamInfoSet.Audio.WithHighestBitrate();
			var audio = mediaStreamInfoSet.Audio.Where(a => a.Container == Container.Mp4).OrderBy(a => a.Bitrate).FirstOrDefault();

			var output = Environment.CurrentDirectory.Replace('\\', '/') + "/audio.mp4";
			await client.DownloadMediaStreamAsync(audio, output);
			//var mediaStreamInfos = new MediaStreamInfo[] { audio };
			//await converter.DownloadAndProcessMediaStreamsAsync(mediaStreamInfos, output, "mp3");
		}
	}
}