﻿namespace redd096
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using System.IO;

	public class WindowParseCSVData : ScriptableObject
	{
		const string DATANAME = "Window Parse CSV Data.asset";

		public ParseCSV ParseClass = new ParseCSV();

		#region static functions

		/// <summary>
		/// Get Path to this scriptable object saved in project folder
		/// </summary>
		/// <returns></returns>
		public static string GetDataPath()
		{
			//find script folder and create scriptable object there
			string[] guids = AssetDatabase.FindAssets(typeof(WindowParseCSVData).ToString());
			foreach (string guid in guids)
			{
				string pathToScript = AssetDatabase.GUIDToAssetPath(guid);                                                                          //get path to the script
				string pathToFolder = pathToScript.Remove(pathToScript.LastIndexOf('/'), pathToScript.Length - pathToScript.LastIndexOf('/'));      //remove everything from last slash (script name) to get only directory path

				return Path.Combine(pathToFolder, DATANAME);
			}

			return null;
		}

		/// <summary>
		/// Load or Create Scriptable Object
		/// </summary>
		/// <returns></returns>
		public static WindowParseCSVData LoadData()
		{
			//try get scriptable object (data)
			WindowParseCSVData data = AssetDatabase.LoadAssetAtPath<WindowParseCSVData>(GetDataPath());

			//if there is no data, create it
			if (data == null)
			{
				data = CreateInstance<WindowParseCSVData>();
				AssetDatabase.CreateAsset(data, GetDataPath());
			}

			//load data
			return data;
		}

		#endregion
	}

	[Serializable]
	public class ParseCSV
	{
		//every row is a scriptable object
		//every column is a variable (or array)

		[Header("CSV")]
		[TextArea(2, 5)]
		public string DefaultCSV = "";                      //downloaded CSV
		public string[][] ParsedCSV = default;              //at [row][column] there is excel cell item - can be a variable (item) or array (item1;item2;item3;)
		public string[][][] ArraysParsedCSV = default;      //every [row][column] is an array - a variable ([0] = item) or array ([0] = item1, [1] = item2, [2] = item3)

		[Header("Split Arrays")]
		public bool SplitArrays = true;
		public string StringToSplitArrayElements = ";";
		public bool TrimArrayElements = true;
		public bool RemoveEmptyArrayElements = true;
		public bool RemoveDuplicates = true;

		[Header("Dictionary")]
		public bool AllCapsDictionaryValues = true;
		public bool NoSpacesDictionaryValues = true;
		public string[] Dictionary;

		#region public API

		public void Parse()
		{
			//from csv to rows, then [rows][columns]
			string[] rows = SplitInRows(DefaultCSV);
			ParsedCSV = SplitInColumns(rows);

			//create a dictionary, so can reorder columns on excel without problem
			Dictionary = CreateDictionary(ParsedCSV);

			//if split arrays, split every string in array
			if (SplitArrays)
			{
				ArraysParsedCSV = SplitArray(ParsedCSV);
			}
		}

		#endregion

		#region private API

		/// <summary>
		/// Split csv in rows (every '\n')
		/// </summary>
		string[] SplitInRows(string csv)
		{
			//character for new line ( normally \n )
			return csv.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
		}

		/// <summary>
		/// Split a row in columns (every ',')
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		string[] SplitInColumns(string row)
		{
			//split row
			return row.Split(',');
		}

		/// <summary>
		/// Split every row in columns (every ',')
		/// </summary>
		/// <param name="rows"></param>
		/// <returns></returns>
		string[][] SplitInColumns(string[] rows)
		{
			string[][] rowsColumns = new string[rows.Length][];

			//split every row
			for (int i = 0; i < rows.Length; i++)
			{
				rowsColumns[i] = SplitInColumns(rows[i]);
			}

			return rowsColumns;
		}

		/// <summary>
		/// Create a dictionary using index (int) as Key, and string as Value
		/// </summary>
		/// <param name="firstValues"></param>
		/// <returns></returns>
		string[] CreateDictionary(string[] firstRow)
		{
			string[] dictionary = new string[firstRow.Length];

			//use index as Key, and string as Value
			for (int i = 0; i < firstRow.Length; i++)
			{
				dictionary[i] = ModifiedDictionaryValue(firstRow[i]);
			}

			return dictionary;
		}

		/// <summary>
		/// Create a dictionary from first row using index (int) as Key, and string as Value
		/// </summary>
		/// <param name="splittedRows"></param>
		/// <returns></returns>
		string[] CreateDictionary(string[][] rowsColumns)
		{
			//create dictionary from first row
			return CreateDictionary(rowsColumns[0]);
		}

		/// <summary>
		/// Split string in array
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>
		string[] SplitArray(string array)
		{
			//split array in a list
			string[] splittedArray = array.Split(new string[] { StringToSplitArrayElements }, StringSplitOptions.None);

			return ModifiedArray(splittedArray);
		}

		/// <summary>
		/// Split all parsed csv in arrays
		/// </summary>
		/// <param name="rowsColumns"></param>
		/// <returns></returns>
		string[][][] SplitArray(string[][] rowsColumns)
		{
			string[][][] parsedArrays = new string[rowsColumns.Length][][];

			for (int row = 0; row < rowsColumns.Length; row++)
			{
				parsedArrays[row] = new string[rowsColumns[row].Length][];

				for (int column = 0; column < rowsColumns[row].Length; column++)
				{
					//set every cell at [row][column] as an array
					parsedArrays[row][column] = SplitArray(rowsColumns[row][column]);
				}
			}

			return parsedArrays;
		}

		#region modify API

		string ModifiedDictionaryValue(string dictionaryValue)
		{
			//all caps
			if (AllCapsDictionaryValues)
				dictionaryValue = dictionaryValue.ToUpper();

			//remove white spaces
			if (NoSpacesDictionaryValues)
				dictionaryValue = dictionaryValue.Replace(" ", string.Empty);

			return dictionaryValue;
		}

		string[] ModifiedArray(string[] splittedArray)
		{
			//put array in a list
			List<string> splittedList = new List<string>(splittedArray);

			if (TrimArrayElements)
			{
				//if necessary - foreach array, split it
				for (int i = 0; i < splittedList.Count; i++)
					splittedList[i] = splittedList[i].Trim();
			}

			if (RemoveEmptyArrayElements)
			{
				//if necessary - foreach array, if empty remove from list
				foreach (string s in new List<string>(splittedList))
					if (string.IsNullOrWhiteSpace(s))
						splittedList.Remove(s);
			}

			if (RemoveDuplicates)
			{
				//if necessary - remove duplicates (create hashSet that remove duplicates by default and recreate a list from it)
				HashSet<string> listNoDuplicates = new HashSet<string>(splittedList);
				splittedList = new List<string>(listNoDuplicates);
			}

			return splittedList.ToArray();
		}

		#endregion

		#endregion
	}
}