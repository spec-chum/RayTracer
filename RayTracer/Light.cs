using System.Numerics;

namespace RayTracer
{
    public readonly struct Light
    {
        public Vector3 Position { get; }
        public float Intensity { get; }

        public Light(Vector3 position, float intensity)
        {
            Position = position;
            Intensity = intensity;
        }
    }
}