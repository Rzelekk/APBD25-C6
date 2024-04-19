using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WebApplication2.Models;
using WebApplication2.Models.DTOs;

namespace WebApplication2.Controllers;

[ApiController]
//[Route("api/animals")]
[Route("api/[controller]")]
public class AnimalsController : ControllerBase
{
    private IConfiguration _configuration;

    public AnimalsController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    [HttpGet]
    public IActionResult GetAnimals(string orderBy = "name")
    {
        // otwieranie połączenia
        SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();
        
        // sprawdzenie parametru
        if (orderBy.ToLower() != "idanimal"
            || orderBy.ToLower() != "name"
            || orderBy.ToLower() != "description"
            || orderBy.ToLower() != "area")
        {
            orderBy = "name";
        }
        
        // definiujemy comanda
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = $"SELECT * FROM Animal ORDER BY {orderBy} asc";
        
        // wykonanie comand
        var reader = command.ExecuteReader();
        List<Animal> animals = new List<Animal>();
        
        int idAnimalOrginal = reader.GetOrdinal("IdAnimal");
        int nameOrginal = reader.GetOrdinal("Name");
        int descOrginal = reader.GetOrdinal("Description");
        int categoryOrginal = reader.GetOrdinal("Category");
        int areaOrginal = reader.GetOrdinal("Area");
        
        while (reader.Read())
        {
                animals.Add(new Animal()
                {
                    IdAnimal = reader.GetInt32(idAnimalOrginal),
                    Name = reader.GetString(nameOrginal),
                    Description = reader.GetString(descOrginal),
                    Category = reader.GetString(categoryOrginal),
                    Area = reader.GetString(areaOrginal)
                });
        }
        
        return Ok(animals);
    }

    [HttpPost]
    public IActionResult AddAnimal([FromBody] AddAnimal animal)
    {
      
       
        
        // otwieranie połączenia
        SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();
        
        // definiujemy comanda
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "INSERT into Animal VALUES(@animalName, '', '', '')";
        command.Parameters.AddWithValue("@animalName", animal.Name);
        command.Parameters.AddWithValue("@Description", animal.Description ?? null);
        command.Parameters.AddWithValue("@Category", animal.Category ?? "cat");
        command.Parameters.AddWithValue("@Area", animal.Area ?? "area");

        command.ExecuteNonQuery();
        
        return Created("", null);
    }

    [HttpPut]
    [Route("api/animal/{IdAnimal}")]

    public IActionResult UpdateAnimal(int IdAnimal, UpdateAnimal updateAnimal )
    {
        // otwieranie połączenia
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();

        // definiujemy comanda
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;

        command.CommandText = "UPDATE Animal SET ";

        // Dodawanie warunków do zapytania SQL tylko dla przekazanych wartości
        command.CommandText += updateAnimal.Name != "string" ? "Name = @animalName, " : "Name = Name, ";
        command.Parameters.AddWithValue("@animalName", updateAnimal.Name);

        command.CommandText += updateAnimal.Description != "string" ? "Description = @Description, " : "Description = Description, ";
        command.Parameters.AddWithValue("@Description", updateAnimal.Description);

        command.CommandText += updateAnimal.Category != "string" ? "Category = @Category, " : "Category = Category, ";
        command.Parameters.AddWithValue("@Category", updateAnimal.Category);

        command.CommandText += updateAnimal.Area != "string" ? "Area = @Area, " : "Area = Area, ";
        command.Parameters.AddWithValue("@Area", updateAnimal.Area);

        // Usuń ostatnią przecinek i spację
        command.CommandText = command.CommandText.TrimEnd(',', ' ');

        // Dodaj warunek WHERE
        command.CommandText += " WHERE IdAnimal = @IdAnimal";
        command.Parameters.AddWithValue("@IdAnimal", IdAnimal);


        command.ExecuteNonQuery();
        
        return Ok();
    }

    
    
}