using System.Numerics;

namespace RayTracer
{
	public class Material
	{
		public Vector3 DiffuseColour;
		public Vector3 Albedo;
		public float SpecularExponent;

		public Material()
		{
			Albedo = new Vector3();
			DiffuseColour = new Vector3();
			SpecularExponent = 0f;
		}

		public Material(Vector3 a, Vector3 colour, float spec)
		{
			Albedo = a;
			DiffuseColour = colour;
			SpecularExponent = spec;
		}
	}
}