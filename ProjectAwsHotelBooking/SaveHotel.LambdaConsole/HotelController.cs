using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using System.Text.Json;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.S3;
using Amazon.S3.Model;
using HttpMultipartParser;
using SaveHotel.LambdaConsole.Models;

[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]

public class HotelController
{
    public async Task<APIGatewayProxyResponse> SaveHotel(APIGatewayProxyRequest request, ILambdaContext context)
    {
        var response = new APIGatewayProxyResponse()
        {
            Headers = new Dictionary<string, string>(),
            StatusCode = 200
        };

        response.Headers.Add("Access-Control-Allow-Origin", "*");
        response.Headers.Add("Access-Control-Allow-Headers", "*");
        response.Headers.Add("Access-Control-Allow-Methods", "OPTIONS, POST");

        var bodyContent = request.IsBase64Encoded ? Convert.FromBase64String(request.Body) : Encoding.UTF8.GetBytes(request.Body);

        using (var stream = new MemoryStream(bodyContent))
        {
            var formData = MultipartFormDataParser.Parse(stream);

            var name = formData.GetParameterValue("name");
            var rating = formData.GetParameterValue("rating");
            var city = formData.GetParameterValue("city");
            var price = formData.GetParameterValue("price");

            var file = formData.Files.FirstOrDefault();
            var fileName = file.FileName;

            var userId = formData.GetParameterValue("userId");
            var idToken = formData.GetParameterValue("idToken");

            // var token = new JwtSecurityToken(idToken);
            // var group = token.Claims.FirstOrDefault(x => x.Type == "cognito:groups");

            // if (group == null || group.Value != "Admin")
            // {
            //     response.StatusCode = (int)HttpStatusCode.Unauthorized;
            //     response.Body = JsonSerializer.Serialize(new { Error = "Must be administration group!" });
            // }

            var region = Environment.GetEnvironmentVariable("AWS_REGION");
            var bucketName = Environment.GetEnvironmentVariable("bucketName");

            var s3client = new AmazonS3Client(RegionEndpoint.GetBySystemName(region));
            var dbClient = new AmazonDynamoDBClient(RegionEndpoint.GetBySystemName(region));

            await s3client.PutObjectAsync(new PutObjectRequest()
            {
                BucketName = bucketName,
                Key = fileName,
                InputStream = stream,
                AutoCloseStream = true
            });

            var hotel = new Hotel()
            {
                UserId = userId,
                Id = Guid.NewGuid().ToString(),
                Name = name,
                Price = int.Parse(price),
                Rating = int.Parse(rating),
                CityName = city,
                FileName = fileName
            };

            using (var dbContext = new DynamoDBContext(dbClient))
            {
                await dbContext.SaveAsync(hotel);
            };
        }

        Console.WriteLine("Hotel saved successfully!");

        return response;
    }
}