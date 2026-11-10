using Varjupaik.Services.Models;

namespace Varjupaik.Services.Dtos;

// Avaliku nimekirja päringu parameetrid
public class AnimalQuery
{
    /// <summary>Liik, nt Koer või Kass</summary>
    /// <example>Koer</example>
    public string? Species { get; set; }

    /// <summary>Looma olek</summary>
    /// <example>Available</example>
    public AnimalStatus? Status { get; set; }

    /// <summary>Otsing nime ja kirjelduse järgi</summary>
    /// <example>sõbralik</example>
    public string? Search { get; set; }

    /// <summary>Sorteerimine: newest, oldest, name, age</summary>
    /// <example>newest</example>
    public string? SortBy { get; set; }

    /// <summary>Lehekülje number (algab 1-st)</summary>
    /// <example>1</example>
    public int Page { get; set; } = 1;

    /// <summary>Kirjeid ühel leheküljel (max 50)</summary>
    /// <example>9</example>
    public int PageSize { get; set; } = 9;
}
