using Nest;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/search", async (string? city, int? rating) =>
{
    var host = "host";
    var username = "userName";
    var password = "password";
    var indexName = "indexName";

    var connSettings = new ConnectionSettings(new Uri(host));
    connSettings.BasicAuthentication(username, password);
    connSettings.DefaultIndex(indexName);
    connSettings.DefaultMappingFor<Candidate>(n => n.IdProperty(p => p.Id));

    var esClient = new ElasticClient(connSettings);
});

app.Run();