using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoiseDemo.Models;
using SimplexNoise;

namespace NoiseDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoiseController : ControllerBase
    {
        [HttpPost("generate")]
    public IActionResult GenerateNoise([FromBody] NoiseSettings settings)
    {
            int width = settings.Width;
            int height = settings.Height;
            float scale = settings.NoiseScale;
            int seed = new Random().Next(1, int.MaxValue);

            FastNoiseLite noise = new FastNoiseLite();
            noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            noise.SetFrequency(scale);
            noise.SetSeed(seed);
           

            double Noise(float nx, float ny)
            {
                // Rescale from -1.0:+1.0 to 0.0:1.0
                 return noise.GetNoise(nx, ny) / 2.0 + 0.5;
            }

            double[][] GenerateNoiseValues(int h, int w)
            {
                double[][] values = new double[h][];
                for (int y = 0; y < h; y++)
                {
                    values[y] = new double[w];
                    for (int x = 0; x < w; x++)
                    {
                        // Normalize x and y coordinates to the range [-0.5, 0.5)
                        float nx = (float)x / w - 0.5f;
                        float ny = (float)y / h - 0.5f;
                        values[y][x] = Math.Pow(Noise(nx, ny) + 0.5 * Noise(2*nx, 2* ny) + 0.25 * Noise(4*nx,4*ny),4);
                    }
                }
                return values;
            }


    // Call the method to generate noise values and store the result in a variable.
    double[][] noiseValues = GenerateNoiseValues(height, width);
    return Ok(noiseValues);
}
    }

}