using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace RayTracer
{
	public static class RayTrace
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool SceneIntersect(Vector3 orig, Vector3 dir, List<Sphere> spheres, ref Vector3 hit, ref Vector3 N, ref Material material)
		{
			float spheresDist = float.MaxValue;

			for (int i = 0; i < spheres.Count; i++)
			{
				float distI = 0f;

				if (spheres[i].RayIntersect(orig, dir, ref distI) && distI < spheresDist)
				{
					spheresDist = distI;
					hit = orig + (dir * distI);
					N = Vector3.Normalize(hit - spheres[i].Centre);
					material = spheres[i].Material;
				}
			}

			return spheresDist < 1000f;
		}

		public static Vector3 CastRay(Vector3 orig, Vector3 dir, List<Sphere> spheres, List<Light> lights)
		{
			var point = new Vector3();
			var N = new Vector3();
			var material = new Material();

			if (!SceneIntersect(orig, dir, spheres, ref point, ref N, ref material))
			{
				return new Vector3(0.2f, 0.7f, 0.8f); // background colour
			}

			float diffuseLightIntensity = 0;
			float specularLightIntensity = 0;

			for (int i = 0; i < lights.Count; i++)
			{
				Vector3 lightDir = Vector3.Normalize(lights[i].Position - point);
				float lightDistance = (lights[i].Position - point).Length();

				Vector3 shadowOrig = Vector3.Dot(lightDir, N) < 0f ? point - (N * 1e-3f) : point + (N * 1e-3f);
				var shadowPt = new Vector3();
				var shadowN = new Vector3();
				var tmpMaterial = new Material();

				if (SceneIntersect(shadowOrig, lightDir, spheres, ref shadowPt, ref shadowN, ref tmpMaterial)
					&& (shadowPt - shadowOrig).Length() < lightDistance)
				{
					continue;
				}

				diffuseLightIntensity += lights[i].Intensity * MathF.Max(0f, Vector3.Dot(lightDir, N));
				specularLightIntensity += MathF.Pow(MathF.Max(0f, Vector3.Dot(-Vector3.Reflect(-lightDir, N), dir)), material.SpecularExponent) * lights[i].Intensity;
			}

			return (material.DiffuseColour * diffuseLightIntensity * material.Albedo.X) + new Vector3(specularLightIntensity * material.Albedo.Y);
		}

		private static void Render(List<Sphere> spheres, List<Light> lights)
		{
			const int width = 1980;
			const int height = 1080;

			float tanFov = 2f * MathF.Tan(MathF.PI / 6);
			float z = -height / tanFov;
			var framebuffer = new Vector3[width * height];

			var sw = Stopwatch.StartNew();

			_ = Parallel.For(0, height, yPos =>
			  {
				  for (int xPos = 0; xPos < width; xPos++)
				  {
					  float x = (xPos + 0.5f) - width * 0.5f;
					  float y = -(yPos + 0.5f) + height * 0.5f;
					  var dir = Vector3.Normalize(new Vector3(x, y, z));

					  framebuffer[xPos + (yPos * width)] = CastRay(Vector3.Zero, dir, spheres, lights);
				  }
			  });

			sw.Stop();
			Console.WriteLine($"Image took: {sw.ElapsedMilliseconds}ms to render");

			using var bw = new BinaryWriter(File.Open("image.ppm", FileMode.Create));
			bw.Write($"P6\n{width} {height}\n255\n".ToCharArray());

			for (int i = 0; i < width * height; i++)
			{
				ref Vector3 pixel = ref framebuffer[i];

				// Clamp elements above 1
				pixel = Vector3.Clamp(pixel, Vector3.Zero, Vector3.One);

				// Write out bytes to file
				bw.Write((byte)(255.999f * pixel.X));
				bw.Write((byte)(255.999f * pixel.Y));
				bw.Write((byte)(255.999f * pixel.Z));
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