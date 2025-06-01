using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using BulkBatcher.Models;

namespace BulkBatcher
{
    /// <summary>
    /// NOTE: This is sample code and is not intended for production use.
    /// This program reads a JSON file containing business data,
    /// deserializes it into a list of CreateBusinessPayload objects,
    /// and batches them into smaller groups for API submission
    /// </summary>
    class Program
    {
        private const int BatchSize = 5; // We recommend a batch size of 5 to ensure timely responses from the API
        private const string JsonFileName = "testData.json"; // The JSON file containing business data - You can change this to your actual file name
        private const string ApiBaseUrl = "https://api.joinworth.com/case/api/v1/businesses/customers"; // Base URL for the API endpoint
        private const string CustomerId = "your-customer-id"; // Your customer ID for the API
        private const string AuthToken = "your-auth-token"; // Your authentication token for the API. This is a JWT token or similar that you get from the Auth Endpoint
        public static async Task Main(string[] args)
        {
            try
            {
                string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), JsonFileName);
                string jsonContent = await File.ReadAllTextAsync(jsonFilePath);
                string apiPath = $"{ApiBaseUrl}/{CustomerId}/bulk/process";

                // Deserialize JSON into list of CreateBusinessPayload objects
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var businesses = JsonSerializer.Deserialize<List<CreateBusinessPayload>>(jsonContent, options);

                Console.WriteLine($"Successfully loaded {businesses?.Count ?? 0} businesses from JSON file.");

                // Display some data to verify loading worked correctly
                if (businesses != null && businesses.Count > 0)
                {
                    Console.WriteLine("\nFirst few businesses:");
                    foreach (var business in businesses.Take(2))
                    {
                        Console.WriteLine($"- {business.Name} ({business.ExternalId})");
                        Console.WriteLine($"  Contact: {business.Mobile}");
                        Console.WriteLine($"  Address: {business.AddressLine1}, {business.AddressCity}, {business.AddressState} {business.AddressPostalCode}");
                        Console.WriteLine($"  Owner: {business.Owner1FirstName} {business.Owner1LastName}");
                        Console.WriteLine();
                    }
                }

                // Batch businesses into groups of BatchSize
                if (businesses != null)
                {
                    var batches = CreateBatches(businesses, BatchSize);
                    Console.WriteLine($"Created {batches.Count} batches of size {BatchSize}");

                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AuthToken);
                        httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                        // Process each batch sequentially
                        for (int i = 0; i < batches.Count; i++)
                        {
                            var batch = batches[i];
                            string serializedBatch = JsonSerializer.Serialize(batch, options);
                            var content = new StringContent(serializedBatch, System.Text.Encoding.UTF8, "application/json");

                            Console.WriteLine($"Posting batch {i + 1}/{batches.Count} containing {batch.Count} businesses to API...");

                            // Display businesses in this batch
                            foreach (var business in batch)
                            {
                                Console.WriteLine($"  - {business.Name} ({business.ExternalId})");
                            }

                            var response = await httpClient.PostAsync(apiPath, content);
                            if (response.IsSuccessStatusCode)
                            {
                                string responseContent = await response.Content.ReadAsStringAsync();
                                Console.WriteLine($"Success! Batch {i + 1} response: {responseContent}");
                            }
                            else
                            {
                                Console.WriteLine($"Failed to post batch {i + 1}. Status code: {response.StatusCode}");
                                Console.WriteLine($"Error response: {await response.Content.ReadAsStringAsync()}");
                            }

                            // Add a small delay between requests to avoid spamming the API
                            if (i < batches.Count - 1)
                            {
                                await Task.Delay(500);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        /// <summary>
        /// Splits a list into batches of specified size
        /// </summary>
        private static List<List<CreateBusinessPayload>> CreateBatches(List<CreateBusinessPayload> items, int batchSize)
        {
            var batches = new List<List<CreateBusinessPayload>>();
            
            for (int i = 0; i < items.Count; i += batchSize)
            {
                batches.Add([.. items.Skip(i).Take(batchSize)]);
            }
            
            return batches;
        }
    }
}