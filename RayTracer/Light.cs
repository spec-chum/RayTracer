using System.Numerics;

namespace RayTracer
{
	public class Light
	{
		public Vector3 Position;
		public float Intensity;

		public Light(Vector3 p, float i)
		{
			Position = p;
			Intensity = i;
		}
	}
}