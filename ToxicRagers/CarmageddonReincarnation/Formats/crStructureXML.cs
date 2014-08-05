using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml;

using ToxicRagers.Helpers;
using ToxicRagers.CarmageddonReincarnation.Helpers;

namespace ToxicRagers.CarmageddonReincarnation.Formats
{
    public enum StructurePartStrutType
    {
        Hub,
        UpperMount,
        Wishbone,
        WishboneMount
    }

    public enum StructurePartStrutWheel
    {
        FL,
        FR,
        RL,
        RR
    }

    public enum StructurePartSnapType
    {
        PointToPointOnOtherPart
    }

    public enum StructurePartRotateType
    {
        InY,
        InZ,
        NamedInX,
        NamedInY,
        NamedInZ,
        PointToLineOnOtherPart,
        PointToPointOnOtherPart,
        PointToPointOnOtherPartWithScaling,
        VibrateZ
    }

    public enum StructurePartRockType
    {
        InX
    }

    public enum StructurePartSlideType
    {
        InY
    }

    public enum StructurePartOscillateType
    {
        InZ
    }

    public enum StructurePhysicsProperty
    {
        FRONT_LEFT_POINT_OF_SUSPENSION,
        FRONT_LEFT_POINT_OF_STEERING,
        FRONT_LEFT_POINT_OF_ROTATION,
        FRONT_LEFT_WHEEL,
        FRONT_RIGHT_POINT_OF_SUSPENSION,
        FRONT_RIGHT_POINT_OF_STEERING,
        FRONT_RIGHT_POINT_OF_ROTATION,
        FRONT_RIGHT_WHEEL,
        LEFT_STEERING,
        REAR_LEFT_POINT_OF_SUSPENSION,
        REAR_LEFT_POINT_OF_ROTATION,
        REAR_LEFT_WHEEL,
        REAR_RIGHT_POINT_OF_SUSPENSION,
        REAR_RIGHT_POINT_OF_ROTATION,
        REAR_RIGHT_WHEEL,
        RIGHT_STEERING,
        STEERING_WHEEL
    }

    public enum StructurePartLiveType
    {
        Axle,
        Axle_Hub,
        Axle_TrailingArm,
        Axle_TrailingArmMount
    }

    public enum StructurePartWishboneType
    {
        Hub,
        Lower,
        MountLowerFL,
        MountUpperFL,
        MountLowerFR,
        MountUpperFR,
        MountLowerRL,
        MountUpperRL,
        MountLowerRR,
        MountUpperRR,
        Upper
    }

    public enum StructurePartVariable
    {
        AIR_BRAKE,
        CONSTANT_OVER_TIME,
        ENGINE_CRANK_ANGLE,
        ENGINE_NORMALISED_RPM,
        GEARBOX_OUTPUT_ANGLE,
        SPEED_DEPENDENT_AEROFOIL,
        SPEED_DEPENDENT_AEROFOIL_2,
        SPEED_OVER_TIME,
        STEERING,
        WHEEL_ROTATION_FL,
        WHEEL_ROTATION_FR,
        WHEEL_ROTATION_RL,
        WHEEL_ROTATION_RR
    }

    public enum StructurePartLightType
    {
        BRAKE_LIGHT,
        HEAD_LIGHT,
        REVERSE_LIGHT,
        SIREN_LIGHT,
        STROBE1_LIGHT,
        STROBE2_LIGHT,
        TAIL_LIGHT
    }

    public enum StructurePartWeaponSpeed
    {
        CONSTANT
    }

    public enum StructurePartWeaponType 
    {
        Accessory,
        Ped,
        Vehicle
    }

    public class Structure
    {
        StructureCharacteristics characteristics;
        StructurePart root;

        public StructureCharacteristics Characteristics
        {
            get { return characteristics; }
            set { characteristics = value; }
        }

        public StructurePart Root
        {
            get { return root; }
            set { root = value; }
        }

        public static Structure Load(string path)
        {
            Structure structure = new Structure();

            using (var xml = new XMLParser(path, "STRUCTURE"))
            {
                structure.characteristics = new StructureCharacteristics(xml.GetNode("CHARACTERISTICS").FirstChild.InnerText);
                structure.root = new StructurePart(xml.GetNode("ROOT"));
            }

            return structure;
        }
    }

    public class StructurePart
    {
        string name;
        List<StructurePart> parts;
        List<StructureWeld> welds;
        Single crushability;
        Single stiffness;
        Color driverBoxVertexColour;
        Single vehicleSimpleWeapon;
        Single resiliance;
        List<StructurePartStrut> struts;
        List<StructurePhysicsProperty> physicsProperties;
        string shape;
        Single restitution;
        List<StructurePartSnap> snaps;
        List<StructurePartRotate> rotates;
        List<StructurePartLive> live;
        Single mass;
        string crushDamageSoundSubCat;
        List<StructurePartLight> lights;
        List<StructurePartCrushDamageMaterial> crushDamageMaterials;
        List<StructurePartCrushDamageEmitter> crushDamageEmitters;
        List<StructurePartRock> rocks;
        List<StructurePartWeapon> weapons;
        List<StructurePartWishbone> wishbones;
        bool driverEjectionSmash;
        string soundConfigFile;
        List<StructurePartDetachEmitter> detachPartEmitters;
        List<StructurePartDetachEmitter> detachParentEmitters;
        List<StructurePartSlide> slides;
        bool alwaysJointed;
        List<StructurePartOscillate> oscillates;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public List<StructurePart> Parts
        {
            get { return parts; }
            set { parts = value; }
        }

        public List<StructureWeld> Welds
        {
            get { return welds; }
            set { welds = value; }
        }

        public Single Crushability
        {
            get { return crushability; }
            set { crushability = value; }
        }

        public Single Stiffness
        {
            get { return stiffness; }
            set { stiffness = value; }
        }

        public Color DriverBoxVertexColour
        {
            get { return driverBoxVertexColour; }
            set { driverBoxVertexColour = value; }
        }

        public Single VehicleSimpleWeapon
        {
            get { return vehicleSimpleWeapon; }
            set { vehicleSimpleWeapon = value; }
        }

        public Single Resiliance
        {
            get { return resiliance; }
            set { resiliance = value; }
        }

        public List<StructurePartStrut> Struts
        {
            get { return struts; }
            set { struts = value; }
        }

        public List<StructurePhysicsProperty> PhysicsProperties
        {
            get { return physicsProperties; }
            set { physicsProperties = value; }
        }

