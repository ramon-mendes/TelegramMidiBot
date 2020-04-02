using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace TelegramMidiBot
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllersWithViews();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if(env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}
			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");
			});

			StartBot();
		}

		static ITelegramBotClient botClient;

		static void StartBot()
		{
			Task.Run(() =>
			{
				var botClient = new TelegramBotClient("925635831:AAEkK38Samii6HNYUAeS3EfJZrk81qlG6Ks");
				var me = botClient.GetMeAsync().Result;
				Console.WriteLine(
				  $"Hello, World! I am user {me.Id} and my name is {me.FirstName}."
				);

				botClient.OnMessage += Bot_OnMessage;
				botClient.StartReceiving();
				Thread.Sleep(int.MaxValue);
			});
		}

		static async void Bot_OnMessage(object sender, MessageEventArgs e)
		{
			if(e.Message.Text != null)
			{
				Console.WriteLine($"Received a text message in chat {e.Message.Chat.Id}.");

				await botClient.SendTextMessageAsync(
				  chatId: e.Message.Chat,
				  text: "You said:\n" + e.Message.Text
				);
			}
		}
	}
}