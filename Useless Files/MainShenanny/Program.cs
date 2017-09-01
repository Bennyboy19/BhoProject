using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;

namespace MainShenanny
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpClient cons = new HttpClient();
            cons.BaseAddress = new Uri("http://localhost:52566/");
            cons.DefaultRequestHeaders.Accept.Clear();
            cons.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            getMyApi(cons).Wait();
        }

        static async Task getMyApi(HttpClient client)
        {
            try
            {
                using (client)
                {
                    HttpResponseMessage res = await client.GetAsync("api/Tables/1");
                    res.EnsureSuccessStatusCode();
                    if (res.IsSuccessStatusCode)
                    {
                        LoginInfo lI = await res.Content.ReadAsAsync<LoginInfo>();

                        Console.WriteLine(lI.password);
                        Console.ReadLine();
                    }
                }
            }
            catch (WebException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e.InnerException.Message);
                Console.ReadLine();
            }
        
        }
    }
}
