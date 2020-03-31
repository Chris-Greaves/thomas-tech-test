using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

            for (int i = 0; i < testData.Length; i++)
            {
                var candidate = context.Candidates.Add(new Candidate
                {
                    Id = testData[i].Id,
                    RoleId = testData[i].RoleId,
                    Firstname = testData[i].Firstname,
                    Lastname = testData[i].Lastname,
                });

                context.Assessments.Add(new Assessment
                {
                    AssessmentType = AssessmentType.Behaviour,
                    SentOn = testData[i].BehaviourAssessment.SentOn,
                    CompletedOn = testData[i].BehaviourAssessment.CompletedOn,
                    WorkingStrengths = testData[i].BehaviourAssessment.WorkingStrengths,
                    Candidate = candidate.Entity
                });

                context.Assessments.Add(new Assessment
                {
                    AssessmentType = AssessmentType.Aptitude,
                    SentOn = testData[i].AptitudeAssessment.SentOn,
                    CompletedOn = testData[i].AptitudeAssessment.CompletedOn,
                    TrainabilityIndex = testData[i].AptitudeAssessment.TrainabilityIndex,
                    Candidate = candidate.Entity
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
            return DateTime.ParseExact(reader.GetString(), "dd/MM/yyyy", null);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToUniversalTime().ToString());
        }
    }
}
