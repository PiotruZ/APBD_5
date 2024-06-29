using APBD_5.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;


namespace APBD_5.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnimalsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AnimalsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetAnimals([FromQuery] string orderBy = "name")
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            List<Animal> animals = new List<Animal>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = $"SELECT * FROM Animals ORDER BY {orderBy}";
                SqlCommand command = new SqlCommand(query, connection);
                await connection.OpenAsync();
                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    animals.Add(new Animal
                    {
                        IdAnimal = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                        Category = reader.GetString(3),
                        Area = reader.GetString(4)
                    });
                }
            }

            return Ok(animals);
        }

        [HttpPost]
        public async Task<IActionResult> AddAnimal([FromBody] Animal newAnimal)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Animals (Name, Description, Category, Area) VALUES (@Name, @Description, @Category, @Area)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Name", newAnimal.Name);
                command.Parameters.AddWithValue("@Description", newAnimal.Description);
                command.Parameters.AddWithValue("@Category", newAnimal.Category);
                command.Parameters.AddWithValue("@Area", newAnimal.Area);
                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }

            return Ok();
        }

        [HttpPut("{idAnimal}")]
        public async Task<IActionResult> UpdateAnimal(int idAnimal, [FromBody] Animal updatedAnimal)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE Animals SET Name = @Name, Description = @Description, Category = @Category, Area = @Area WHERE IdAnimal = @IdAnimal";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IdAnimal", idAnimal);
                command.Parameters.AddWithValue("@Name", updatedAnimal.Name);
                command.Parameters.AddWithValue("@Description", updatedAnimal.Description);
                command.Parameters.AddWithValue("@Category", updatedAnimal.Category);
                command.Parameters.AddWithValue("@Area", updatedAnimal.Area);
                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }

            return Ok();
        }

        [HttpDelete("{idAnimal}")]
        public async Task<IActionResult> DeleteAnimal(int idAnimal)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Animals WHERE IdAnimal = @IdAnimal";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IdAnimal", idAnimal);
                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }

            return Ok();
        }
    }
}
