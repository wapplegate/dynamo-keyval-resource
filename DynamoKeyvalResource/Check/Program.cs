using System.Text.Json;
using Amazon;
using Common;

try
{
    Console.Error.WriteLine("Executing the check...");

    var standardInputPayload = Console.ReadLine();

    //Console.Error.WriteLine(standardInputPayload);

    ArgumentException.ThrowIfNullOrEmpty(standardInputPayload);

    var concoursePayload = JsonSerializer.Deserialize(standardInputPayload, SourceGenerationContext.Default.ConcoursePayload);

    ArgumentNullException.ThrowIfNull(concoursePayload);
    ArgumentNullException.ThrowIfNull(concoursePayload.Source);

    ArgumentException.ThrowIfNullOrEmpty(concoursePayload.Source.AccessKey);
    ArgumentException.ThrowIfNullOrEmpty(concoursePayload.Source.SecretKey);
    ArgumentException.ThrowIfNullOrEmpty(concoursePayload.Source.TableName);
    ArgumentException.ThrowIfNullOrEmpty(concoursePayload.Source.PartitionKey);
    ArgumentException.ThrowIfNullOrEmpty(concoursePayload.Source.Key);

    // Console.Error.WriteLine($"{concoursePayload.Source.TableName}");
    // Console.Error.WriteLine($"{concoursePayload.Source.AppName}");
    // Console.Error.WriteLine($"{concoursePayload.Source.PartitionKey}");
    // Console.Error.WriteLine($"{concoursePayload.Source.AccessKey}");
    // Console.Error.WriteLine($"{concoursePayload.Source.SecretKey}");

    var accessKey = concoursePayload.Source.AccessKey;
    var secretKey = concoursePayload.Source.SecretKey;

    var region = RegionEndpoint.GetBySystemName(concoursePayload.Source.Region);

    var repository = new Repository(accessKey, secretKey, region);

    var table        = concoursePayload.Source.TableName;
    var partitionKey = concoursePayload.Source.PartitionKey;
    var app          = concoursePayload.Source.Key;

    var values = await repository.GetAll(table, partitionKey, app);

    var output = JsonSerializer.Serialize(values, SourceGenerationContext.Default.ListReturnPayload);

    Console.Error.WriteLine("Check completed successfully...");

    Console.WriteLine(output);
}
catch (Exception exception)
{
    Console.Error.WriteLine(exception.ToString());
}