using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using SSOService.Models;
using System.Net;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;

namespace HelloOpenShift.Controllers
{
    public class TestResults
    {
        public Boolean is_success { get; set; }
        public string error_msg { get; set; }
        public string sso_token { get; set; }
        public string user_data { get; set; }
    }

    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        private static string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        private const string S_SSO_CookieName = "iPlanetDirectoryPro";

        private IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SampleDataController(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = config;
            _httpContextAccessor = httpContextAccessor;
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

        [HttpGet("[action]")]
        public TestResults GetSSOToken()
        {
            TestResults rval = new TestResults() { is_success = false };
            try
            {
                string val = _httpContextAccessor.HttpContext.Request.Cookies[S_SSO_CookieName];
                if (!String.IsNullOrEmpty(val))
                {
                    rval.sso_token = val;
                    rval.is_success = true;
                }
                else
                {
                    rval.error_msg = "Could not get the cookie value from request object.";
                }
            }
            catch(Exception ex)
            {
                rval.error_msg = "Exception, details: " + ex.ToString();
            }
            return rval;
        }

        [HttpPost("[action]")]
        public async Task<TestResults> CallSSOServiceAsync([FromBody] VerifyTokenInputParams input)
        {
            TestResults rval = new TestResults() { is_success = false };
            try
            {
                if (String.IsNullOrEmpty(input.sso_token))
                    throw new Exception("Invalid token parameter");

                string url = _configuration["SSOServiceURL"];
                if (String.IsNullOrEmpty(url))
                    throw new Exception("URL not found in the conifguration variables.");

                VerifyTokenInputParams inParams = new VerifyTokenInputParams()
                {
                    environment = String.IsNullOrEmpty(input.sso_token) ? "Prod" : input.sso_token,
                    portal = "JOE",
                    sso_token = input.sso_token
                };

                HttpClient client = new HttpClient();
                StringContent strContent = new StringContent(JsonConvert.SerializeObject(inParams), UnicodeEncoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(url, strContent);

                HttpContent content = response.Content;

                rval.user_data = response.Content.ReadAsStringAsync().Result;
                rval.is_success = true;

                //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_configuration["SSOServiceURL"]);
                //request.Method = "POST";
                //request.ContentType = "application/json";

                //using (Stream webStream = request.GetRequestStream())
                //using (StreamWriter requestWriter = new StreamWriter(webStream, System.Text.Encoding.ASCII))
                //{
                //    requestWriter.Write()
                //}

                //rval.user_data = "testing user fetch";

            }
            catch (Exception ex)
            {
                rval.error_msg = "Exception, details: " + ex.ToString();
            }

            return rval;
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
                        rval = _configuration["Pens:Division"]; // specify a nested value
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
