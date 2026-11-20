namespace Varjupaik.Services.Dtos;

// Keskmine aeg varjupaigas ühe liigi kohta
public class AverageStayDto
{
    public string Species { get; set; } = "";

    /// <summary>Mitu looma sellest liigist on arvesse võetud</summary>
    public int AnimalCount { get; set; }

    /// <summary>Keskmine päevade arv varjupaigas (adopteeritud loomadel kuni adopteerimiseni)</summary>
    public double AverageDays { get; set; }
}
