using System.IO;

using ToxicRagers.CarmageddonReincarnation.Helpers;
using ToxicRagers.Helpers;

namespace ToxicRagers.CarmageddonReincarnation.Formats
{
    public class VehicleSetup
    {
        VehicleSetupScript setup;

        public VehicleSetup() { }

        public static VehicleSetup Load(string path)
        {
            VehicleSetup vehicleSetup = new VehicleSetup();
            LOL lol = LOL.Load(path);

            vehicleSetup.setup = VehicleSetupScript.Parse(lol.Document);

            return vehicleSetup;
        }

        public void Save(string path)
        {
            using (StreamWriter sw = new StreamWriter(Path.Combine(path, "vehicle_setup.lol")))
            {
                sw.WriteLine(setup.ToString().Indent());
            }
        }
    }

    public class VehicleSetupScript : LUAScript
    {
        public VehicleSetupScript()
        {
            AddProperty(
                "default",
                LUAScriptPropertyType.Boolean
            );

            AddProperty(
                "camera_distance",
                LUAScriptPropertyType.Int
            );

            AddProperty(
                "camera_extra_tilt_angle",
                LUAScriptPropertyType.Int
            );

            AddProperty(
                "dev",
                LUAScriptPropertyType.Boolean
            );

            AddProperty(
                "cop",
                LUAScriptPropertyType.Boolean
            );

            AddProperty(
                "eagle",
                LUAScriptPropertyType.Boolean
            );

            AddProperty(
                "tier",
                LUAScriptPropertyType.Int
            );

            AddProperty(
                "progress",
                LUAScriptPropertyType.Int
            );

            AddProperty(
                "shipped",
                LUAScriptPropertyType.Boolean
            );

            AddProperty(
                "leaderboard_id",
                LUAScriptPropertyType.Int
            );

            AddProperty(
                "unlock_node",
                LUAScriptPropertyType.Int
            );

            AddProperty(
                "class_rounded",
                LUAScriptPropertyType.Boolean
            );

            AddProperty(
                "class_speedy",
                LUAScriptPropertyType.Boolean
            );

            AddProperty(
                "class_smashy",
                LUAScriptPropertyType.Boolean
            );

            AddProperty(
                "barred_from_easy_on_ramp",
                LUAScriptPropertyType.Boolean
            );

            AddProperty(
                "hawk",
                LUAScriptPropertyType.Boolean
            );

            AddProperty(
                "suppressor",
                LUAScriptPropertyType.Boolean
            );

            AddProperty(
                "product_id",
                LUAScriptPropertyType.Int
            );

            AddProperty(
                "consider_in_stats",
                LUAScriptPropertyType.Boolean
            );

            AddProperty(
                "override_vehicle_name",
                LUAScriptPropertyType.String
            );

            AddProperty(
                "override_driver_name",
                LUAScriptPropertyType.String
            );

            AddProperty(
                "override_bio",
                LUAScriptPropertyType.String
            );

            AddProperty(
                "hud_damage_levels",
                LUAScriptPropertyType.Float,
                true
            );
        }

        public static VehicleSetupScript Parse(string script)
        {
            return Parse<VehicleSetupScript>(script);
        }
    }
}