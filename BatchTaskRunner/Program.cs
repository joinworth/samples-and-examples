
namespace BatchTaskRunner
{

    class Program
    {
        public static async Task Main(string[] args)
        {

            var token = "your-token-here";
            var platformName = "TRULIOO_PSC || or other platform name";
            var platformTaskCode = "fetch_watchlist_hits || or other task code";

            // swap this call with RerunTaskForBusiness if you want to use that function instead. 
            // I don't know which tasks are compatible with which functions, so you'll have to investigate the source code or do some trial and error to figure out which one you need.
            await GenerateAndExecuteTaskForBusiness(token, BusinessIds, platformName, platformTaskCode);
        }


        // only some integrations will be run with this function's endpoint. Dunno why.
        public static async Task RerunTaskForBusiness(string token, string[] businessIds, string platformName, string taskCode)
        {
            foreach (var businessId in businessIds)
            {
                try
                {
                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Post, $"https://api.joinworth.com/integration/api/v1/businesses/{businessId}/integrations/rerun");
                    request.Headers.Add("Authorization",
                        "Bearer " + token);
                    var content = new StringContent($"{{\r\n  \"platform_codes\": [\"{platformName}\"],\r\n  \"task_codes\": [\"{taskCode}\"]\r\n}}", null, "application/json");
                    request.Content = content;
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    Console.WriteLine($"Successfully updated task {platformName} business with ID: {businessId}");
                    Console.WriteLine(await response.Content.ReadAsStringAsync());

                    // wait for 2 seconds before processing the next business ID to avoid hitting API "rate limits"
                    await Task.Delay(2000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to execute {platformName} task for business ID: {businessId}. Details: {ex.Message}");
                }

            }
        }


        // this also only covers some tasks for #reasonsIGuess. Best bet is to investigate the source to determine which you should use.
        public static async Task GenerateAndExecuteTaskForBusiness(string token, string[] businessIds, string platformName, string taskCode)
        {
            foreach (var businessId in businessIds)
            {
                try
                {
                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Post, $"https://api.joinworth.com/integration/api/v1/tasks/business/{businessId}/platform/{platformName}/{taskCode}");
                    request.Headers.Add("Authorization", "Bearer " + token);
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    Console.WriteLine(await response.Content.ReadAsStringAsync());

                    // wait for 2 seconds before processing the next business ID to avoid hitting API "rate limits"
                    await Task.Delay(2000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to execute {platformName} task for business ID: {businessId}. Details: {ex.Message}");
                }

            }
        }

        public static string[] BusinessIds = ["array of business ids go here"];
    }
}