using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Thomas.TechTest.Data;

namespace Thomas.TechTest.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            //using (var scope = host.Services.CreateScope())
            //{
            //    var context = scope.ServiceProvider.GetService<CandidateDbContext>();
            //    AddTestDataToDatabase(context);
            //}
            host.Run();
        }

        private static void AddTestDataToDatabase(CandidateDbContext context)
        {
            var jsonString = File.ReadAllText("test-database.json");
            var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            jsonOptions.Converters.Add(new DateTimeConverter());
            var testData = JsonSerializer.Deserialize<Models.Candidate[]>(jsonString, jsonOptions);

            foreach (var testCandidate in testData)
            {
                context.Candidates.Add(new Candidate
                {
                    Id = testCandidate.Id,
                    Firstname = testCandidate.Firstname,
                    Surname = testCandidate.Surname,
                    Assessments = new []
                    {
                        new Assessment
                        {
                            AssessmentType = AssessmentType.Aptitude,
                            AssignedCandidateId = testCandidate.Id,
                            SentOn = testCandidate.Assessments[0].Aptitude.SentOn,
                            CompletedOn = testCandidate.Assessments[0].Aptitude.CompletedOn,
                            TrainabilityIndex = testCandidate.Assessments[0].Aptitude.TrainabilityIndex
                        },
                        new Assessment
                        {
                            AssessmentType = AssessmentType.Behaviour,
                            AssignedCandidateId = testCandidate.Id,
                            SentOn = testCandidate.Assessments[0].Behaviour.SentOn,
                            CompletedOn = testCandidate.Assessments[0].Behaviour.CompletedOn,
                            TrainabilityIndex = testCandidate.Assessments[0].Behaviour.TrainabilityIndex
                        },
                    }
                });
            }
            context.SaveChanges();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
    public class DateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.ParseExact(reader.GetString(),"dd/MM/yyyy", null);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToUniversalTime().ToString());
        }
    }
}