        public string Shape
        {
            get { return shape; }
            set { shape = value; }
        }

        public Single Restitution
        {
            get { return restitution; }
            set { restitution = value; }
        }

        public List<StructurePartSnap> Snaps
        {
            get { return snaps; }
            set { snaps = value; }
        }

        public List<StructurePartRotate> Rotates
        {
            get { return rotates; }
            set { rotates = value; }
        }

        public List<StructurePartLive> Live
        {
            get { return live; }
            set { live = value; }
        }

        public Single Mass
        {
            get { return mass; }
            set { mass = value; }
        }

        public string CrushDamageSoundSubCat
        {
            get { return crushDamageSoundSubCat; }
            set { crushDamageSoundSubCat = value; }
        }

        public List<StructurePartLight> Lights
        {
            get { return lights; }
            set { lights = value; }
        }

        public List<StructurePartCrushDamageMaterial> CrushDamageMaterials
        {
            get { return crushDamageMaterials; }
            set { crushDamageMaterials = value; }
        }

        public List<StructurePartCrushDamageEmitter> CrushDamageEmitters
        {
            get { return crushDamageEmitters; }
            set { crushDamageEmitters = value; }
        }

        public List<StructurePartRock> Rocks
        {
            get { return rocks; }
            set { rocks = value; }
        }

        public List<StructurePartWeapon> Weapons
        {
            get { return weapons; }
            set { weapons = value; }
        }

        public List<StructurePartWishbone> Wishbones
        {
            get { return wishbones; }
            set { wishbones = value; }
        }

        public bool DriverEjectionSmash
        {
            get { return driverEjectionSmash; }
            set { driverEjectionSmash = value; }
        }

        public string SoundConfigFile
        {
            get { return soundConfigFile; }
            set { soundConfigFile = value; }
        }

        public List<StructurePartDetachEmitter> DetachPartEmitters
        {
            get { return detachPartEmitters; }
            set { detachPartEmitters = value; }
        }

        public List<StructurePartDetachEmitter> DetachParentEmitters
        {
            get { return detachParentEmitters; }
            set { detachParentEmitters = value; }
        }

        public List<StructurePartSlide> Slides
        {
            get { return slides; }
            set { slides = value; }
        }

        public bool AlwaysJointed
        {
            get { return alwaysJointed; }
            set { alwaysJointed = value; }
        }

        public List<StructurePartOscillate> Oscillates
        {
            get { return oscillates; }
            set { oscillates = value; }
        }

