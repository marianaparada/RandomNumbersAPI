using Microsoft.AspNetCore.Mvc;

namespace RandomNumbers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RandomNumbersController : ControllerBase
    {
        public readonly Random _random = new Random();
        
        [HttpGet("number")]
        public IActionResult GetRandomNumber([FromQuery] int? min, [FromQuery] int? max)
        {
            if (min.HasValue && max.HasValue)
            {
                if (min > max)
                {
                    return BadRequest("el valor minimo no puede ser mayor que el maximo");
                }

                return Ok(_random.Next(min.Value, max.Value + 1));
            }

            return Ok(_random.Next());
        }

        [HttpGet("decimal")]
        public IActionResult GetRandomDecimal()
        {
            return Ok(_random.NextDouble());
        }

        [HttpGet("string")]
        public IActionResult GetRandomString([FromQuery] int length = 8)
        {
            if (length < 1 || length > 1024)
                return BadRequest("La longitud debe estar entre 1 y 1024.");

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var result = new char[length];

            for (int i = 0; i < length; i++)
                result[i] = chars[_random.Next(chars.Length)];

            return Ok(new string(result));
        }

        [HttpPost("custom")]
        public IActionResult GetCustomRandom([FromBody] CustomRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Type))
            {
                return BadRequest("Debe especificar un tipo válido.");
            }

            switch (request.Type.ToLower())
            {
                case "number":
                    if (!request.Min.HasValue || !request.Max.HasValue)
                        return BadRequest("Debe especificar min y max para números.");
                    if (request.Min > request.Max)
                        return BadRequest("min no puede ser mayor que max.");
                    return Ok(new { result = _random.Next(request.Min.Value, request.Max.Value + 1) });

                case "decimal":
                    int decimals = request.Decimals ?? 2;
                    double value = Math.Round(_random.NextDouble(), decimals);
                    return Ok(new { result = value });

                case "string":
                    int length = request.Length ?? 8;
                    if (length < 1 || length > 1024)
                        return BadRequest("La longitud debe estar entre 1 y 1024.");
                    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                    var result = new char[length];
                    for (int i = 0; i < length; i++)
                        result[i] = chars[_random.Next(chars.Length)];
                    return Ok(new { result = new string(result) });

                default:
                    return BadRequest("Tipo inválido. Use 'number', 'decimal' o 'string'.");
            }
        }

        public class CustomRequest
        {
            public string? Type { get; set; }
            public int? Min { get; set; }
            public int? Max { get; set; }
            public int? Decimals { get; set; }
            public int? Length { get; set; }
        }
    }
}