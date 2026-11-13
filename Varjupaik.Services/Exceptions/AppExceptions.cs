namespace Varjupaik.Services.Exceptions;

// Teenused viskavad need erandid, API globaalne veakäsitleja teeb neist ProblemDetails vastuse

/// <summary>Midagi ei leitud (404)</summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}

/// <summary>Vigased andmed või keelatud tegevus (400)</summary>
public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message)
    {
    }
}

/// <summary>Sisselogimine ebaõnnestus (401)</summary>
public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message)
    {
    }
}
