using System.Text;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using HttpMultipartParser;

[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]

public class HotelController
{
    public APIGatewayProxyResponse SaveHotel(APIGatewayProxyRequest request, ILambdaContext context)
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
            var token = formData.GetParameterValue("token");
        }

        Console.WriteLine("Hotel saved successfully!");

        return response;
    }
}