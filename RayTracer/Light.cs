using System.Numerics;

namespace RayTracer
{
	public readonly struct Light
	{
		public readonly Vector3 Position;
		public readonly float Intensity;

		public Light(Vector3 p, float i)
		{
			Position = p;
			Intensity = i;
		}
	}
}