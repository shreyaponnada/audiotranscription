using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

class Program
{
    private static readonly string apiKey = "OPENAI_API_KEY";
    private static readonly string openAiUrl = "https://api.openai.com/v1/audio/transcriptions";

    static async Task Main()
    {
        Console.Write("Enter the path to the audio file: ");
        string filePath = Console.ReadLine();
        
        if (!File.Exists(filePath))
        {
            Console.WriteLine("File not found!");
            return;
        }
        
        string transcript = await TranscribeAudioAsync(filePath);
        Console.WriteLine("Transcription:");
        Console.WriteLine(transcript);
    }

    private static async Task<string> TranscribeAudioAsync(string filePath)
    {
        using (var client = new HttpClient())
        using (var form = new MultipartFormDataContent())
        using (var fileStream = File.OpenRead(filePath))
        using (var fileContent = new StreamContent(fileStream))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("audio/wav");
            form.Add(fileContent, "file", Path.GetFileName(filePath));
            form.Add(new StringContent("whisper-1"), "model");
            
            HttpResponseMessage response = await client.PostAsync(openAiUrl, form);
            
            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error: {response.StatusCode}, {error}");
            }
            
            return await response.Content.ReadAsStringAsync();
        }
    }
}
