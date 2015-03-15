using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Dynamic;

namespace GitHubBrowser
{
    static class Program
    {
        // You'll need to put your own OAuth token here
        // It needs to have repo deletion capability
                                        // This Token auto populated?
        private const string TOKEN = "b8188687c21cb076f09eddb15b86e84227359655";

        // You'll need to put your own GitHub user name here
        private const string USER_NAME = "josephzachary";

        // You'll need to put your own login name here
        private const string EMAIL = "zachary@cs.utah.edu";

        // You'll need to put one of your public REPOs here
        private const string PUBLIC_REPO = "repo1";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            GetDemo();
            Console.ReadLine();
            GetAllDemo();
            Console.ReadLine();
            GetWithParamsDemo();
            Console.ReadLine();
            PostDemo();
            Console.ReadLine();
            DeleteDemo();
            Console.ReadLine();
        }

        /// <summary>
        /// Creates a generic client for communicating with GitHub
        /// </summary>
        public static HttpClient CreateClient()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://api.github.com/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
            client.DefaultRequestHeaders.UserAgent.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", Uri.EscapeDataString(EMAIL));
            client.DefaultRequestHeaders.Add("Authorization", "token " + TOKEN);
            return client;
        }

        /// <summary>
        /// Prints out the names of the organizations to which the user belongs
        /// </summary>
        public static async void GetDemo()
        {
            using (HttpClient client = CreateClient())
            {
                HttpResponseMessage response = await client.GetAsync("/user/orgs");
                if (response.IsSuccessStatusCode)
                {
                    String result = await response.Content.ReadAsStringAsync();
                    dynamic orgs = JsonConvert.DeserializeObject(result);
                    foreach (dynamic org in orgs)
                    {
                        Console.WriteLine(org.login);
                    }
                }
                else
                {
                    Console.WriteLine("Error: " + response.StatusCode);
                    Console.WriteLine(response.ReasonPhrase);
                }
            }
        }

        /// <summary>
        /// Prints out (some of) the users of GitHub, along with the link to use to get more.
        /// </summary>
        public static async void GetAllDemo()
        {
            using (HttpClient client = CreateClient())
            {
                HttpResponseMessage response = await client.GetAsync("/users");
                if (response.IsSuccessStatusCode)
                {
                    String result = await response.Content.ReadAsStringAsync();
                    dynamic users = JsonConvert.DeserializeObject(result);
                    foreach (dynamic user in users)
                    {
                        Console.WriteLine(user.login);
                    }
                    foreach (String link in response.Headers.GetValues("Link"))
                    {
                        Console.WriteLine(link);
                    }
                }
                else
                {
                    Console.WriteLine("Error: " + response.StatusCode);
                    Console.WriteLine(response.ReasonPhrase);
                }
            }
        }

        /// <summary>
        /// Prints out all of the commits on the master branch
        /// </summary>
        public static async void GetWithParamsDemo()
        {
            using (HttpClient client = CreateClient())
            {
                String url = String.Format("/repos/{0}/{1}/commits?sha={2}", USER_NAME, PUBLIC_REPO, Uri.EscapeDataString("master"));
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    String result = await response.Content.ReadAsStringAsync();
                    dynamic commits = JsonConvert.DeserializeObject(result);
                    Console.WriteLine(commits);
                }
                else
                {
                    Console.WriteLine("Error: " + response.StatusCode);
                    Console.WriteLine(response.ReasonPhrase);
                }
            }
        }

        /// <summary>
        /// Creates a new public repository
        /// </summary>
        public static async void PostDemo()
        {
            using (HttpClient client = CreateClient())
            {
                dynamic data = new ExpandoObject();
                data.name = "TestRepo";
                StringContent content = new StringContent(JsonConvert.SerializeObject(data));
                HttpResponseMessage response = await client.PostAsync("/user/repos", content);
                if (response.IsSuccessStatusCode)
                {
                    String result = await response.Content.ReadAsStringAsync();
                    dynamic newRepo = JsonConvert.DeserializeObject(result);
                    Console.WriteLine(newRepo);
                }
                else
                {
                    Console.WriteLine("Error: " + response.StatusCode);
                    Console.WriteLine(response.ReasonPhrase);
                }
            }
        }

        /// <summary>
        /// Deletes an existing public repository
        /// </summary>
        public static async void DeleteDemo()
        {
            using (HttpClient client = CreateClient())
            {
                String url = String.Format("/repos/{0}/TestRepo", USER_NAME);
                Console.WriteLine(url);
                HttpResponseMessage response = await client.DeleteAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Deleted");
                }
                else
                {
                    Console.WriteLine("Error: " + response.StatusCode);
                    Console.WriteLine(response.ReasonPhrase);
                }
            }
        }
    }
}
