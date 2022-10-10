using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace InputDataReader.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TextReaderController : ControllerBase
    {

        [HttpPost("upload-file")]
        public async Task<IActionResult> UploadFileAsync(IFormFile formFile)
        {


            if (formFile.Length > 0)
            {
                var filePath = Path.GetTempFileName();
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await formFile.CopyToAsync(stream);
                    stream.Position = 0;
                    using var reader = new StreamReader(stream);


                    var integers = new List<int>();
                    var dates = new List<DateTime>();
                    var strings = new List<string>();
                    string line;

                    ReadFromFiles(reader, integers, dates, strings);

                    WriteToFiles(integers, dates, strings);

                }


                return Ok();

            }
            else
            {
                return BadRequest();
            }
        }

        private static void ReadFromFiles(StreamReader reader, List<int> integers, List<DateTime> dates, List<string> strings)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                DateTime dateTime;
                int number;
                if (DateTime.TryParseExact(line, "dd.MM.yyyy HH:mm:ss", null, DateTimeStyles.None, out dateTime))
                {
                    dates.Add(dateTime);
                }
                else if (int.TryParse(line, out number))
                {
                    integers.Add(number);

                }
                else
                {
                    strings.Add(line);
                }
            }

        }

        private static void WriteToFiles(List<int> integers, List<DateTime> dates, List<string> strings)
        {
            if (integers.Any())
            {
                integers.Sort();
                using (TextWriter tw = new StreamWriter("integers.txt"))
                {
                    foreach (int number in integers)
                        tw.WriteLine(number);
                }
            }


            if (dates.Any())
            {
                dates.Sort();
                using (TextWriter tw = new StreamWriter("dates.txt"))
                {
                    foreach (DateTime date in dates)
                        tw.WriteLine(date.ToString());
                }
            }

            if (strings.Any())
            {
                strings.Sort();
                using (TextWriter tw = new StreamWriter("strings.txt"))
                {
                    foreach (string text in strings)
                        tw.WriteLine(text);
                }

            }
        }



    }
}
