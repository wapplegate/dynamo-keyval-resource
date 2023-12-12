using System.Globalization;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;

namespace Common;

public class Repository
{
    private readonly AmazonDynamoDBClient client;

    public Repository(string accessKey, string secretKey, RegionEndpoint region)
    {
        client = new AmazonDynamoDBClient(new BasicAWSCredentials(accessKey, secretKey), region);
    }

    public async Task<List<ReturnPayload>> GetAll(string tableName, string partitionKey, string key)
    {
        var value = await client.QueryAsync(new QueryRequest
        {
            TableName = tableName,
            KeyConditionExpression = $"{partitionKey} = :Id",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":Id", new AttributeValue { S = key }}
            }
        });

        var values = new List<ReturnPayload>();

        foreach (var dictionary in value.Items)
        {
            var payload = new ReturnPayload
            {
                Value = dictionary["value"].S,
                Created = dictionary["created"].S
            };

            values.Add(payload);
        }

        return values.OrderBy(a => a.CreatedDateTimeOffset).ToList();
    }

    public async Task Create(string tableName, string partitionKey, string app, string value, DateTime created)
    {
        var putItemRequest = new PutItemRequest
        {
            TableName = tableName,
            Item = new Dictionary<string, AttributeValue>
            {
                {
                    partitionKey,
                    new AttributeValue
                    {
                        S = app
                    }
                },
                {
                    "created",
                    new AttributeValue
                    {
                        S = created.ToString(CultureInfo.InvariantCulture)
                    }
                },
                {
                    "value",
                    new AttributeValue
                    {
                        S = value
                    }
                }
            }
        };

        await client.PutItemAsync(putItemRequest);
    }
}
