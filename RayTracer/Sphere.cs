using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace RayTracer
{
	public class Sphere
	{
		public readonly Vector3 Centre;
		public readonly float Radius;
		public readonly Material Material;

		public Sphere(Vector3 centre, float radius, Material material)
		{
			Centre = centre;
			Radius = radius;
			Material = material;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool RayIntersect(Vector3 origin, Vector3 direction, ref float t0)
		{
			Vector3 L = Centre - origin;
			float tca = Vector3.Dot(L, direction);
			float d2 = Vector3.Dot(L, L) - (tca * tca);
			float radiusSqrd = Radius * Radius;

			if (d2 > radiusSqrd)
			{
				return false;
			}

			float thc = (float)MathF.Sqrt((radiusSqrd) - d2);
			t0 = tca >= thc ? tca - thc : tca + thc;

			return t0 >= 0;
		}
	}
}