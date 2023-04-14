using System.Numerics;

namespace RayTracer
{
    public readonly struct Material
    {
        public Vector3 DiffuseColour { get; }
        public Vector3 Albedo { get; }
        public float SpecularExponent { get; }

        public Material()
        {
            Albedo = new Vector3();
            DiffuseColour = new Vector3();
            SpecularExponent = 0f;
        }

        public Material(Vector3 albedo, Vector3 colour, float specular)
        {
            Albedo = albedo;
            DiffuseColour = colour;
            SpecularExponent = specular;
        }
    }
}