using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BulkBatcher.Models;
using CsvHelper;
using CsvHelper.Configuration;

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
        // All of the settings below come from the .env file next to this project.
        // Copy .env.example to .env and fill in your own values - .env is git-ignored
        // so credentials never end up in source control.
        // These are properties rather than fields on purpose: a field initializer runs
        // before Main, so a missing or malformed setting would surface as an opaque
        // TypeInitializationException instead of the readable message Main logs.
        private static int BatchSize => Env.GetInt("BATCH_SIZE", 1); // This is using the batch upload endpoint, but please keep this to 1 to avoid overloading the API.
        private static string JsonFileName => Env.GetString("JSON_FILE_NAME", "testData.json"); // The JSON file containing business data - You can change this to your actual file name
        private static string CsvFileName => Env.GetString("CSV_FILE_NAME", "testData.csv"); // The CSV file containing business data - You can change this to your actual file name
        private static string AuthUrl => Env.GetString("AUTH_URL", "https://api.joinworth.com/auth/api/v1/admin/sign-in"); // URL for authentication endpoint
        private static string CaseUrl => Env.GetString("CASE_URL", "https://api.joinworth.com/case/api/v1/businesses/customers"); // Base URL for the API endpoint
        private static string CustomerId => Env.GetRequiredString("CUSTOMER_ID"); // Your customer ID for the API
        private static string Email => Env.GetRequiredString("EMAIL"); // Your email for authentication
        private static string Password => Env.GetRequiredString("PASSWORD"); // Your password for authentication
        private static bool isCSV => Env.GetBool("IS_CSV", true); // Set to true if you are using CSV file upload, false if using JSON file upload
        private static int delayInterval => Env.GetInt("DELAY_INTERVAL_MS", 10000); // Delay interval between batch requests in milliseconds. 
        private static string errorLogFileName => Env.GetString("ERROR_LOG_FILE_NAME", "error_log.txt"); // File to log errors
        private static int TokenLifetimeMinutes => Env.GetInt("TOKEN_LIFETIME_MINUTES", 60); // Auth token lifetime in minutes
        private static int TokenRefreshBufferMinutes => Env.GetInt("TOKEN_REFRESH_BUFFER_MINUTES", 10); // Refresh token this many minutes before expiry

        private static string _currentToken = string.Empty;
        private static DateTime _tokenAcquiredAt = DateTime.MinValue;
        private static readonly string _logFileName = $"upload_{DateTime.Now:yyyyMMdd_HHmmss}.log";

        private static void Log(string message)
        {
            Console.WriteLine(message);
            File.AppendAllText(_logFileName, message + Environment.NewLine);
        }

        public static async Task Main(string[] args)
        {
            try
            {
                string apiPath = $"{CaseUrl}/{CustomerId}/bulk/process?async=true";
                await SignInAsync(Email, Password);
                Log("Successfully authenticated.");
                string jsonContent = "";

                if (!isCSV)
                {
                    string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), JsonFileName);
                    jsonContent = await File.ReadAllTextAsync(jsonFilePath);
                }
                else
                {
                    // Convert CSV to JSON
                    string csvFilePath = Path.Combine(Directory.GetCurrentDirectory(), CsvFileName);
                    jsonContent = ConvertCsvToJson(csvFilePath);



                }

                // Deserialize JSON into list of CreateBusinessPayload objects
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new EmptyStringToNullConverter() },
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,

                };

                var businesses = JsonSerializer.Deserialize<List<CreateBusinessPayload>>(jsonContent, options);

                if (businesses != null)
                {
                    foreach (var business in businesses)
                        business.Owner1Mobile = EnsureLeadingPlusForInternationalMobile(business.Owner1Mobile);
                }

                Log($"Successfully loaded {businesses?.Count ?? 0} businesses from JSON file.");

                // Display some data to verify loading worked correctly
                // if (businesses != null && businesses.Count > 0)
                //{
                //    Console.WriteLine("\nbusinesses:");
                //    foreach (var business in businesses.Take(2))
                //    {
                //        Console.WriteLine($"- {business.Name} ({business.ExternalId})");
                //        Console.WriteLine($"  Contact: {business.Mobile}");
                //        Console.WriteLine($"  Address: {business.AddressLine1}, {business.AddressCity}, {business.AddressState} {business.AddressPostalCode}");
                //        Console.WriteLine($"  Owner: {business.Owner1FirstName} {business.Owner1LastName}");
                //        Console.WriteLine();
                //    }
                //}

                // Batch businesses into groups of BatchSize
                if (businesses != null)
                {
                    var batches = CreateBatches(businesses, BatchSize);
                    Log($"Created {batches.Count} batches of size {BatchSize}");

                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));


                        int successCount = 0;
                        int failureCount = 0;
                        // Process each batch sequentially
                        for (int i = 0; i < batches.Count; i++)
                        {
                            // Refresh token if it is close to expiring
                            await EnsureValidTokenAsync();
                            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _currentToken);
                            var batch = batches[i];
                            string serializedBatch = JsonSerializer.Serialize(batch, options);
                            var content = new StringContent(serializedBatch, System.Text.Encoding.UTF8, "application/json");
                            var requestBody = await content.ReadAsStringAsync();

                            Log($"\nRequest Body for Batch {i + 1}:\n{requestBody}\n");

                            Log($"Posting batch {i + 1}/{batches.Count} containing {batch.Count} businesses to API...");

                            // Display businesses in this batch
                            foreach (var business in batch)
                            {
                                Log($"  - {business.Name} ({business.ExternalId})");
                            }

                            var response = await httpClient.PostAsync(apiPath, content);
                            if (response.IsSuccessStatusCode)
                            {
                                string responseContent = await response.Content.ReadAsStringAsync();
                                Log($"Success! Batch {i + 1} response: {responseContent}");
                                successCount++;
                            }
                            else
                            {
                                Log($"Failed to post batch {i + 1}. Status code: {response.StatusCode}");
                                Log($"Error response: {await response.Content.ReadAsStringAsync()}");
                                failureCount++;
                                // Log error to file
                                foreach (var business in batch)
                                {
                                    string errorLog = $"Batch {i + 1} - Business: {business.Name} ({business.ExternalId}) - Status code: {response.StatusCode}\n";
                                    await File.AppendAllTextAsync(errorLogFileName, errorLog);
                                }
                            }

                            // Add a small delay between requests to avoid spamming the API
                            if (i < batches.Count - 1)
                            {
                                await Task.Delay(delayInterval);
                            }


                        }

                        Log($"\nBatch processing complete. Successful batches: {successCount}, Failed batches: {failureCount}");
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Error: {ex.Message}");
                Log(ex.StackTrace ?? string.Empty);
            }
        }

        /// <summary>
        /// Splits a list into batches of specified size
        /// </summary>
        /// <summary>
        /// Ensures owner mobile values that include a country code use a leading + (E.164-style).
        /// If the first non-whitespace character is not +, one is prepended.
        /// </summary>
        private static string? EnsureLeadingPlusForInternationalMobile(string? owner1Mobile)
        {
            if (owner1Mobile is null)
                return null;
            var trimmed = owner1Mobile.Trim();
            if (trimmed.Length == 0)
                return null;
            if (trimmed[0] == '+')
                return trimmed;
            return '+' + trimmed;
        }

        private static List<List<CreateBusinessPayload>> CreateBatches(List<CreateBusinessPayload> items, int batchSize)
        {
            var batches = new List<List<CreateBusinessPayload>>();

            for (int i = 0; i < items.Count; i += batchSize)
            {
                batches.Add([.. items.Skip(i).Take(batchSize)]);
            }

            return batches;
        }

        public static string ConvertCsvToJson(string csvFilePath)
        {
            using var reader = new StreamReader(csvFilePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = csv.GetRecords<dynamic>().ToList();

            return JsonSerializer.Serialize(records, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }

        /// <summary>
        /// Checks if the current token is close to expiring and re-authenticates if needed.
        /// Refreshes the token when there are fewer than TokenRefreshBufferMinutes remaining.
        /// </summary>
        private static async Task EnsureValidTokenAsync()
        {
            var elapsed = DateTime.UtcNow - _tokenAcquiredAt;
            var remainingMinutes = TokenLifetimeMinutes - elapsed.TotalMinutes;

            if (remainingMinutes <= TokenRefreshBufferMinutes)
            {
                Log($"Token expiring in {remainingMinutes:F1} minutes. Refreshing...");
                await SignInAsync(Email, Password);
                Log("Token refreshed successfully.");
            }
        }

        public static async Task SignInAsync(string email, string password)
        {
            try
            {
                var credentials = new
                {
                    email,
                    password
                };

                using var httpClient = new HttpClient();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };


                string serializedCredentials = JsonSerializer.Serialize(credentials, options);
                var content = new StringContent(serializedCredentials, System.Text.Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(AuthUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseContent, options);
                    _currentToken = authResponse?.Data?.IdToken ?? throw new Exception("Token not found in authentication response.");
                    _tokenAcquiredAt = DateTime.UtcNow;
                }
                else
                {
                    throw new Exception("Authentication failed with status code: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Log($"Authentication failed: {ex.Message}");
                throw;
            }
        }
    }
}