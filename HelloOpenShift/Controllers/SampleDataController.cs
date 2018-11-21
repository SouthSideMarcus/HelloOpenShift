using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace HelloOpenShift.Controllers
{
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        private static string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private IConfiguration _configuration;

        public SampleDataController(IConfiguration config)
        {
            _configuration = config;
        }

        [HttpGet("[action]")]
        public IEnumerable<WeatherForecast> WeatherForecasts()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                DateFormatted = DateTime.Now.AddDays(index).ToString("d"),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)],
                //ConfigValue = "Testing"
                ConfigValue = GetConfigValue(index)
            });
        }

        private string GetConfigValue(int index)
        {
            string rval = "somethign happened";
            try
            {
                switch(index)
                {
                    case 1:
                        rval = _configuration["TestKey1"];
                        break;

                    case 2:
                        rval = _configuration["TestKey2"];
                        break;

                    case 3:
                        {
                            IConfigurationSection sec = _configuration.GetSection("Pens");
                            if (sec == null)
                                rval = "sec is null";
                            else
                                rval = sec["StanleyCup"];
                        }
                        break;

                    default:
                        rval = _configuration["Pens:Division"];
                        break;
                }
            }
            catch(Exception ex)
            {
                rval = "Exception getting index " + index + "   \r\nDetails: " + ex.ToString();
            }
            return rval;
        }

        public class WeatherForecast
        {
            public string DateFormatted { get; set; }
            public int TemperatureC { get; set; }
            public string Summary { get; set; }
            public string ConfigValue { get; set; }

            public int TemperatureF
            {
                get
                {
                    return 32 + (int)(TemperatureC / 0.5556);
                }
            }
        }
    }
}
