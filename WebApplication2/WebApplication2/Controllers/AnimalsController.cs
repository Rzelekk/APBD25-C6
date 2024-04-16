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
    public IActionResult GetAnimals()
    {
        // otwieranie połączenia
        SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();
        
        // definiujemy comanda
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "SELECT * FROM Animal";
        
        // wykonanie comand
        var reader = command.ExecuteReader();
        List<Animal> animals = new List<Animal>();

        int idAnimalOrginal = reader.GetOrdinal("IdAnimal");
        int nameOrginal = reader.GetOrdinal("Name");
        
        while (reader.Read())
        {
                animals.Add(new Animal()
                {
                    IdAnimal = reader.GetInt32(idAnimalOrginal),
                    Name = reader.GetString(nameOrginal)
                });
        }
        
        return Ok(animals);
    }

    [HttpPost]
    public IActionResult AddAnimal(AddAnimal animal)
    {
      
        // otwieranie połączenia
        SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();
        
        // definiujemy comanda
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "INSERT into Animal VALUES(@animalName, '', '', '')";
        command.Parameters.AddWithValue("@animalName", animal.Name);

        command.ExecuteNonQuery();
        
        return Created("", null);
    }
    
    
}