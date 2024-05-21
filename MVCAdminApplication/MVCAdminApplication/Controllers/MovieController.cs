using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using MVCAdminApplication.Models;
using Newtonsoft.Json;
using System.Text;

namespace MVCAdminApplication.Controllers
{
    public class MovieController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ImportMovies(IFormFile file)
        {

            string pathToUpload = $"{Directory.GetCurrentDirectory()}\\files\\{file.FileName}";


            using (FileStream fileStream = System.IO.File.Create(pathToUpload))
            {
                file.CopyTo(fileStream);
                fileStream.Flush();
            }

            List<Movie> movies = getAllMoviesFromFile(file.FileName);
            HttpClient client = new HttpClient();
            string URL = "https://localhost:44341/api/Admin/saveMovies";

            HttpContent content = new StringContent(JsonConvert.SerializeObject(movies), Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync(URL, content).Result;

            var result = response.Content.ReadAsAsync<bool>().Result;

            return RedirectToAction("Index", "Order");
        }
        private List<Movie> getAllMoviesFromFile(string fileName)
        {
            List<Movie> movies = new List<Movie>();
            string filePath = $"{Directory.GetCurrentDirectory()}\\files\\{fileName}";

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        movies.Add(new Movie
                        {
                            MovieName = reader.GetValue(0).ToString(),
                            MovieDescription = reader.GetValue(1).ToString(),
                            MovieImage = reader.GetValue(2).ToString(),
                            Rating = (double)reader.GetValue(3),
                        });
                    }

                }
            }
            return movies;

        }
    }
}
