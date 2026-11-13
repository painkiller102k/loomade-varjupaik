using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Varjupaik.Services;
using Varjupaik.Services.Dtos;
using Varjupaik.Services.Exceptions;

namespace Varjupaik.Api.Controllers;

/// <summary>
/// Loomade nimekiri (avalik) ja loomade haldus (Admin, Staff)
/// </summary>
[ApiController]
[Route("api/animals")]
[Produces("application/json")]
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

    /// <summary>
    /// Avalik loomade nimekiri filtrite, otsingu, sorteerimise ja lehekülgedega
    /// </summary>
    /// <remarks>
    /// Näide: GET /api/animals?species=Koer&amp;status=Available&amp;search=sõbralik&amp;sortBy=name&amp;page=1&amp;pageSize=9
    /// </remarks>
    /// <response code="200">Lehekülg loomadega</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedResult<AnimalDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] AnimalQuery query)
    {
        var result = await _animalService.GetAllAsync(query);
        return Ok(result);
    }

    /// <summary>
    /// Ühe looma andmed
    /// </summary>
    /// <param name="id">Looma id</param>
    /// <response code="200">Loom leiti</response>
    /// <response code="404">Looma ei leitud</response>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AnimalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var animal = await _animalService.GetByIdAsync(id);
        return Ok(animal);
    }

    /// <summary>
    /// Lisab uue looma (multipart/form-data, pilt on valikuline)
    /// </summary>
    /// <response code="201">Loom lisati</response>
    /// <response code="400">Vigased andmed või vale pildifail</response>
    /// <response code="401">Pole sisse logitud</response>
    /// <response code="403">Roll ei ole Admin ega Staff</response>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(AnimalDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromForm] AnimalSaveDto dto, IFormFile? photo)
    {
        var photoPath = await SavePhoto(photo);
        if (photo != null && photoPath == null)
        {
            throw new BadRequestException("Lubatud on ainult pildifailid (jpg, png, gif, webp)");
        }

        var animal = await _animalService.CreateAsync(dto, photoPath);
        return CreatedAtAction(nameof(GetById), new { id = animal.Id }, animal);
    }

    /// <summary>
    /// Muudab looma andmeid (uus pilt on valikuline)
    /// </summary>
    /// <param name="id">Looma id</param>
    /// <response code="200">Loom muudeti</response>
    /// <response code="400">Vigased andmed</response>
    /// <response code="404">Looma ei leitud</response>
    [HttpPut("{id}")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(AnimalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromForm] AnimalSaveDto dto, IFormFile? photo)
    {
        var photoPath = await SavePhoto(photo);
        if (photo != null && photoPath == null)
        {
            throw new BadRequestException("Lubatud on ainult pildifailid (jpg, png, gif, webp)");
        }

        var animal = await _animalService.UpdateAsync(id, dto, photoPath);
        return Ok(animal);
    }

    /// <summary>
    /// Kustutab looma
    /// </summary>
    /// <param name="id">Looma id</param>
    /// <response code="204">Kustutatud</response>
    /// <response code="404">Looma ei leitud</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _animalService.DeleteAsync(id);
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