        public StructurePart(XmlNode node)
        {
            parts = new List<StructurePart>();
            welds = new List<StructureWeld>();
            struts = new List<StructurePartStrut>();
            snaps = new List<StructurePartSnap>();
            rotates = new List<StructurePartRotate>();
            live = new List<StructurePartLive>();
            lights = new List<StructurePartLight>();
            rocks = new List<StructurePartRock>();
            weapons = new List<StructurePartWeapon>();
            wishbones = new List<StructurePartWishbone>();
            slides = new List<StructurePartSlide>();
            oscillates = new List<StructurePartOscillate>();
            crushDamageMaterials = new List<StructurePartCrushDamageMaterial>();
            crushDamageEmitters = new List<StructurePartCrushDamageEmitter>();
            detachPartEmitters = new List<StructurePartDetachEmitter>();
            detachParentEmitters = new List<StructurePartDetachEmitter>();
            physicsProperties = new List<StructurePhysicsProperty>();

            foreach (XmlAttribute attribute in node.Attributes)
            {
                switch (attribute.Name)
                {
                    case "name":
                        this.name = attribute.Value;
                        break;

                    default:
                        throw new NotImplementedException("Unknown Part attribute: " + attribute.Name);
                }
            }

            foreach (XmlNode data in node.ChildNodes)
            {
                switch (data.NodeType)
                {
                    case XmlNodeType.CDATA:
                        var lines = data.InnerText.Split('\r', '\n').Select(str => str.Trim())
                                                                    .Where(str => str != string.Empty)
                                                                    .ToArray();

                        for (int i = 0; i < lines.Length; i++)
                        {
                            var line = lines[i].Trim();
                            if (line.Length > 1 && line.Substring(0, 2) == "--") { continue; }

                            var c = line.Split(':', '(', ',', ')').Select(str => str.Trim())
                                                                  .Where(str => str != string.Empty)
                                                                  .ToArray();

                            if (c.Length == 0) { continue; }
                            if (c[0] != "CDamageParameters") { throw new ArgumentOutOfRangeException(string.Format("{0} was unexpected, expected CDamageParameters", c[0])); }

                            switch (c[1])
                            {
                                case "Set_Crushability":
                                    this.crushability = c[2].ToSingle();
                                    break;

                                case "Set_Stiffness":
                                    this.stiffness = c[2].ToSingle();
                                    break;

                                case "Set_DriverBoxVertexColour":
                                    this.driverBoxVertexColour = Color.FromArgb(int.Parse(c[2]), int.Parse(c[3]), int.Parse(c[4]), int.Parse(c[5]));
                                    break;

                                case "Add_VehicleSimpleWeapon":
                                    this.vehicleSimpleWeapon = c[2].ToSingle();
                                    break;

                                case "Set_Resiliance":
                                    this.resiliance = c[2].ToSingle();
                                    break;
			
                                case "Set_PreIK_StrutWishboneMountFL":
			                    case "Set_PreIK_StrutWishboneMountFR":
                                case "Set_PreIK_StrutWishboneMountRL":
                                case "Set_PreIK_StrutWishboneMountRR":
                                case "Set_PreIK_StrutUpperMountFL":
                                case "Set_PreIK_StrutUpperMountFR":
                                case "Set_PreIK_StrutUpperMountRL":
                                case "Set_PreIK_StrutUpperMountRR":
                                    this.struts.Add(new StructurePartStrut(int.Parse(c[2]), c[3].ToSingle(), c[4].ToSingle(), c[5].ToSingle(), c[1].Replace("Set_PreIK_Strut", "")));
                                    break;

                                case "Set_PreIK_StrutWishbone":
                                    this.struts.Add(new StructurePartStrut(int.Parse(c[2]), int.Parse(c[3]), c[4].ToSingle(), c[5].ToSingle(), c[6].ToSingle(), c[7].ToSingle(), c[8].ToSingle(), c[9].ToSingle(), c[1].Replace("Set_PreIK_Strut", "")));
                                    break;

                                case "Set_PreIK_StrutHub":
                                    this.struts.Add(new StructurePartStrut(int.Parse(c[2]), int.Parse(c[3]), c[4].ToSingle(), c[5].ToSingle(), c[6].ToSingle(), c[7].ToSingle(), c[8].ToSingle(), c[9].ToSingle(), c[10].ToSingle(), c[11].ToSingle(), c[12].ToSingle(), c[1].Replace("Set_PreIK_Strut", "")));
                                    break;

                                case "Add_PhysicsProperty":
                                    this.physicsProperties.Add(c[2].Replace("\"", "").Trim().ToEnum<StructurePhysicsProperty>());
                                    break;

                                case "Set_ShapeType":
                                    this.shape = c[2].Replace("\"", "");
                                    break;

                                case "Set_Restitution":
                                    this.restitution = c[2].ToSingle();
                                    break;

                                case "Set_PostIK_SnapPointToPointOnOtherPart":
                                    this.snaps.Add(new StructurePartSnap(c[2].ToSingle(), c[3].ToSingle(), c[4].ToSingle(), c[5], c[6].ToSingle(), c[7].ToSingle(), c[8].ToSingle(), c[1].Replace("Set_PostIK_Snap", "")));
                                    break;

                                case "Set_PostIK_RotatePointToPointOnOtherPart":
                                case "Set_PostIK_RotatePointToPointOnOtherPartWithScaling":
                                    this.rotates.Add(new StructurePartRotate(c[2].ToSingle(), c[3].ToSingle(), c[4].ToSingle(), c[5], c[6].ToSingle(), c[7].ToSingle(), c[8].ToSingle(), c[1].Replace("Set_PostIK_Rotate", "")));
                                    break;

                                case "Set_PostIK_RotatePointToLineOnOtherPart":
                                    this.rotates.Add(new StructurePartRotate(c[2].ToSingle(), c[3].ToSingle(), c[4].ToSingle(), c[5], c[6].ToSingle(), c[7].ToSingle(), c[8].ToSingle(), c[9].ToSingle(), c[10].ToSingle(), c[11].ToSingle(), c[1].Replace("Set_PostIK_Rotate", "")));
                                    break;

                                case "Set_PostIK_RotateInY":
                                case "Set_PostIK_RotateInZ":
                                    this.rotates.Add(new StructurePartRotate(c[2].Replace("\"", "").Trim().ToEnum<StructurePartVariable>(), c[3].ToSingle(), c[1].Replace("Set_PostIK_Rotate", "")));
                                    break;

                                case "Set_PostIK_RotateVibrateZ":
                                    this.rotates.Add(new StructurePartRotate(c[2].Replace("\"", "").Trim().ToEnum<StructurePartVariable>(), c[3].ToSingle(), c[4].ToSingle(), c[5].ToSingle(), c[6].ToSingle(), c[7].ToSingle(), c[8].ToSingle(), c[9].ToSingle(), c[10].ToSingle(), c[11].ToSingle(), c[1].Replace("Set_PostIK_Rotate", "")));
                                    break;

                                case "Add_PostIK_NamedRotateInX":
                                case "Add_PostIK_NamedRotateInY":
                                case "Add_PostIK_NamedRotateInZ":
                                    this.rotates.Add(new StructurePartRotate(c[2].Replace("\"", "").Trim().ToEnum<StructurePartVariable>(), c[3].ToSingle(), c[4], c[1].Replace("Add_PostIK_NamedRotate", "Named")));
                                    break;

                                case "Set_PreIK_LiveAxle":
                                case "Set_PreIK_LiveAxle_Hub":
                                case "Set_PreIK_LiveAxle_TrailingArmMount":
                                    this.live.Add(new StructurePartLive(int.Parse(c[2]), c[3].ToSingle(), c[4].ToSingle(), c[5].ToSingle(), c[1].Replace("Set_PreIK_Live", "")));
                                    break;

                                case "Set_PreIK_LiveAxle_TrailingArm":
                                    this.live.Add(new StructurePartLive(int.Parse(c[2]), c[3].ToSingle(), c[4].ToSingle(), c[5].ToSingle(), c[6].ToSingle(), c[7].ToSingle(), c[8].ToSingle(), c[1].Replace("Set_PreIK_Live", "")));
                                    break;

                                case "Set_Mass":
                                    this.mass = c[2].ToSingle();
                                    break;

                                case "Set_CrushDamageSoundSubCat":
                                    this.crushDamageSoundSubCat = c[2].Replace("\"", "");
                                    break;

                                case "Add_FunctionalLight":
                                    this.lights.Add(new StructurePartLight(c[2].Replace("\"", "").Trim().ToEnum<StructurePartLightType>(), c[3]));
                                    break;

                                case "Add_CrushDamageMaterial":
                                    this.crushDamageMaterials.Add(new StructurePartCrushDamageMaterial(int.Parse(c[2]), c[3], c[4]));
                                    break;

                                case "Add_CrushDamageEmitter":
                                    this.crushDamageEmitters.Add(new StructurePartCrushDamageEmitter(int.Parse(c[2]), c[3], c[4].ToSingle(), c[5].ToSingle(), c[6].ToSingle()));
                                    break;

                                case "Set_PostIK_RockInX":
                                    this.rocks.Add(new StructurePartRock(c[2].Replace("\"", "").Trim().ToEnum<StructurePartVariable>(), c[3].ToSingle(), c[4].ToSingle(), c[5].ToSingle(), c[6].ToSingle(), c[7].ToSingle(), c[1].Replace("Set_PostIK_Rock", "")));
                                    break;

                                case "Add_PedWeapon":
                                case "Add_VehicleWeapon":
                                case "Add_AccessoryWeapon":
                                    this.weapons.Add(new StructurePartWeapon(c[2], c[3].Replace("\"", "").Trim().ToEnum<StructurePartWeaponSpeed>(), c[4].ToSingle(), c[5].ToSingle(), c[6].ToSingle(), c[1].Replace("Add_", "").Replace("Weapon", "")));
                                    break;

                                case "Set_PreIK_WishboneMountLowerFL":
                                case "Set_PreIK_WishboneMountUpperFL":
                                case "Set_PreIK_WishboneMountLowerFR":
                                case "Set_PreIK_WishboneMountUpperFR":
                                case "Set_PreIK_WishboneMountLowerRL":
                                case "Set_PreIK_WishboneMountUpperRL":
                                case "Set_PreIK_WishboneMountLowerRR":
                                case "Set_PreIK_WishboneMountUpperRR":
                                    this.wishbones.Add(new StructurePartWishbone(int.Parse(c[2]), c[3].ToSingle(), c[4].ToSingle(), c[5].ToSingle(), c[1].Replace("Set_PreIK_Wishbone", "")));
                                    break;

                                case "Set_PreIK_WishboneLower":
                                case "Set_PreIK_WishboneUpper":
                                    this.wishbones.Add(new StructurePartWishbone(int.Parse(c[2]), int.Parse(c[3]), c[4].ToSingle(), c[5].ToSingle(), c[6].ToSingle(), c[7].ToSingle(), c[8].ToSingle(), c[9].ToSingle(), c[1].Replace("Set_PreIK_Wishbone", "")));
                                    break;

                                case "Set_PreIK_WishboneHub":
                                    this.wishbones.Add(new StructurePartWishbone(int.Parse(c[2]), int.Parse(c[3]), c[4].ToSingle(), c[5].ToSingle(), c[6].ToSingle(), c[7].ToSingle(), c[8].ToSingle(), c[9].ToSingle(), c[10].ToSingle(), c[11].ToSingle(), c[12].ToSingle(), c[1].Replace("Set_PreIK_Wishbone", "")));
                                    break;

                                case "Set_DriverEjectionSmash":
                                    this.driverEjectionSmash = (c[2] == "true");
                                    break;

                                case "Set_SoundConfigFile":
                                    this.soundConfigFile = c[2].Replace("\"", "").Trim();
                                    break;

                                case "Add_DetachPartEmitter":
                                    this.detachPartEmitters.Add(new StructurePartDetachEmitter(c[2], c[3].ToSingle(), c[4].ToSingle(), c[5].ToSingle(), c[6].ToSingle()));
                                    break;

                                case "Add_DetachParentEmitter":
                                    this.detachParentEmitters.Add(new StructurePartDetachEmitter(c[2], c[3].ToSingle(), c[4].ToSingle(), c[5].ToSingle(), c[6].ToSingle()));
                                    break;

                                case "Set_PostIK_SlideInY":
                                    this.slides.Add(new StructurePartSlide(c[2].Replace("\"", "").Trim().ToEnum<StructurePartVariable>(), c[3].ToSingle(), c[1].Replace("Set_PostIK_Slide", "")));
                                    break;

                                case "Set_AlwaysJointed":
                                    this.alwaysJointed = (c[2] == "true");
                                    break;

                                case "Set_PostIK_OscillateInZ":
                                    this.oscillates.Add(new StructurePartOscillate(c[2].Replace("\"", "").Trim().ToEnum<StructurePartVariable>(), c[3].ToSingle(), c[3].ToSingle(), c[1].Replace("Set_PostIK_Oscillate", "")));
                                    break;

                                default:
                                    throw new NotImplementedException("Unknown CDamageParameters method: " + c[1]);
                            }
                        }
                        break;

                    case XmlNodeType.Element:
                        switch (data.Name)
                        {
                            case "PART":
                                parts.Add(new StructurePart(data));
                                break;

                            case "WELD":
                                welds.Add(new StructureWeld(data));
                                break;

                            default:
                                throw new NotImplementedException("Unknown Element of PART: " + data.Name);
                        }
                        break;
                }
            }
        }
    }

