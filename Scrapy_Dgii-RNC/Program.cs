using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Scrapy_Dgii_RNC
{
    class Program
    {

        public static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            if (args.Length != 1)
            {
                Console.WriteLine("Por favor, proporciona el número de RNC como argumento.");
                return;
            }

            string RNC = args[0].Trim();
            RNC = Regex.Replace(RNC, "[^0-9]", "");

            // Verificar que la longitud sea de 9 o 11 dígitos (dependiendo del formato de RNC)
            if (RNC.Length != 9 && RNC.Length != 11)
            {
                Console.WriteLine("El RNC no es válido (longitud incorrecta).");
                return;
            }

            string Data = await RunPuppeteer(RNC);
            List<Clases.Empresa> Empresas = JsonConvert.DeserializeObject<List<Clases.Empresa>>(Data);
            string jsonResult = JsonConvert.SerializeObject(Empresas);
            Console.WriteLine(jsonResult);
        }

        static async Task<string> RunPuppeteer(string RNC = "")
        {
            var browserFetcher = new BrowserFetcher(new BrowserFetcherOptions());
            await browserFetcher.DownloadAsync();

            using (var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true}))
            using (var page = await browser.NewPageAsync())
            {
                // https://www.dgii.gov.do/app/WebApps/ConsultasWeb/consultas/rnc.aspx

                string Prefix = "#cphMain_"; // "#ctl00_cphMain_"

                string Url = "https://dgii.gov.do/app/WebApps/ConsultasWeb2/ConsultasWeb/consultas/rnc.aspx";
                await page.GoToAsync(Url);
                await page.TypeAsync($"{Prefix}txtRNCCedula", RNC);
                await page.ClickAsync($"{Prefix}btnBuscarPorRNC");

                // Esperar unos segundos (opcional, puedes ajustar según tus necesidades)
                await page.WaitForTimeoutAsync(1000);
                var timeoutTask = Task.Delay(2000);
                var waitForSelectorTask = page.WaitForSelectorAsync($"{Prefix}dvDatosContribuyentes");
                var completedTask = await Task.WhenAny(waitForSelectorTask, timeoutTask);

                if (completedTask == timeoutTask)
                {
                    return "[]";
                }

                await waitForSelectorTask;
                var tbodyHandle = await page.QuerySelectorAsync($"{Prefix}dvDatosContribuyentes tbody");

                if (tbodyHandle != null)
                {
                    string callback = @"(element) =>{
                        let Data = [];
                        let Object2 = {};
                        let Body = Array.from(element.children);
                        let Title = ['RNC', 'Razon_Social', 'Nombre', 'Categoria', 'Pagos', 'Estado', 'Actividad', 'Administracion'];

                        Body.map((A, i) => {
                            Object.defineProperty(Object2, Title[i],{ value: A.children[1].innerText.trim(), enumerable: true, configurable: true, writable: true});
                        });

                        Data.push(Object2);
                        return JSON.stringify(Data);
                    }";

                    var tbodyContent = await page.EvaluateFunctionAsync<string>(callback, tbodyHandle);
                    byte[] byteArray = Encoding.UTF8.GetBytes(tbodyContent);
                    return Encoding.UTF8.GetString(byteArray); ;
                }
                else
                {
                    return "[]";
                }
            }
        }
    }
}
