using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToxicRagers.CarmageddonMobile.Formats
{
	public class CareerModeTxt
	{
		public List<CareerModeTxtItem> Items { get; } = new List<CareerModeTxtItem>();

		public static CareerModeTxt Load(string fileName)
		{
			if (!File.Exists(fileName))
			{
				throw new FileNotFoundException($"Could not find Career Mode tx at: {fileName}");
			}

			CareerModeTxt txt = new CareerModeTxt();
			string[] allLines = File.ReadAllText(fileName).Split(new []{'\n','\r'}, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 1; i < allLines.Length; i++)
			{
				txt.Items.Add(CareerModeTxtItem.Load(allLines[i]));
			}

			return txt;
		}

		public CareerModeTxtItem[] FindLevel(string levelName)
		{
			if (!Items.Any(i =>
				    i.Level.ToUpper() == levelName.ToUpper()))
			{
				return null;
			}
			return Items.Where(i =>
				i.Level.ToUpper() == levelName.ToUpper()).ToArray();
		}
		public CareerModeTxtItem FindLevel(string levelName, string levelTxt)
		{
			return Items.FirstOrDefault(i =>
				i.Level.ToUpper() == levelName.ToUpper() && i.LevelTxt.ToUpper() == levelTxt.ToUpper());
		}
		public void Save(string fileName)
		{

			string content = "Track\t\t\tText1\tText2\tText3\tTrack Unlock\tVehicle Unlock\tEvent unlock\tSpecial code\tRace type\tLaps\tAI skill\tAI Cars\tNum AI\tBoss\tRequired\tWin Bonus Laps\tWin Bonus peds\tWin Bonus opponents\tStart Time\tLap length\tPeds\n";

			for (int i = 0; i < Items.Count; i++)
			{
				content += $"{Items[i]}\n";
			}

			File.WriteAllText(fileName, content);
		}
	}

	public class CareerModeTxtItem
	{

		public string Level { get; set; }
		public string LevelTxt { get; set; }
		public string LevelNameKey { get; set; }
		public string LevelDescriptionKey { get; set; }
		public string EnvironmentNameKey { get; set; }
		public string LocalisationTextKey4 { get; set; }
		public string TrackUnlock { get; set; }
		public string VehicleUnlock { get; set; }
		public string EventUnlock { get; set; }
		public int SpecialCode { get; set; }	
		public string GameMode { get; set; }
		public int Laps { get; set; }
		public int AISkill { get; set; }
		public int AICars { get; set; }
		public int NumAI { get; set; }
		public string Boss { get; set; }
		public int RequiredCredits { get; set; }
		public CareerModeDifficultySetting WinBonusLaps { get; set; }
		public CareerModeDifficultySetting WinBonusPeds { get; set; }
		public CareerModeDifficultySetting WinBonusWasted { get; set; }
		public CareerModeDifficultySetting StartTime { get; set; }
		public float LapLength { get; set; }
		public int NumPeds { get; set; }

		public static CareerModeTxtItem Load(string row)
		{
			string[] splitRow = row.Split('\t');

			CareerModeTxtItem item = new CareerModeTxtItem();
			item.Level = splitRow[0];
			item.LevelTxt = splitRow[1];
			item.LevelNameKey = splitRow[2];
			item.LevelDescriptionKey = splitRow[3];
			item.EnvironmentNameKey = splitRow[4];
			item.LocalisationTextKey4 = splitRow[5];
			item.TrackUnlock = splitRow[6];
			item.VehicleUnlock = splitRow[7];
			item.EventUnlock = splitRow[8];
			item.SpecialCode = int.Parse(splitRow[9]);
			item.GameMode = splitRow[10];
			item.Laps = int.Parse(splitRow[11]);
			item.AISkill = int.Parse(splitRow[12]);
			item.AICars = int.Parse(splitRow[13]);
			item.NumAI = int.Parse(splitRow[14]);
			item.Boss = splitRow[15];
			item.RequiredCredits = int.Parse(splitRow[16]);
			item.WinBonusLaps = new CareerModeDifficultySetting(splitRow[17]);
			item.WinBonusPeds = new CareerModeDifficultySetting(splitRow[18]);
			item.WinBonusWasted = new CareerModeDifficultySetting(splitRow[19]);
			item.StartTime = new CareerModeDifficultySetting(splitRow[20]);
			item.LapLength = float.Parse(splitRow[16]);
			item.NumPeds = int.Parse(splitRow[16]);
			return item;
		}

		public override string ToString()
		{
			return $"{Level}\t" +
				$"{LevelTxt}\t" +
				$"{LevelNameKey}\t" +
				$"{LevelDescriptionKey}\t" +
				$"{EnvironmentNameKey}\t" +
				$"{LocalisationTextKey4}\t" +
				$"{TrackUnlock}\t" +
				$"{VehicleUnlock}\t" +
				$"{EventUnlock}\t" +
				$"{SpecialCode}\t" +
				$"{GameMode}\t" +
				$"{Laps}\t" +
				$"{AISkill}\t" +
				$"{AICars}\t" +
				$"{NumAI}\t" +
				$"{Boss}\t" +
				$"{RequiredCredits}\t" +
				$"\"{WinBonusLaps}\"\t" +
				$"\"{WinBonusPeds}\"\t" +
				$"\"{WinBonusWasted}\"\t" +
				$"\"{StartTime}\"\t" +
				$"{LapLength}\t" +
				$"{NumPeds}\t";

		}
	}

	public class CareerModeDifficultySetting
	{
		public int Easy { get; set; }
		public int Medium { get; set; }
		public int Hard { get; set; }

		public CareerModeDifficultySetting(int easy, int medium, int hard)
		{
			Easy = easy;
			Medium = medium;
			Hard = hard;
		}

		public CareerModeDifficultySetting(string value)
		{
			string[] splitValue = value.Replace("\"", "").Split(',');
			Easy = int.Parse(splitValue[0]);
			Medium = int.Parse(splitValue[1]);
			Hard = int.Parse(splitValue[2]);
		}

		public override string ToString()
		{
			return $"{Easy},{Medium},{Hard}";
		}
	}
}
