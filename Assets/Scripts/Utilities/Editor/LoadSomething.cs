using UnityEngine;
using UnityEditor;

public enum ETypeClass
{
	class1,
	class2,
}

public class ClassValues : ScriptableObject
{
	public string className;
	public float health;
	public int scoreOnKill;
	public ETypeClass typeClass;
}

public class LoadSomething
{
	//public on web csv of spreadsheet and get link
	static string linkDB = "https://docs.google.com/spreadsheets/d/e/2PACX-1vTULSsC_Nak6x2hkdmU3ZuvYUaDLBJN1yEfkXACESJYvBAPRrhny1TSppkqpeQUJAna4fupmCb3DZHh/pub?gid=103538417&single=true&output=csv";

	//path of every scriptable objects
	static string pathAssets = "DESIGNERS/Enemies";

	[MenuItem("Load CSV/Load Enemies")]
	static void Load()
	{
		LoadCSV.LoadAndUpdate<ClassValues>(linkDB, pathAssets, UpdateAsset);
	}

	static void UpdateAsset(string[] value, ClassValues asset)
	{
		asset.className = value[1];
		asset.health = float.Parse(value[2]);
		asset.scoreOnKill = int.Parse(value[3]);
		asset.typeClass = StringToBehaviour(value[4]);
	}

	static ETypeClass StringToBehaviour(string value)
	{
		//remove spaces
		value = value.Replace(" ", "");

		//return enum
		return (ETypeClass)System.Enum.Parse(typeof(ETypeClass), value);
	}
}
