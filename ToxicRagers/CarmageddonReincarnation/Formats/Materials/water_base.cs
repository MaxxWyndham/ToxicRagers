﻿using System.Linq;
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

        [Required]
        public string DiffuseColour
        {
            get => GetFile("diffuse");
            set => fileNames.Add("diffuse", value);
        }

        [Required]
        public string Normal_Map
        {
            get => GetFile("normal");
            set => fileNames.Add("normal", value);
        }

        [Required]
        public string Spec_Map
        {
            get => GetFile("specular");
            set => fileNames.Add("specular", value);
        }

        [Required]
        public string Normal_Map2
        {
            get => GetFile("normal2");
            set => fileNames.Add("normal2", value);
        }

        string foamMap;
        string cubeMap;

        Vector3 minDistance;
        Vector3 maxDistance;
        Vector3 seaFalloff;
        Vector3 shoreFactor;

        public string Foam_Map
        {
            get => foamMap;
            set => foamMap = value;
        }

        public string EnvironmentCube
        {
            get => cubeMap;
            set => cubeMap = value;
        }

        public float Min_Distance
        {
            get => minDistance.X;
            set => minDistance.X = value;
        }

        public float Max_Distance
        {
            get => maxDistance.X;
            set => maxDistance.X = value;
        }

        public float Sea_Falloff
        {
            get => seaFalloff.X;
            set => seaFalloff.X = value;
        }

        public float shore_factor
        {
            get => shoreFactor.X;
            set => shoreFactor.X = value;
        }

        public water_base() { }

        public water_base(XElement xml)
            : base(xml)
        {
            XElement diff = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "DiffuseColour").FirstOrDefault();
            XElement nor1 = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Normal_Map").FirstOrDefault();
            XElement spec = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Spec_Map").FirstOrDefault();
            XElement nor2 = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Normal_Map2").FirstOrDefault();
            XElement foam = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "Foam_Map").FirstOrDefault();
            XElement cube = xml.Descendants("Texture").Where(e => e.Attribute("Alias").Value == "EnvironmentCube").FirstOrDefault();

            if (diff != null) { DiffuseColour = diff.Attribute("FileName").Value; }
            if (nor1 != null) { Normal_Map = nor1.Attribute("FileName").Value; }
            if (spec != null) { Spec_Map = spec.Attribute("FileName").Value; }
            if (nor2 != null) { Normal_Map2 = nor2.Attribute("FileName").Value; }
            if (foam != null) { foamMap = foam.Attribute("FileName").Value; }
            if (cube != null) { cubeMap = cube.Attribute("FileName").Value; }

            XElement mind = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "Min_distance").FirstOrDefault();
            XElement maxd = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "Max_distance").FirstOrDefault();
            XElement seaf = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "Sea_Falloff").FirstOrDefault();
            XElement shor = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "shore_factor").FirstOrDefault();

            if (mind != null) { minDistance = ReadConstant(mind); }
            if (maxd != null) { maxDistance = ReadConstant(maxd); }
            if (seaf != null) { seaFalloff = ReadConstant(seaf); }
            if (shor != null) { shoreFactor = ReadConstant(shor); }
        }
    }
}