using System;
using System.Collections.Generic;
using System.IO;

using ToxicRagers.Helpers;

namespace ToxicRagers.TDR2000.Formats
{
    public class FUNC
    {
        public string XAxisTitle { get; set; }
        public string YAxisTitle { get; set; }

        //Target Objects
        public double FunctionTranslateX { get; set; }
        public double FunctionTranslateY { get; set; }
        public double FunctionScaleX { get; set; }
        public double FunctionScaleY { get; set; }
        public double CameraTranslateX { get; set; }
        public double CameraTranslateY { get; set; }
        public double CameraScaleX { get; set; }
        public double CameraScaleY { get; set; }
        public double SelectionTranslateX { get; set; }
        public double SelectionTranslateY { get; set; }
        public double SelectionScaleX { get; set; }
        public double SelectionScaleY { get; set; }

        public List<FUNCPoint> Points { get; set; } = new List<FUNCPoint>();

        public static FUNC Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            FUNC func = new FUNC();

            using (BinaryReader br = new BinaryReader(fi.OpenRead()))
            {
                _ = br.ReadByte(); // 1
                func.XAxisTitle = br.ReadString((int)br.ReadUInt32());
                func.YAxisTitle = br.ReadString((int)br.ReadUInt32());

                func.FunctionTranslateX = br.ReadDouble();
                func.FunctionTranslateY = br.ReadDouble();
                func.FunctionScaleX = br.ReadDouble();
                func.FunctionScaleY = br.ReadDouble();

                func.CameraTranslateX = br.ReadDouble();
                func.CameraTranslateY = br.ReadDouble();
                func.CameraScaleX = br.ReadDouble();
                func.CameraScaleY = br.ReadDouble();

                func.SelectionTranslateX = br.ReadDouble();
                func.SelectionTranslateY = br.ReadDouble();
                func.SelectionScaleX = br.ReadDouble();
                func.SelectionScaleY = br.ReadDouble();

                uint pointCount = br.ReadUInt32();

                for (int i = 0; i < pointCount; i++)
                {
                    func.Points.Add(new FUNCPoint
                    {
                        X = br.ReadDouble(),
                        Y = br.ReadDouble(),
                        Unknown1 = br.ReadDouble(),
                        Selected = br.ReadByte()
                    });

                    _ = br.ReadBytes(7);
                }
            }

            return func;
        }
    }

    public class FUNCPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Unknown1 { get; set; }
        public byte Selected { get; set; }
    }
}
