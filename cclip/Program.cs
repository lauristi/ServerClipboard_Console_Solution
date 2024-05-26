using cclip.Model;
using EncryptionLib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((context, services) =>
{
    services.AddSingleton<ICrypto, CryptoClass>();
});

var host = builder.Build();

using (var serviceScope = host.Services.CreateScope())
{
    //injetando serviço de criptografia
    var services = serviceScope.ServiceProvider;
    var crypto = services.GetRequiredService<ICrypto>();

    //Acessando o endpoit da ServerClipboard_API
    using (var client = new HttpClient())
    {
        client.BaseAddress = new Uri("http://localhost:5020/"); // Base URL da sua Web API
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        //Apenas para Debug Local
        //Thread.Sleep(15000);

        var getResponse = await client.GetAsync("api/clipboard/get");
        if (getResponse.IsSuccessStatusCode)
        {
            var content = await getResponse.Content.ReadAsStringAsync();
            var json = JsonSerializer.Deserialize<ClipboardResponse>(content);

            Console.WriteLine(json.Clipboard);
        }
        else
        {
            Console.WriteLine("ServerClipboard_API not found!");
        }
    }
}