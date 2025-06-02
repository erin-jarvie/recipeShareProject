using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace RecipeApiBenchmark
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            using var httpClient = new HttpClient();

            string url = "http://localhost:5102/api/recipes"; 
            int iterations = 500;
            long totalTime = 0;

            Console.WriteLine($"Sending {iterations} sequential GET requests to {url}...");

            for (int i = 1; i <= iterations; i++)
            {
                var sw = Stopwatch.StartNew();
                var response = await httpClient.GetAsync(url);
                sw.Stop();

                if (!response.IsSuccessStatusCode)
                    Console.WriteLine($"Request {i} failed: {response.StatusCode}");

                totalTime += sw.ElapsedMilliseconds;
            }

            Console.WriteLine($"\nAverage latency over {iterations} requests: {totalTime / (double)iterations:N2} ms");
        }
    }
}
