using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace RayTracer
{
    public static class RayTrace
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SceneIntersect(Vector3 origin, Vector3 direction, List<Sphere> spheres, ref Vector3 hit, ref Vector3 normal, ref Material material)
        {
            float spheresDist = float.MaxValue;

            for (int i = 0; i < spheres.Count; i++)
            {
                float distI = 0f;

                if (spheres[i].RayIntersect(origin, direction, ref distI) && distI < spheresDist)
                {
                    spheresDist = distI;
                    hit = origin + (direction * distI);
                    normal = Vector3.Normalize(hit - spheres[i].Centre);
                    material = spheres[i].Material;
                }
            }

            return spheresDist < 1000f;
        }

        public static Vector3 CastRay(Vector3 origin, Vector3 direction, List<Sphere> spheres, List<Light> lights)
        {
            var point = Vector3.Zero;
            var normal = Vector3.Zero;
            var material = new Material();

            if (!SceneIntersect(origin, direction, spheres, ref point, ref normal, ref material))
            {
                return new Vector3(0.2f, 0.7f, 0.8f); // background colour
            }

            float diffuseLightIntensity = 0;
            float specularLightIntensity = 0;

            foreach (Light v in lights)
            {
                Vector3 lightDir = Vector3.Normalize(v.Position - point);
                float lightDistance = (v.Position - point).Length();

                Vector3 shadowOrig = Vector3.Dot(lightDir, normal) < 0f ? point - (normal * 1e-3f) : point + (normal * 1e-3f);
                var shadowPt = Vector3.Zero;
                var shadowN = Vector3.Zero;
                var tmpMaterial = new Material();

                if (SceneIntersect(shadowOrig, lightDir, spheres, ref shadowPt, ref shadowN, ref tmpMaterial)
                    && (shadowPt - shadowOrig).Length() < lightDistance)
                {
                    continue;
                }

                diffuseLightIntensity += v.Intensity * MathF.Max(0f, Vector3.Dot(lightDir, normal));
                specularLightIntensity += MathF.Pow(MathF.Max(0f, Vector3.Dot(-Vector3.Reflect(-lightDir, normal), direction)), material.SpecularExponent) * v.Intensity;
            }

            return (material.DiffuseColour * diffuseLightIntensity * material.Albedo.X) + new Vector3(specularLightIntensity * material.Albedo.Y);
        }

        private static void Render(List<Sphere> spheres, List<Light> lights)
        {
            const int width = 1980;
            const int height = 1080;
            const float halfWidth = width * 0.5f;
            const float halfHeight = height * 0.5f;

            var framebuffer = new Vector3[width * height];

            float tanFov = 2f * MathF.Tan(MathF.PI / 6);
            float z = -height / tanFov;

            var sw = Stopwatch.StartNew();

            _ = Parallel.For(0, height, yPos =>
              {
                  int stride = yPos * width;
                  for (int xPos = 0; xPos < width; xPos++)
                  {
                      float x = xPos + 0.5f - halfWidth;
                      float y = -yPos + 0.5f + halfHeight;
                      var direction = Vector3.Normalize(new Vector3(x, y, z));

                      framebuffer[xPos + stride] = CastRay(Vector3.Zero, direction, spheres, lights);
                  }
              });

            sw.Stop();
            Console.WriteLine($"Image took: {sw.ElapsedMilliseconds}ms to render");

            using var binaryWriter = new BinaryWriter(File.Open("image.ppm", FileMode.Create));
            binaryWriter.Write($"P6\n{width} {height}\n255\n".ToCharArray());

            for (int i = 0; i < width * height; i++)
            {
                ref Vector3 pixel = ref framebuffer[i];

                // Clamp elements above 1
                pixel = Vector3.Clamp(pixel, Vector3.Zero, Vector3.One);

                // Write out bytes to file
                binaryWriter.Write((byte)(255.999f * pixel.X));
                binaryWriter.Write((byte)(255.999f * pixel.Y));
                binaryWriter.Write((byte)(255.999f * pixel.Z));
            }
        }

        private static void Main()
        {
            var ivory = new Material(new Vector3(0.6f, 0.3f, 0f), new Vector3(0.4f, 0.4f, 0.3f), 50f);
            var redRubber = new Material(new Vector3(0.9f, 0.1f, 0f), new Vector3(0.3f, 0.1f, 0.1f), 10f);

            var spheres = new List<Sphere>
            {
                new Sphere(new Vector3(-3f, 0f, -16f), 2, ivory),
                new Sphere(new Vector3(-1.0f, -1.5f, -12f), 2, redRubber),
                new Sphere(new Vector3(1.5f, -0.5f, -18f), 3, redRubber),
                new Sphere(new Vector3(7f, 5f, -18f), 4, ivory)
            };

            var lights = new List<Light>
            {
                new Light(new Vector3(-20f, 20f, 20f), 1.5f),
                new Light(new Vector3(30f, 50f, -25f), 1.8f),
                new Light(new Vector3(30f, 20f,  30f), 1.7f)
            };

            Render(spheres, lights);
        }
    }
}