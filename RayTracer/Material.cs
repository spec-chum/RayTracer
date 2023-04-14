using System.Numerics;

namespace RayTracer
{
	public readonly struct Material
	{
		public readonly Vector3 DiffuseColour;
		public readonly Vector3 Albedo;
		public readonly float SpecularExponent;

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