    public class StructurePartStrut
    {
        StructurePartStrutType strutType;
        StructurePartStrutWheel wheel;
        int wheelIndex;
        int pivotAxis;
        Vector3 pivot;
        Vector3 inboardPivot;
        Vector3 outboardPivot;
        Vector3 upperPivot;
        Vector3 lowerPivot;
        Vector3 wheelPosition;

        public StructurePartStrutType StrutType
        {
            get { return strutType; }
            set { strutType = value; }
        }

        public StructurePartStrutWheel Wheel
        {
            get { return wheel; }
            set { wheel = value; }
        }

        public int WheelIndex
        {
            get { return wheelIndex; }
            set { wheelIndex = value; }
        }

        public int PivotAxis
        {
            get { return pivotAxis; }
            set { pivotAxis = value; }
        }

        public Vector3 Pivot
        {
            get { return pivot; }
            set { pivot = value; }
        }

        public Vector3 InboardPivot
        {
            get { return inboardPivot; }
            set { inboardPivot = value; }
        }

        public Vector3 OutboardPivot
        {
            get { return outboardPivot; }
            set { outboardPivot = value; }
        }

        public Vector3 UpperPivot
        {
            get { return upperPivot; }
            set { upperPivot = value; }
        }

        public Vector3 LowerPivot
        {
            get { return lowerPivot; }
            set { lowerPivot = value; }
        }

        public Vector3 WheelPosition
        {
            get { return wheelPosition; }
            set { wheelPosition = value; }
        }

        public StructurePartStrut(int pivotAxis, Single x, Single y, Single z, string typeAndWheel)
        {
            this.pivotAxis = pivotAxis;
            this.pivot = new Vector3(x, y, z);

            this.strutType = typeAndWheel.Substring(0, typeAndWheel.Length - 2).ToEnum<StructurePartStrutType>();
            this.wheel = typeAndWheel.Substring(typeAndWheel.Length - 2).ToEnum<StructurePartStrutWheel>();

            this.wheelIndex = (int)this.wheel;
        }

        public StructurePartStrut(int wheelIndex, int pivotAxis, Single ix, Single iy, Single iz, Single ox, Single oy, Single oz, string type)
        {
            this.wheelIndex = wheelIndex;
            this.pivotAxis = pivotAxis;
            this.inboardPivot = new Vector3(ix, iy, iz);
            this.outboardPivot = new Vector3(ox, oy, oz);

            this.strutType = type.ToEnum<StructurePartStrutType>();
            this.wheel = (StructurePartStrutWheel)wheelIndex;
        }

        public StructurePartStrut(int wheelIndex, int pivotAxis, Single ux, Single uy, Single uz, Single lx, Single ly, Single lz, Single wx, Single wy, Single wz, string type)
        {
            this.wheelIndex = wheelIndex;
            this.pivotAxis = pivotAxis;
            this.upperPivot = new Vector3(ux, uy, uz);
            this.lowerPivot = new Vector3(lx, ly, lz);
            this.wheelPosition = new Vector3(wx, wy, wz);

            this.strutType = type.ToEnum<StructurePartStrutType>();
            this.wheel = (StructurePartStrutWheel)wheelIndex;
        }
    }

