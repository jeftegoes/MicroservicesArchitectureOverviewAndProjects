
using System.Text.Json;
using Amazon.Lambda.SNSEvents;
using static Amazon.Lambda.SNSEvents.SNSEvent;

Environment.SetEnvironmentVariable("host", "https://search-candidates-wz4livytklu6nkhrwmkyqonwcm.sa-east-1.es.amazonaws.com/");
Environment.SetEnvironmentVariable("userName", "elastic");
Environment.SetEnvironmentVariable("password", "ySz6QD*062*z");
Environment.SetEnvironmentVariable("indexName", "indexCandidate");

var candidate = new Candidate()
{
    Name = "Jefté",
    CityName = "Feira",
    Salary = 200,
    Rating = 10,
    Id = "123",
    UserId = "abc",
    CreationDateTime = DateTime.Now
};

var snsEvent = new SNSEvent()
{
    Records = new List<SNSRecord>()
    {
        new SNSEvent.SNSRecord()
        {
            Sns = new SNSMessage()
            {
                MessageId = "100",
                Message = JsonSerializer.Serialize(candidate)
            }
        }
    }
};

var handler = new CandidateCreatedEventHandler();
await handler.Handler(snsEvent);