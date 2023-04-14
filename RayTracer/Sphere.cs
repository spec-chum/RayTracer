using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace RayTracer;

public readonly struct Sphere
{
    private readonly float _radiusSqrd;

    public Vector3 Centre { get; }
    public float Radius { get; }
    public Material Material { get; }

    public Sphere(Vector3 centre, float radius, Material material)
    {
        Centre = centre;
        Radius = radius;
        Material = material;
        _radiusSqrd = radius * radius;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool RayIntersect(Vector3 origin, Vector3 direction, ref float t0)
    {
        Vector3 L = Centre - origin;
        float tca = Vector3.Dot(L, direction);
        float d2 = Vector3.Dot(L, L) - (tca * tca);

        if (d2 > _radiusSqrd)
        {
            return false;
        }

        float thc = MathF.Sqrt(_radiusSqrd - d2);
        t0 = tca >= thc ? tca - thc : tca + thc;

        return t0 >= 0;
    }
}