    public class StructurePartSnap
    {
        StructurePartSnapType snapType;
        Vector3 thisPoint;
        Vector3 partPoint;
        string partName;

        public StructurePartSnapType SnapType
        {
            get { return snapType; }
            set { snapType = value; }
        }

        public Vector3 ThisPoint
        {
            get { return thisPoint; }
            set { thisPoint = value; }
        }

        public Vector3 PartPoint
        {
            get { return partPoint; }
            set { partPoint = value; }
        }

        public string PartName
        {
            get { return partName; }
            set { partName = value; }
        }

        public StructurePartSnap(Single tx, Single ty, Single tz, string partName, Single ox, Single oy, Single oz, string type)
        {
            this.thisPoint = new Vector3(tx, ty, tz);
            this.partPoint = new Vector3(ox, oy, oz);
            this.partName = partName.Replace("\"", "").Trim();

            this.snapType = type.ToEnum<StructurePartSnapType>();
        }
    }

    public class StructurePartRotate
    {
        StructurePartRotateType rotateType;
        Vector3 thisPoint;
        Vector3 partPoint;
        string partName;
        Single angle;
        StructurePartVariable trackVariable;
        Single minFreq;
        Single maxFreq;
        Single randFreq;
        Single minDeg;
        Single maxDeg;
        Single randDeg;
        Vector3 partLine;

        public StructurePartRotateType RotateType
        {
            get { return rotateType; }
            set { rotateType = value; }
        }

        public Vector3 ThisPoint
        {
            get { return thisPoint; }
            set { thisPoint = value; }
        }

        public Vector3 PartPoint
        {
            get { return partPoint; }
            set { partPoint = value; }
        }

        public string PartName
        {
            get { return partName; }
            set { partName = value; }
        }

        public Single Angle
        {
            get { return angle; }
            set { angle = value; }
        }

        public StructurePartVariable TrackVariable
        {
            get { return trackVariable; }
            set { trackVariable = value; }
        }

        public Single MinFreq
        {
            get { return minFreq; }
            set { minFreq = value; }
        }

        public Single MaxFreq
        {
            get { return maxFreq; }
            set { maxFreq = value; }
        }

        public Single RandFreq
        {
            get { return randFreq; }
            set { randFreq = value; }
        }

        public Single MinDeg
        {
            get { return minDeg; }
            set { minDeg = value; }
        }

        public Single MaxDeg
        {
            get { return maxDeg; }
            set { maxDeg = value; }
        }

        public Single RandDeg
        {
            get { return randDeg; }
            set { randDeg = value; }
        }

        public Vector3 PartLine
        {
            get { return partLine; }
            set { partLine = value; }
        }

        public StructurePartRotate(Single tx, Single ty, Single tz, string partName, Single ox, Single oy, Single oz, string type)
        {
            this.thisPoint = new Vector3(tx, ty, tz);
            this.partPoint = new Vector3(ox, oy, oz);
            this.partName = partName.Replace("\"", "").Trim();

            this.rotateType = type.ToEnum<StructurePartRotateType>();
        }

        public StructurePartRotate(StructurePartVariable track, Single angle, string type)
        {
            this.trackVariable = track;
            this.angle = angle;

            this.rotateType = type.ToEnum<StructurePartRotateType>();
        }

        public StructurePartRotate(StructurePartVariable track, Single minFreq, Single maxFreq, Single randFreq, Single minDeg, Single maxDeg, Single randDeg, Single x, Single y, Single z, string type)
        {
            this.trackVariable = track;
            this.minFreq = minFreq;
            this.maxFreq = maxFreq;
            this.randFreq = randFreq;
            this.minDeg = minDeg;
            this.maxDeg = maxDeg;
            this.randDeg = randDeg;
            this.partPoint = new Vector3(x, y, z);

            this.rotateType = type.ToEnum<StructurePartRotateType>();
        }

        public StructurePartRotate(StructurePartVariable track, Single angle, string partName, string type)
        {
            this.trackVariable = track;
            this.partName = partName.Replace("\"", "").Trim();
            this.angle = angle;

            this.rotateType = type.ToEnum<StructurePartRotateType>();
        }

        public StructurePartRotate(Single tx, Single ty, Single tz, string partName, Single ox, Single oy, Single oz, Single lx, Single ly, Single lz, string type)
        {
            this.thisPoint = new Vector3(tx, ty, tz);
            this.partPoint = new Vector3(ox, oy, oz);
            this.partLine = new Vector3(lx, ly, lz);
            this.partName = partName.Replace("\"", "").Trim();

            this.rotateType = type.ToEnum<StructurePartRotateType>();
        }
    }

    public class StructurePartLive
    {
        StructurePartLiveType liveType;
        int wheelIndex;
        Vector3 rightMountingPoint;
        Vector3 mountingPivot;
        Vector3 axlePivot;

        public StructurePartLiveType LiveType
        {
            get { return liveType; }
            set { liveType = value; }
        }

        public int WheelIndex
        {
            get { return wheelIndex; }
            set { wheelIndex = value; }
        }

        public Vector3 RightMountingPoint
        {
            get { return rightMountingPoint; }
            set { rightMountingPoint = value; }
        }

        public Vector3 MountingPivot
        {
            get { return mountingPivot; }
            set { mountingPivot = value; }
        }

        public Vector3 AxlePivot
        {
            get { return axlePivot; }
            set { axlePivot = value; }
        }

        public StructurePartLive(int wheelIndex, Single mpx, Single mpy, Single mpz, string type)
        {
            this.wheelIndex = wheelIndex;
            this.rightMountingPoint = new Vector3(mpx, mpy, mpz);

            this.liveType = type.ToEnum<StructurePartLiveType>();
        }

        public StructurePartLive(int wheelIndex, Single mpx, Single mpy, Single mpz, Single apx, Single apy, Single apz, string type)
        {
            this.wheelIndex = wheelIndex;
            this.mountingPivot = new Vector3(mpx, mpy, mpz);
            this.axlePivot = new Vector3(apx, apy, apz);

            this.liveType = type.ToEnum<StructurePartLiveType>();
        }
    }

    public class StructurePartRock
    {
        StructurePartRockType rockType;
        StructurePartVariable trackVariable;
        Single a;
        Single b;
        Single c;
        Single d;
        Single e;

        public StructurePartRockType RockType
        {
            get { return rockType; }
            set { rockType = value; }
        }

        public StructurePartVariable TrackVariable
        {
            get { return trackVariable; }
            set { trackVariable = value; }
        }

