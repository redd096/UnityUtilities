using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;

public static class LoadCSV
{
	/// <summary>
	/// Load CSV from internet, then call UpdateAsset to update, and at the end save everything
	/// </summary>
	public static void LoadAndUpdate<T>(string linkDB, string pathAssets, Action<string[], T> UpdateAsset) where T : ScriptableObject
	{
		//get value from spreadsheet online
		List<string[]> values = LoadCSVFromInternet(linkDB);

		//get every scriptable object
		T[] assets = Resources.LoadAll<T>(pathAssets);

		//update them
		UpdateAssets(values, assets, UpdateAsset);

		//set dirty, so unity know they are getting modified
		foreach (T asset in assets)
		{
			EditorUtility.SetDirty(asset);
		}

		//save assets
		AssetDatabase.SaveAssets();


		Debug.Log("Download Complete!");
	}

	/// <summary>
	/// Load CSV from internet. Return a list where every element is a row of the spreadsheet, then every row has an array for every value (column)
	/// </summary>
	public static List<string[]> LoadCSVFromInternet(string link)
	{
		UnityWebRequest www = UnityWebRequest.Get(link);
		UnityWebRequestAsyncOperation request = www.SendWebRequest();

		//wait download
		while (!request.isDone) { }

		//get error or load csv
		if (www.isNetworkError || www.isHttpError)
		{
			Debug.LogError(www.error);
			return new List<string[]>();
		}
		else
		{
			return LoadFromCsv(www.downloadHandler.text);
		}
	}

	#region load csv

	static List<string[]> LoadFromCsv(string csv)
	{
		//split in rows - then get a list of rows, every row is an array of values (column of the spreadsheet)
		string[] rowsCSV = SplitInRows(csv);
		List<string[]> simpleValues = GetSimpleValues(rowsCSV);

		//get values usable
		List<string[]> usableValues = GetUsableValues(simpleValues);

		return usableValues;
	}

	/// <summary>
	/// split csv in rows (every '\n')
	/// </summary>
	static string[] SplitInRows(string csv)
	{
		//character for new line ( normally \n )
		string[] newLine = new[] { Environment.NewLine };

		return csv.Split(newLine, StringSplitOptions.None);
	}

	/// <summary>
	/// return list of rows, every row is an array of values (split by ',')
	/// </summary>
	static List<string[]> GetSimpleValues(string[] rows)
	{
		List<string[]> rawValues = new List<string[]>();

		//foreach row
		for (int i = 0; i < rows.Length; i++)
		{
			//this row, split by ',' to get every value
			string[] row = rows[i].Split(',');

			//add to list
			rawValues.Add(row);
		}

		return rawValues;
	}

	/// <summary>
	/// simple values from spreadsheet to usable values for unity
	/// </summary>
	static List<string[]> GetUsableValues(this List<string[]> simpleValues)
	{
		List<string[]> values = new List<string[]>();

		//foreach row in the list
		foreach (string[] row in simpleValues)
		{
			//add usable values
			values.Add(ToUsable(row));
		}

		return values;
	}

	/// <summary>
	/// Removes Symbols like % and find floats
	/// </summary>
	static string[] ToUsable(string[] row)
	{
		//create list copy of row
		List<string> usableRow = new List<string>();

		//used to remove values
		bool ignoreNext = false;

		//check every value (every element in the array)
		for (int i = 0; i < row.Length; i++)
		{
			if (ignoreNext)
			{
				//ignore this one
				ignoreNext = false;
				continue;
			}

			string usableValue = "";

			//replace point (.) with comma (,) for floats
			row[i] = row[i].Replace('.', ',');

			//check if this value need to be setted usable
			if (FirstIsQuote(row, i, out usableValue, out ignoreNext))
			{
				usableRow.Add(usableValue);
				continue;
			}
			else if (RemovePercentage(row[i], out usableValue))
			{
				usableRow.Add(usableValue);
				continue;
			}

			//copy same row
			usableRow.Add(row[i]);
		}

		return usableRow.ToArray();
	}

	/// <summary>
	/// if first char is quote (") and the same for last char in next string, then these are a single float separated in GetRawValues() because the comma
	/// </summary>
	static bool FirstIsQuote(string[] row, int valueIndex, out string result, out bool ignoreNext)
	{
		if (row[valueIndex].Length > 0)
		{
			//check first char is (")
			if (row[valueIndex][0] == '"')
			{
				int next = valueIndex + 1;

				//check there is next value
				if (row.Length > next)
				{
					int length = row[next].Length;

					//check there is something
					if (length > 0)
					{
						//check also last char is (")
						if (row[next][length - 1] == '"')
						{
							//then next will be ignored, because we are checking it now
							ignoreNext = true;

							//we must to create one single string from this and next value, separated by comma (,)
							string floatValueInQuotes = string.Concat(row[valueIndex], ",", row[next]);

							//remove first and last char, cause they are quotes (")
							result = floatValueInQuotes.RemoveFirstAndLast();

							//if has percentage, remove it
							RemovePercentage(result, out result);

							return true;
						}
					}
				}
			}
		}

		ignoreNext = false;
		result = row[valueIndex];
		return false;
	}

	/// <summary>
	/// if last char is percentage (%), remove that char
	/// </summary>
	static bool RemovePercentage(string value, out string result)
	{
		//check last char is (%)
		if (value.Length > 0)
		{
			int last = value.Length - 1;

			if (value[last] == '%')
			{
				//remove last char
				result = value.Remove(last);

				return true;
			}
		}

		result = value;
		return false;
	}

	/// <summary>
	/// remove first and last char from this string
	/// </summary>
	static string RemoveFirstAndLast(this string stringWithQuotes)
	{
		string s = "";

		for (int i = 0; i < stringWithQuotes.Length; i++)
		{
			//ignore first and last char
			if (i <= 0 || i >= stringWithQuotes.Length - 1)
				continue;

			//add every other char
			s += stringWithQuotes[i];
		}

		return s;
	}

	#endregion

	#region load assets

	static void UpdateAssets<T>(List<string[]> valuesDB, T[] assetsToSet, Action<string[], T> UpdateAsset) where T : ScriptableObject
	{
		//foreach asset
		foreach (T asset in assetsToSet)
		{
			//check if there are its value online
			foreach (string[] value in valuesDB)
			{
				//0 is name - check ToUpper so no case sensitive
				if (value[0].ToUpper().Contains(asset.name.ToUpper()))
				{
					//update it
					UpdateAsset(value, asset);
				}
			}
		}
	}

	static T CreateMissingElement<T>(string objName) where T : ScriptableObject
	{
		//create instance
		T asset = ScriptableObject.CreateInstance<T>();

		//create asset in path
		string path = "Assets/Resources/ScriptableObjects/" + objName + ".asset";
		AssetDatabase.CreateAsset(asset, path);

		Debug.Log("Created: " + objName);

		return asset;
	}

	#endregion
}
