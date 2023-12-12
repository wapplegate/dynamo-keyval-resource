using System.Text.Json;
using Common;

try
{
    Console.Error.WriteLine("Executing the in...");

    // The file destination is passed in as the first argument:
    var destination = args[0];

    // Read in the payload sent in from concourse through standard output:
    var standardInputPayload = Console.ReadLine();

    //Console.Error.WriteLine(standardInputPayload);

    ArgumentException.ThrowIfNullOrEmpty(standardInputPayload);

    var concoursePayload = JsonSerializer.Deserialize(standardInputPayload, SourceGenerationContext.Default.ConcoursePayload);

    ArgumentNullException.ThrowIfNull(concoursePayload);
    ArgumentNullException.ThrowIfNull(concoursePayload.Version);

    if (!Directory.Exists(destination))
    {
        Directory.CreateDirectory(destination);
    }

    var filePath = Path.Combine(destination, "version.txt");

    using (var writer = new StreamWriter(filePath))
    {
        writer.WriteLine(concoursePayload.Version.Value);
    }

    var output = $"{{\"version\": {{ \"value\": \"{concoursePayload.Version.Value}\", \"created\":\"{concoursePayload.Version.Created}\"}}}}";

    Console.WriteLine(output);
}
catch (Exception exception)
{
    Console.Error.WriteLine($"{exception.Message}");
}