        public Single A
        {
            get { return a; }
            set { a = value; }
        }

        public Single B
        {
            get { return b; }
            set { b = value; }
        }

        public Single C
        {
            get { return c; }
            set { c = value; }
        }

        public Single D
        {
            get { return d; }
            set { d = value; }
        }

        public Single E
        {
            get { return e; }
            set { e = value; }
        }

        public StructurePartRock(StructurePartVariable track, Single a, Single b, Single c, Single d, Single e, string type)
        {
            this.trackVariable = track;
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            this.e = e;

            this.rockType = type.ToEnum<StructurePartRockType>();
        }
    }

    public class StructurePartWishbone
    {
        StructurePartWishboneType wishboneType;
        int wheelIndex;
        Vector3 position;
        int pivotAxis;
        Vector3 inboardPivot;
        Vector3 outboardPivot;
        Vector3 lowerPivot;
        Vector3 upperPivot;
        Vector3 wheelPosition;

        public StructurePartWishboneType WishboneType
        {
            get { return wishboneType; }
            set { wishboneType = value; }
        }

        public int WheelIndex
        {
            get { return wheelIndex; }
            set { wheelIndex = value; }
        }

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        public int PivotAxis
        {
            get { return pivotAxis; }
            set { pivotAxis = value; }
        }

        public Vector3 InboardPivot
        {
            get { return inboardPivot; }
            set { inboardPivot = value; }
        }

        public Vector3 OutboardPivot
        {
            get { return outboardPivot; }
            set { outboardPivot = value; }
        }

        public Vector3 LowerPivot
        {
            get { return lowerPivot; }
            set { lowerPivot = value; }
        }

        public Vector3 UpperPivot
        {
            get { return upperPivot; }
            set { upperPivot = value; }
        }

        public Vector3 WheelPosition
        {
            get { return wheelPosition; }
            set { wheelPosition = value; }
        }

        public StructurePartWishbone(int wheelIndex, Single x, Single y, Single z, string type)
        {
            this.wheelIndex = wheelIndex;
            this.position = new Vector3(x, y, z);

            this.wishboneType = type.ToEnum<StructurePartWishboneType>();
        }

        public StructurePartWishbone(int wheelIndex, int pivotAxis, Single ix, Single iy, Single iz, Single ox, Single oy, Single oz, string type)
        {
            this.wheelIndex = wheelIndex;
            this.pivotAxis = pivotAxis;
            this.inboardPivot = new Vector3(ix, iy, iz);
            this.outboardPivot = new Vector3(ox, oy, oz);

            this.wishboneType = type.ToEnum<StructurePartWishboneType>();
        }

        public StructurePartWishbone(int wheelIndex, int pivotAxis, Single ux, Single uy, Single uz, Single lx, Single ly, Single lz, Single wx, Single wy, Single wz, string type)
        {
            this.wheelIndex = wheelIndex;
            this.pivotAxis = pivotAxis;
            this.upperPivot = new Vector3(ux, uy, uz);
            this.lowerPivot = new Vector3(lx, ly, lz);
            this.wheelPosition = new Vector3(wx, wy, wz);

            this.wishboneType = type.ToEnum<StructurePartWishboneType>();
        }
    }

    public class StructurePartSlide
    {
        StructurePartSlideType slideType;
        StructurePartVariable trackVariable;
        Single amount;

        public StructurePartSlideType SlideType
        {
            get { return slideType; }
            set { slideType = value; }
        }

        public StructurePartVariable TrackVariable
        {
            get { return trackVariable; }
            set { trackVariable = value; }
        }

        public Single Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        public StructurePartSlide(StructurePartVariable track, Single amount, string type)
        {
            this.trackVariable = track;
            this.amount = amount;

            this.slideType = type.ToEnum<StructurePartSlideType>();
        }
    }

    public class StructurePartOscillate
    {
        StructurePartOscillateType oscillateType;
        StructurePartVariable trackVariable;
        Single a;
        Single b;

        public StructurePartOscillateType OscillateType
        {
            get { return oscillateType; }
            set { oscillateType = value; }
        }

        public StructurePartVariable TrackVariable
        {
            get { return trackVariable; }
            set { trackVariable = value; }
        }

        public Single A
        {
            get { return a; }
            set { a = value; }
        }

        public Single B
        {
            get { return b; }
            set { b = value; }
        }

        public StructurePartOscillate(StructurePartVariable track, Single a, Single b, string type)
        {
            this.trackVariable = track;
            this.a = a;
            this.b = b;

            this.oscillateType = type.ToEnum<StructurePartOscillateType>();
        }
    }

    public class StructurePartWeapon
    {
        StructurePartWeaponType weaponType;
        string weaponName;
        StructurePartWeaponSpeed weaponSpeed;
        Single a;
        Single b;
        Single c;

        public StructurePartWeapon(string weaponName, StructurePartWeaponSpeed weaponSpeed, Single a, Single b, Single c, string type)
        {
            this.weaponName = weaponName.Replace("\"", "").Trim();
            this.weaponSpeed = weaponSpeed;
            this.a = a;
            this.b = b;
            this.c = c;

            this.weaponType = type.ToEnum<StructurePartWeaponType>();
        }
    }

    public class StructurePartLight
    {
        StructurePartLightType lightType;
        string partName;

        public StructurePartLightType LightType
        {
            get { return lightType; }
            set { lightType = value; }
        }

        public string PartName
        {
            get { return partName; }
            set { partName = value; }
        }

        public StructurePartLight(StructurePartLightType lightType, string partName)
        {
            this.lightType = lightType;
            this.partName = partName.Replace("\"", "").Trim();
        }
    }

    public class StructurePartCrushDamageMaterial
    {
        int level;
        string partName;
        string material;

        public int Level
        {
            get { return level; }
            set { level = value; }
        }

        public string PartName
        {
            get { return partName; }
            set { partName = value; }
        }

        public string Material
        {
            get { return material; }
            set { material = value; }
        }

        public StructurePartCrushDamageMaterial(int level, string partName, string material)
        {
            this.level = level;
            this.partName = partName.Replace("\"", "").Trim();
            this.material = material.Replace("\"", "").Trim();
        }
    }

    public class StructurePartCrushDamageEmitter
    {
        int level;
        string emitterName;
        Vector3 position;

        public int Level
        {
            get { return level; }
            set { level = value; }
        }

