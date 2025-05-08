using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using School.Data;
using School.Models.Casting;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace School.Controllers
{
    public class StudentController : Controller
    {
        private readonly StudentDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public StudentController(IConfiguration configuration, StudentDbContext context)
        {
            _context = context;
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<IActionResult> Index()
        {
            using var connection = CreateConnection();
            var students = await connection.QueryAsync<StudentDetails>("GetAllStudents");
            return View(students);
        }




        public IActionResult Register() => View();
        public IActionResult Matronic() => View();




        [HttpPost]
        public async Task<IActionResult> SaveStudent(StudentDetails model)
        {
            if (ModelState.IsValid)
            {

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                    Directory.CreateDirectory(uploadsFolder);
                    string fileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }

                    model.ImagePath = "/images/" + fileName;
                }
                else
                {

                    model.ImagePath = "/images/dhoni.jpg";
                }
                using var connection = CreateConnection();
                model.CountryName = await connection.QueryFirstOrDefaultAsync<string>(
          "SELECT Name FROM Countries WHERE Id = @Id", new { Id = model.CountryId });

                model.StateName = await connection.QueryFirstOrDefaultAsync<string>(
                    "SELECT Name FROM States WHERE Id = @Id", new { Id = model.StateId });

                model.CityName = await connection.QueryFirstOrDefaultAsync<string>(
                    "SELECT Name FROM Cities WHERE Id = @Id", new { Id = model.CityId });



                var parameters = new DynamicParameters();
                parameters.Add("@Name", model.Name);
                parameters.Add("@Email", model.Email);
                parameters.Add("@Phone", model.Phone);
                parameters.Add("@CountryId", model.CountryId);
                parameters.Add("@StateId", model.StateId);
                parameters.Add("@CityId", model.CityId);
                parameters.Add("@CityName", model.CityName);
                parameters.Add("@CountryName", model.CountryName);
                parameters.Add("@StateName", model.StateName);

                parameters.Add("@ImagePath", model.ImagePath);


                try
                {
                    await connection.ExecuteAsync("InsertStudent", parameters);
                    TempData["Success"] = "Student saved successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error: " + ex.Message;
                    return View("Register", model);
                }

            }

            return View("Register", model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            using var connection = CreateConnection();
            var student = await connection.QueryFirstOrDefaultAsync<StudentDetails>(
                "GetStudentById",
                new { Id = id }

            );

            if (student == null)
            {
                TempData["Error"] = "Student not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(student);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(StudentDetails model)
        {
            if (ModelState.IsValid)
            {
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                    Directory.CreateDirectory(uploadsFolder);
                    string fileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }

                    model.ImagePath = "/images/" + fileName;
                }
                else
                {

                    model.ImagePath = "/images/dhoni.jpg";
                }

                using var connection = CreateConnection();


                var countryName = await connection.QueryFirstOrDefaultAsync<string>(
                    "SELECT Name FROM Countries WHERE Id = @Id",
                    new { Id = model.CountryId });

                var stateName = await connection.QueryFirstOrDefaultAsync<string>(
                    "SELECT Name FROM States WHERE Id = @Id",
                    new { Id = model.StateId });

                var cityName = await connection.QueryFirstOrDefaultAsync<string>(
                    "SELECT Name FROM Cities WHERE Id = @Id",
                    new { Id = model.CityId });

                var parameters = new DynamicParameters();
                parameters.Add("@Id", model.Id);
                parameters.Add("@Name", model.Name);
                parameters.Add("@Email", model.Email);
                parameters.Add("@Phone", model.Phone);
                parameters.Add("@CountryId", model.CountryId);
                parameters.Add("@StateId", model.StateId);
                parameters.Add("@CityId", model.CityId);
                parameters.Add("@CountryName", countryName);
                parameters.Add("@StateName", stateName);
                parameters.Add("@CityName", cityName);
                parameters.Add("@ImagePath", model.ImagePath);

                try
                {
                    await connection.ExecuteAsync("UpdateStudent", parameters);
                    TempData["Success"] = "Student updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error: " + ex.Message;
                    return View(model);
                }


               
                
            }
            return View(model);
        }



        public async Task<IActionResult> Delete(int id)
        {
            using var connection = CreateConnection();
            await connection.ExecuteAsync("DeleteStudent", new { Id = id }, commandType: CommandType.StoredProcedure);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<JsonResult> GetCountries()
        {
            using var connection = CreateConnection();
            var countries = await connection.QueryAsync<Country>("GetAllCountries");
            return Json(countries);
        }

        [HttpGet]
        public async Task<JsonResult> GetStates(int id)
        {
            using var connection = CreateConnection();
            var states = await connection.QueryAsync<State>("GetStatesByCountryId", new { CountryId = id });
            return Json(states);
        }

        [HttpGet]
        public async Task<JsonResult> GetCities(int id)
        {
            using var connection = CreateConnection();
            var cities = await connection.QueryAsync<City>("GetCitiesByStateId", new { StateId = id });
            return Json(cities);
        }
    }
}
