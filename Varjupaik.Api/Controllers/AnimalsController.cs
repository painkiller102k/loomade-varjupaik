using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Varjupaik.Services;
using Varjupaik.Services.Dtos;

namespace Varjupaik.Api.Controllers;

[ApiController]
[Route("api/animals")]
[Authorize(Roles = "Admin,Staff")]
public class AnimalsController : ControllerBase
{
    private readonly AnimalService _animalService;
    private readonly IWebHostEnvironment _env;

    public AnimalsController(AnimalService animalService, IWebHostEnvironment env)
    {
        _animalService = animalService;
        _env = env;
    }

    // GET /api/animals?species=&status=&search=&sortBy=&page=&pageSize=
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] AnimalQuery query)
    {
        var result = await _animalService.GetAllAsync(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var animal = await _animalService.GetByIdAsync(id);
        if (animal == null)
        {
            return NotFound();
        }
        return Ok(animal);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] AnimalSaveDto dto, IFormFile? photo)
    {
        var photoPath = await SavePhoto(photo);
        if (photo != null && photoPath == null)
        {
            return BadRequest(new { message = "Lubatud on ainult pildifailid (jpg, png, gif, webp)" });
        }

        var animal = await _animalService.CreateAsync(dto, photoPath);
        return CreatedAtAction(nameof(GetById), new { id = animal.Id }, animal);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromForm] AnimalSaveDto dto, IFormFile? photo)
    {
        var photoPath = await SavePhoto(photo);
        if (photo != null && photoPath == null)
        {
            return BadRequest(new { message = "Lubatud on ainult pildifailid (jpg, png, gif, webp)" });
        }

        var animal = await _animalService.UpdateAsync(id, dto, photoPath);
        if (animal == null)
        {
            return NotFound();
        }
        return Ok(animal);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _animalService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }

    // Salvestab pildi kausta wwwroot/uploads ja tagastab selle aadressi
    private async Task<string?> SavePhoto(IFormFile? photo)
    {
        if (photo == null || photo.Length == 0)
        {
            return null;
        }

        var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var extension = Path.GetExtension(photo.FileName).ToLower();
        if (!allowed.Contains(extension))
        {
            return null;
        }

        var folder = Path.Combine(_env.WebRootPath, "uploads");
        Directory.CreateDirectory(folder);

        var fileName = Guid.NewGuid() + extension;
        using (var stream = new FileStream(Path.Combine(folder, fileName), FileMode.Create))
        {
            await photo.CopyToAsync(stream);
        }

        return "/uploads/" + fileName;
    }
}