        public string EmitterName
        {
            get { return emitterName; }
            set { emitterName = value; }
        }

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        public StructurePartCrushDamageEmitter(int level, string emitterName, Single x, Single y, Single z)
        {
            this.level = level;
            this.emitterName = emitterName.Replace("\"", "").Trim();
            this.position = new Vector3(x, y, z);
        }
    }

    public class StructurePartDetachEmitter
    {
        string emitterName;
        Vector3 position;
        Single snapForceFactor;

        public string EmitterName
        {
            get { return emitterName; }
            set { emitterName = value; }
        }

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Single SnapForceFactor
        {
            get { return snapForceFactor; }
            set { snapForceFactor = value; }
        }

        public StructurePartDetachEmitter(string emitterName, Single x, Single y, Single z, Single snapForceFactor)
        {
            this.emitterName = emitterName.Replace("\"", "").Trim();
            this.position = new Vector3(x, y, z);
            this.snapForceFactor = snapForceFactor;
        }
    }

    public class StructureWeld
    {
        string name;
        string partner;
        Color vertexColour;
        Single weakness;
        List<Vector3> partSpaceVertex;
        Single absoluteLimit;
        Single breakPoint;
        int chanceOfFailure;
        List<StructureWeldJoint> joints;
        List<string> gangedBreaks;
        List<StructurePart> parts;

        public string Partner
        {
            get { return partner; }
            set { partner = value; }
        }

        public Color VertexColour
        {
            get { return vertexColour; }
            set { vertexColour = value; }
        }

        public Single Weakness
        {
            get { return weakness; }
            set { weakness = value; }
        }

        public List<Vector3> PartSpaceVertex
        {
            get { return partSpaceVertex; }
            set { partSpaceVertex = value; }
        }

        public Single AbsoluteLimit
        {
            get { return absoluteLimit; }
            set { absoluteLimit = value; }
        }

        public Single Break
        {
            get { return breakPoint; }
            set { breakPoint = value; }
        }

        public List<StructureWeldJoint> Joints
        {
            get { return joints; }
            set { joints = value; }
        }

        public int ChanceOfFailure
        {
            get { return chanceOfFailure; }
            set { chanceOfFailure = value; }
        }

        public List<string> GangedBreaks
        {
            get { return gangedBreaks; }
            set { gangedBreaks = value; }
        }

        public List<StructurePart> Parts
        {
            get { return parts; }
            set { parts = value; }
        }

        public StructureWeld(XmlNode node)
        {
            joints = new List<StructureWeldJoint>();
            parts = new List<StructurePart>();
            partSpaceVertex = new List<Vector3>();
            gangedBreaks = new List<string>();

            foreach (XmlAttribute attribute in node.Attributes)
            {
                switch (attribute.Name)
                {
                    case "name":
                        this.name = attribute.Value;
                        break;

                    case "partner":
                        this.partner = attribute.Value;
                        break;

                    default:
                        throw new NotImplementedException("Unknown Weld attribute: " + attribute.Name);
                }
            }

            foreach (XmlNode data in node.ChildNodes)
            {
                switch (data.NodeType)
                {
                    case XmlNodeType.CDATA:
                        var lines = data.InnerText.Split('\r', '\n').Select(str => str.Trim())
                                                                               .Where(str => str != string.Empty)
                                                                               .ToArray();

                        for (int i = 0; i < lines.Length; i++)
                        {
                            var line = lines[i].Trim();
                            if (line.Substring(0, 2) == "--") { continue; }

                            var c = line.Split(':', '(', ',', ')').Select(str => str.Trim())
                                                                  .Where(str => str != string.Empty)
                                                                  .ToArray();

                            if (c[0] != "CWeldParameters") { throw new ArgumentOutOfRangeException(string.Format("{0} was unexpected, expected CWeldParameters", c[0])); }

                            switch (c[1])
                            {
                                case "Set_VertexColour":
                                    this.vertexColour = Color.FromArgb(int.Parse(c[2]), int.Parse(c[3]), int.Parse(c[4]), int.Parse(c[5]));
                                    break;

                                case "Set_Weakness":
                                    this.weakness = c[2].ToSingle();
                                    break;

                                case "Add_PartSpaceVertex":
                                    this.partSpaceVertex.Add(new Vector3(c[2].ToSingle(), c[3].ToSingle(), c[4].ToSingle()));
                                    break;

                                case "Set_AbsoluteLimit":
                                    this.absoluteLimit = c[2].ToSingle();
                                    break;

                                case "Set_Break":
                                    this.breakPoint = c[2].ToSingle();
                                    break;

                                case "Set_ChanceOfFailure":
                                    this.chanceOfFailure = int.Parse(c[2]);
                                    break;

                                case "Add_GangedBreak":
                                    this.gangedBreaks.Add(c[2].Replace("\"", "").Trim());
                                    break;

                                default:
                                    throw new NotImplementedException("Unknown CWeldParameters method: " + c[1]);
                            }
                        }
                        break;

                    case XmlNodeType.Element:
                        switch (data.Name)
                        {
                            case "JOINT":
                                joints.Add(new StructureWeldJoint(data));
                                break;

                            case "PART":
                                parts.Add(new StructurePart(data));
                                break;

                            default:
                                throw new NotImplementedException("Unknown Element of WELD: " + data.Name);
                        }
                        break;
                }
            }
        }
    }

    public class StructureWeldJoint
    {
        bool hinge;
        bool ballJoint;
        Vector3 jointAxis;
        int minLimit;
        int maxLimit;
        int minLimit2;
        int maxLimit2;
        int minTwistLimit;
        int maxTwistLimit;
        List<StructureWeldJointFlapSpring> flapSprings;
        int weakness;
        Vector3 jointLocation;
        Vector3 jointNormal;

        public bool Hinge
        {
            get { return hinge; }
            set { hinge = value; }
        }

        public bool BallJoint
        {
            get { return ballJoint; }
            set { ballJoint = value; }
        }

        public Vector3 JointAxis
        {
            get { return jointAxis; }
            set { jointAxis = value; }
        }

        public int MinLimit
        {
            get { return minLimit; }
            set { minLimit = value; }
        }

        public int MaxLimit
        {
            get { return maxLimit; }
            set { maxLimit = value; }
        }

        public int MinLimit2
        {
            get { return minLimit2; }
            set { minLimit2 = value; }
        }

        public int MaxLimit2
        {
            get { return maxLimit2; }
            set { maxLimit2 = value; }
        }

        public int MinTwistLimit
        {
            get { return minTwistLimit; }
            set { minTwistLimit = value; }
        }

