using ToxicRagers.Carmageddon.Helpers;
using ToxicRagers.Helpers;

namespace ToxicRagers.TDR2000.Formats
{
    public class Track
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string TrackSelectMesh { get; set; }

        public int TrackSelectRenderNode { get; set; }

        public string TrackSelectTexture { get; set; }

        public string SplashScreenMesh { get; set; }

        public int SplashScreenRenderNode { get; set; }

        public string SkySphere { get; set; }

        public int SkySphereRenderNode { get; set; }

        public string WaterMesh { get; set; }

        public int WaterRenderNode { get; set; }

        public float WaterLevel { get; set; }

        public string StaticMeshDescriptor { get; set; }

        public string BreakablesDescriptor { get; set; }

        public string PedsDescriptor { get; set; }

        public string ZombiesDescriptor { get; set; }

        public string DroneDescriptor { get; set; }

        public string AnimatedProps { get; set; }

        public string MovableObjects { get; set; }

        public string TextureAnimDescriptor { get; set; }

        public string PathFollowers { get; set; }

        public string RadarDescriptor { get; set; }

        public string AmbientSounds { get; set; }

        public string OccluderMesh { get; set; }

        public Vector3 StartPos { get; set; }

        public float StartAngle { get; set; }

        public string SpecialVEnvironments { get; set; }

        public string SpecialVHEnvironments { get; set; }

        public string SpecialVSFXEnvironments { get; set; }

        public float AmbientLight { get; set; }

        public int DynLightTrack { get; set; }

        public Vector3 SunVector { get; set; }

        public Vector3 SunLightColour { get; set; }

        public int FogTrack { get; set; }

        public Vector3 FogTrackColour { get; set; }

        public float FogTrackDist { get; set; }

        public int FogSky { get; set; }

        public Vector3 FogSkyColour { get; set; }

        public float FogSkyNear { get; set; }

        public float FogSkyFar { get; set; }

        public static Track Load(string path)
        {
            DocumentParser file = new(path);
            Track track = new();

            do
            {
                string[] line = file.ReadLine().Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                string setting = line[0];
                string value = string.Join("\t", line, 1, line.Length - 1).Replace("\"", "");

                switch (setting.ToUpper())
                {
                    case "NAME":
                        track.Name = value;
                        break;

                    case "DESCRIPTION":
                        track.Description = value;
                        break;

                    case "TRACK_SELECT_MESH":
                        track.TrackSelectMesh = value;
                        break;

                    case "TRACK_SELECT_RENDER_NODE":
                        track.TrackSelectRenderNode = int.Parse(value);
                        break;

                    case "TRACK_SELECT_TEXTURE":
                        track.TrackSelectTexture = value;
                        break;

                    case "SPLASH_SCREEN_MESH":
                        track.SplashScreenMesh = value;
                        break;

                    case "SPLASH_SCREEN_RENDER_NODE":
                        track.SplashScreenRenderNode = int.Parse(value);
                        break;

                    case "SKY_SPHERE":
                        track.SkySphere = value;
                        break;

                    case "SKY_SPHERE_RENDER_NODE":
                        track.SkySphereRenderNode = int.Parse(value);
                        break;

                    case "WATER_MESH":
                        track.WaterMesh = value;
                        break;

                    case "WATER_RENDER_NODE":
                        track.WaterRenderNode = int.Parse(value);
                        break;

                    case "WATER_LEVEL":
                        track.WaterLevel = float.Parse(value);
                        break;

                    case "STATIC_MESH_DESCRIPTOR":
                        track.StaticMeshDescriptor = value;
                        break;

                    case "BREAKABLES_DESCRIPTOR":
                        track.BreakablesDescriptor = value;
                        break;

                    case "PEDS_DESCRIPTOR":
                        track.PedsDescriptor = value;
                        break;

                    case "ZOMBIES_DESCRIPTOR":
                        track.ZombiesDescriptor = value;
                        break;

                    case "DRONE_DESCRIPTOR":
                        track.DroneDescriptor = value;
                        break;

                    case "ANIMATED_PROPS":
                        track.AnimatedProps = value;
                        break;

                    case "MOVABLE_OBJECTS":
                        track.MovableObjects = value;
                        break;

                    case "TEXTURE_ANIM_DESCRIPTOR":
                        track.TextureAnimDescriptor = value;
                        break;

                    case "PATH_FOLLOWERS":
                        track.PathFollowers = value;
                        break;

                    case "RADAR_DESCRIPTOR":
                        track.RadarDescriptor = value;
                        break;

                    case "AMBIENT_SOUNDS":
                        track.AmbientSounds = value;
                        break;

                    case "OCCLUDER_MESH":
                        track.OccluderMesh = value;
                        break;

                    case "START_POS":
                        track.StartPos = Vector3.Parse(value);
                        break;

                    case "START_ANGLE":
                        track.StartAngle = float.Parse(value);
                        break;

                    case "SPECIALV_ENVIRONMENTS":
                        track.SpecialVEnvironments = value;
                        break;

                    case "SPECIALV_H_ENVIRONMENTS":
                        track.SpecialVHEnvironments = value;
                        break;

                    case "SPECIALV_SFX_ENVIRONMENTS":
                        track.SpecialVSFXEnvironments = value;
                        break;

                    case "AMBIENT_LIGHT":
                        track.AmbientLight = float.Parse(value);
                        break;

                    case "DYN_LIGHT_TRACK":
                        track.DynLightTrack = int.Parse(value);
                        break;

                    case "SUN_VECTOR":
                        track.SunVector = Vector3.Parse(value);
                        break;

                    case "SUN_LIGHT_COLOUR":
                        track.SunLightColour = Vector3.Parse(value);
                        break;

                    case "FOG_TRACK":
                        track.FogTrack = int.Parse(value);
                        break;

                    case "FOG_TRACK_COLOUR":
                        track.FogTrackColour = Vector3.Parse(value);
                        break;

                    case "FOG_TRACK_DIST":
                        track.FogTrackDist = float.Parse(value);
                        break;

                    case "FOG_SKY":
                        track.FogSky = int.Parse(value);
                        break;

                    case "FOG_SKY_COLOUR":
                        track.FogSkyColour = Vector3.Parse(value);
                        break;

                    case "FOG_SKY_NEAR":
                        track.FogSkyNear = float.Parse(value);
                        break;

                    case "FOG_SKY_FAR":
                        track.FogSkyFar = float.Parse(value);
                        break;

                    default:
                        System.Diagnostics.Debug.WriteLine($"Unknown setting: {setting}");
                        break;
                }
            } while (!file.EOF);

            return track;
        }
    }
}
