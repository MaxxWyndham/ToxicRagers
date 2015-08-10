using System;
using System.Linq;
using System.Xml.Linq;

using ToxicRagers.Helpers;

namespace ToxicRagers.CarmageddonReincarnation.Formats.Materials
{
    public class water_base : MT2
    {
        // Uses:
        // DoubleSided
        // NeedsWorldLightDir, NeedsLightingSpaceVertexNormal, NeedsLocalCubeMap
        // Multiplier

        string normal;
        string specular;
        string normal2;
        string foamMap;
        string cubeMap;

        Vector3 minDistance;
        Vector3 maxDistance;
        Vector3 seaFalloff;
        Vector3 shoreFactor;

        public string Normal_Map
        {
            get { return normal; }
            set { normal = value; }
        }

        public string Spec_Map
        {
            get { return specular; }
            set { specular = value; }
        }

        public string Normal_Map2
        {
            get { return normal2; }
            set { normal2 = value; }
        }

        public string Foam_Map
        {
            get { return foamMap; }
            set { foamMap = value; }
        }

        public string EnvironmentCube
        {
            get { return cubeMap; }
            set { cubeMap = value; }
        }

        public Single Min_Distance
        {
            get { return minDistance.X; }
            set { minDistance.X = value; }
        }

        public Single Max_Distance
        {
            get { return maxDistance.X; }
            set { maxDistance.X = value; }
        }

        public Single Sea_Falloff
        {
            get { return seaFalloff.X; }
            set { seaFalloff.X = value; }
        }

        public Single shore_factor
        {
            get { return shoreFactor.X; }
            set { shoreFactor.X = value; }
        }

        public water_base(XElement xml)
            : base(xml)
        {
            var diff = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "DiffuseColour").FirstOrDefault();
            var nor1 = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Normal_Map").FirstOrDefault();
            var spec = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Spec_Map").FirstOrDefault();
            var nor2 = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Normal_Map2").FirstOrDefault();
            var foam = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Foam_Map").FirstOrDefault();
            var cube = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "EnvironmentCube").FirstOrDefault();

            if (diff != null) { diffuse = diff.Attribute("FileName").Value; }
            if (nor1 != null) { normal = nor1.Attribute("FileName").Value; }
            if (spec != null) { specular = spec.Attribute("FileName").Value; }
            if (nor2 != null) { normal2 = nor2.Attribute("FileName").Value; }
            if (foam != null) { foamMap = foam.Attribute("FileName").Value; }
            if (cube != null) { cubeMap = cube.Attribute("FileName").Value; }

            var mind = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "Min_distance").FirstOrDefault();
            var maxd = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "Max_distance").FirstOrDefault();
            var seaf = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "Sea_Falloff").FirstOrDefault();
            var shor = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "shore_factor").FirstOrDefault();

            if (mind != null) { minDistance = ReadConstant(mind); }
            if (maxd != null) { maxDistance = ReadConstant(maxd); }
            if (seaf != null) { seaFalloff = ReadConstant(seaf); }
            if (shor != null) { shoreFactor = ReadConstant(shor); }
        }
    }
}