        public int MaxTwistLimit
        {
            get { return maxTwistLimit; }
            set { maxTwistLimit = value; }
        }

        public List<StructureWeldJointFlapSpring> FlapSprings
        {
            get { return flapSprings; }
            set { flapSprings = value; }
        }

        public int Weakness
        {
            get { return weakness; }
            set { weakness = value; }
        }

        public Vector3 JointLocation
        {
            get { return jointLocation; }
            set { jointLocation = value; }
        }

        public Vector3 JointNormal
        {
            get { return jointNormal; }
            set { jointNormal = value; }
        }

        public StructureWeldJoint(XmlNode node)
        {
            flapSprings = new List<StructureWeldJointFlapSpring>();

            foreach (XmlAttribute attribute in node.Attributes)
            {
                switch (attribute.Name)
                {
                    default:
                        throw new NotImplementedException("Unknown WeldJoint attribute: " + attribute.Name);
                }
            }

            foreach (XmlNode data in node.ChildNodes)
            {
                switch (data.NodeType)
                {
                    case XmlNodeType.CDATA:
                        var lines = data.InnerText.Split('\r', '\n').Select(str => str.Trim())
                                                                               .Where(str => str != string.Empty)
                                                                               .ToArray();

                        for (int i = 0; i < lines.Length; i++)
                        {
                            var line = lines[i].Trim();
                            if (line.Substring(0, 2) == "--") { continue; }

                            var c = line.Split(':', '(', ',', ')').Select(str => str.Trim())
                                                                  .Where(str => str != string.Empty)
                                                                  .ToArray();

                            if (c[0] != "CWeldJointParameters") { throw new ArgumentOutOfRangeException(string.Format("{0} was unexpected, expected CWeldJointParameters", c[0])); }

                            switch (c[1])
                            {
                                case "Set_Hinge":
                                    this.hinge = (c[2] == "true");
                                    break;

                                case "Set_JointAxis":
                                    this.jointAxis = new Vector3(c[2].ToSingle(), c[3].ToSingle(), c[4].ToSingle());
                                    break;

                                case "Set_MinLimit":
                                    this.minLimit = int.Parse(c[2]);
                                    break;

                                case "Set_MaxLimit":
                                    this.maxLimit = int.Parse(c[2]);
                                    break;

                                case "Set_MinLimit2":
                                    this.minLimit2 = int.Parse(c[2]);
                                    break;

                                case "Set_MaxLimit2":
                                    this.maxLimit2 = int.Parse(c[2]);
                                    break;

                                case "Add_FlapSpring":
                                    this.flapSprings.Add(new StructureWeldJointFlapSpring(int.Parse(c[2]), int.Parse(c[2])));
                                    break;

                                case "Set_Weakness":
                                    this.weakness = int.Parse(c[2]);
                                    break;

                                case "Set_JointLocation":
                                    this.jointLocation = new Vector3(c[2].ToSingle(), c[3].ToSingle(), c[4].ToSingle());
                                    break;

                                case "Set_BallJoint":
                                    this.ballJoint = (c[2] == "true");
                                    break;

                                case "Set_JointNormal":
                                    this.jointNormal = new Vector3(c[2].ToSingle(), c[3].ToSingle(), c[4].ToSingle());
                                    break;

                                case "Set_MinTwistLimit":
                                    this.minTwistLimit = int.Parse(c[2]);
                                    break;

                                case "Set_MaxTwistLimit":
                                    this.maxTwistLimit = int.Parse(c[2]);
                                    break;	

                                default:
                                    throw new NotImplementedException("Unknown CWeldJointParameters method: " + c[1]);
                            }
                        }
                        break;

                    case XmlNodeType.Element:
                        switch (data.Name)
                        {
                            default:
                                throw new NotImplementedException("Unknown Element of JOINT: " + data.Name);
                        }
                }
            }
        }
    }

    public class StructureWeldJointFlapSpring
    {
        int a;
        int b;

        public int A
        {
            get { return a; }
            set { a = value; }
        }

        public int B
        {
            get { return b; }
            set { b = value; }
        }

        public StructureWeldJointFlapSpring(int a, int b)
        {
            this.a = a;
            this.b = b;
        }
    }

    public class StructureCharacteristics
    {
        Single defenceGeneral;
        Single defenceAgainstCars;
        Single offence;
        Single wholeBodyDeformationFactor;
        Single valueFactor;
        string powerup;

        public Single DefenceGeneral
        {
            get { return defenceGeneral; }
            set { defenceGeneral = value; }
        }

        public Single DefenceAgainstCars
        {
            get { return defenceAgainstCars; }
            set { defenceAgainstCars = value; }
        }

        public Single Offence
        {
            get { return offence; }
            set { offence = value; }
        }

        public Single WholeBodyDeformationFactor
        {
            get { return wholeBodyDeformationFactor; }
            set { wholeBodyDeformationFactor = value; }
        }

        public Single ValueFactor
        {
            get { return valueFactor; }
            set { valueFactor = value; }
        }

        public string Powerup
        {
            get { return powerup; }
            set { powerup = value; }
        }


        public StructureCharacteristics(string cdata)
        {
            var lines = cdata.Split('\r', '\n').Select(str => str.Trim())
                                               .Where(str => str != string.Empty)
                                               .ToArray();

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (line.Substring(0, 2) == "--") { continue; }

                var c = line.Split(':', '(', ')').Select(str => str.Trim())
                                                 .Where(str => str != string.Empty)
                                                 .ToArray();

                if (c[0] != "CVehicleCharacteristics") { throw new ArgumentOutOfRangeException(string.Format("{0} was unexpected, expected CVehicleCharacteristics", c[0])); }

                switch (c[1])
                {
                    case "Set_DefenceGeneral":
                        this.defenceGeneral = c[2].ToSingle();
                        break;

                    case "Set_DefenceAgainstCars":
                        this.defenceAgainstCars = c[2].ToSingle();
                        break;

                    case "Set_Offence":
                        this.offence = c[2].ToSingle();
                        break;

                    case "Set_WholeBodyDeformationFactor":
                        this.wholeBodyDeformationFactor = c[2].ToSingle();
                        break;

                    case "Set_ValueFactor":
                        this.valueFactor = c[2].ToSingle();
                        break;

                    case "Add_PermanentPowerup":
                        this.powerup = c[2].Replace("\"", "").Trim();
                        break;

                    default:
                        throw new NotImplementedException("Unknown CVehicleCharacteristics method: " + c[1]);
                }
            }
        }
    }
}
