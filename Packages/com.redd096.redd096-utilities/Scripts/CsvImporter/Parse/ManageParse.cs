using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace redd096.CsvImporter
{
    public static class ManageParse
    {
        /// <summary>
        /// Used to create dictionaries not case sensitive
        /// </summary>
        static StringComparer notCaseSensitiveComparer = StringComparer.InvariantCultureIgnoreCase;

        //in SplitString() we can use Regex. This is specifically used by SplitStringArray(), and this is a little explanation on how it works:
        //we make an example with this regular expression (the same as the one used by SplitStringArray, but using ' instead of ")
        //",(?=(?:[^']*'[^']*')*[^']*$)"
        //this example splits string using comma - the first character before (?=(?: -  NOT inside quotes
        //e.g. Mike's Kitchen,Jane's Room

        /// <summary>
        /// Parse file content and get result
        /// </summary>
        /// <param name="fileContent"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static FParseResult Parse(string fileContent, FParseOptions options)
        {
            if (string.IsNullOrEmpty(fileContent))
                return default;

            //split in rows (every '\n'), and in columns (every ',')
            string[] rows = SplitString(fileContent, Environment.NewLine);                              //Environment.NewLine normally is \n
            string[][] rowsAndColumns = SplitStringArray(rows, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");    //same as string.Split(",") but ignore if inside double quote, for example we don't want to split "3,14" because is a float
            string[][] columnsAndRows = ReverseDoubleArray(rowsAndColumns);                             //reverse to use column as X and row as Y

            //apply normal parse
            columnsAndRows = ApplyNormalParseOptions(columnsAndRows, options);

            //split cells in array
            Dictionary<Vector2Int, string[]> splittedInArray = new Dictionary<Vector2Int, string[]>();
            if (options.splitSingleCellInArray)
            {
                splittedInArray = SplitStringDoubleArray(columnsAndRows, options.stringForSplitCellInArray);
                splittedInArray = ApplySplittedArrayParseOptions(splittedInArray, options);
            }

            //create dictionaries
            CreateDictionaries(columnsAndRows, out Dictionary<string, int> columnsCaseSensitive, out Dictionary<string, int> columnsNotCaseSensitive,
            out Dictionary<string, int> rowsCaseSensitive, out Dictionary<string, int> rowsNotCaseSensitive);

            //set result
            FParseResult result = new FParseResult
            {
                IsSuccess = true,
                DefaultFileContent = fileContent,
                ParsedColumnsAndRows = columnsAndRows,
                ParsedSplittedInArray = splittedInArray,
                Rows = rowsCaseSensitive,
                RowsNotCaseSentive = rowsNotCaseSensitive,
                Columns = columnsCaseSensitive,
                ColumnsNotCaseSensitive = columnsNotCaseSensitive
            };

            return result;
        }

        #region split strings

        /// <summary>
        /// Split string in array
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static string[] SplitString(string s, string separator, bool useRegex = false)
        {
            //use regex for separators like ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)" to split by comma only if NOT inside double quotes
            return useRegex ? Regex.Split(s, separator) : s.Split(separator);
        }

        /// <summary>
        /// Split every string in array
        /// </summary>
        /// <param name="sArray"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        private static string[][] SplitStringArray(string[] sArray, string separator)
        {
            string[][] sDoubleArray = new string[sArray.Length][];

            //split every string in array
            for (int i = 0; i < sArray.Length; i++)
            {
                sDoubleArray[i] = SplitString(sArray[i], separator, useRegex: true);
            }

            return sDoubleArray;
        }

        /// <summary>
        /// Split every string in another array
        /// </summary>
        /// <param name="sDoubleArray"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        private static Dictionary<Vector2Int, string[]> SplitStringDoubleArray(string[][] sDoubleArray, string separator)
        {
            Dictionary<Vector2Int, string[]> sSplitted = new Dictionary<Vector2Int, string[]>();

            for (int x = 0; x < sDoubleArray.Length; x++)
            {
                for (int y = 0; y < sDoubleArray[x].Length; y++)
                {
                    //split every string in array
                    string[] sArray = SplitString(sDoubleArray[x][y], separator);
                    sSplitted.Add(new Vector2Int(x, y), sArray);
                }
            }

            return sSplitted;
        }

        #endregion

        #region parse private API

        /// <summary>
        /// Apply normal parse options to every string
        /// </summary>
        /// <param name="sDoubleArray"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private static string[][] ApplyNormalParseOptions(string[][] sDoubleArray, FParseOptions options)
        {
            //trim every string
            if (options.trimAll)
            {
                for (int x = 0; x < sDoubleArray.Length; x++)
                    for (int y = 0; y < sDoubleArray[x].Length; y++)
                        sDoubleArray[x][y] = sDoubleArray[x][y].Trim();
            }

            //remove double quotes
            if (options.removeDoubleQuotes)
            {
                for (int x = 0; x < sDoubleArray.Length; x++)
                    for (int y = 0; y < sDoubleArray[x].Length; y++)
                        sDoubleArray[x][y] = sDoubleArray[x][y].Replace("\"", string.Empty);
            }

            //remove empty columns
            if (options.removeEmptyColumns)
            {
                sDoubleArray = sDoubleArray.Where(column => column.All(row => string.IsNullOrEmpty(row)) == false).ToArray();   //keep only columns Where (column has All rows null or empty) == false
            }

            //remove empty rows
            if (options.removeEmptyRows)
            {
                List<List<string>> columnsAndRowsList = new List<List<string>>();                               //create a list of lists
                for (int x = 0; x < sDoubleArray.Length; x++)
                    columnsAndRowsList.Add(new List<string>(sDoubleArray[x]));

                int y = 0;
                int yLength = columnsAndRowsList[0].Count;
                while (y < yLength)
                {
                    if (columnsAndRowsList.All(column => column.Count <= y || string.IsNullOrEmpty(column[y]))) //if (All columns have this row null or empty)
                    {
                        for (int x = 0; x < columnsAndRowsList.Count; x++)                                      //Remove this row from every column
                            columnsAndRowsList[x].RemoveAt(y);

                        yLength--;
                    }
                    else
                    {
                        y++;
                    }
                }

                sDoubleArray = columnsAndRowsList.Select(column => column.ToArray()).ToArray();
            }

            return sDoubleArray;
        }

        /// <summary>
        /// Apply splitted array parse options to every string
        /// </summary>
        /// <param name="sSplitted"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private static Dictionary<Vector2Int, string[]> ApplySplittedArrayParseOptions(Dictionary<Vector2Int, string[]> sSplitted, FParseOptions options)
        {
            Dictionary<Vector2Int, string[]> copy = new Dictionary<Vector2Int, string[]>(sSplitted);
            foreach (var key in copy.Keys)
            {
                sSplitted[key] = ApplySplittedArrayParseOptionsToSingleArray(sSplitted[key], options);
            }

            return sSplitted;
        }

        /// <summary>
        /// Apply splitted array parse options to every string. This is only one array from the dictionary
        /// </summary>
        /// <param name="sArray"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private static string[] ApplySplittedArrayParseOptionsToSingleArray(string[] sArray, FParseOptions options)
        {
            //trim every string
            if (options.trimArrayElements)
            {
                for (int i = 0; i < sArray.Length; i++)
                    sArray[i] = sArray[i].Trim();
            }

            //remove empty
            if (options.removeEmptyElements)
            {
                List<string> sArrayInList = new List<string>(sArray);
                foreach (string s in sArray)
                {
                    if (string.IsNullOrEmpty(s))
                        sArrayInList.Remove(s);
                }

                sArray = sArrayInList.ToArray();
            }

            //remove duplicates
            if (options.removeDuplicatedElements)
            {
                HashSet<string> sArrayInHashSet = new HashSet<string>(sArray);
                sArray = new List<string>(sArrayInHashSet).ToArray();
            }

            return sArray;
        }

        #endregion

        #region other private API

        /// <summary>
        /// Create rows and columns dictionaries, both case sensitive and not case sensitive
        /// </summary>
        /// <param name="columnsAndRows"></param>
        /// <param name="columnsCaseSensitive"></param>
        /// <param name="columnsNotCaseSensitive"></param>
        /// <param name="rowsCaseSensitive"></param>
        /// <param name="rowsNotCaseSensitive"></param>
        private static void CreateDictionaries(string[][] columnsAndRows,
            out Dictionary<string, int> columnsCaseSensitive, out Dictionary<string, int> columnsNotCaseSensitive,
            out Dictionary<string, int> rowsCaseSensitive, out Dictionary<string, int> rowsNotCaseSensitive)
        {
            //create columns dictionaries, using first row as Key
            columnsCaseSensitive = new Dictionary<string, int>();
            columnsNotCaseSensitive = new Dictionary<string, int>(notCaseSensitiveComparer);

            for (int column = 0; column < columnsAndRows.Length; column++)
            {
                //if same name already in the list, change this to not have duplicates
                string key = columnsAndRows[column][0];
                while (columnsNotCaseSensitive.ContainsKey(key)) key += "#";

                columnsCaseSensitive.Add(key, column);
                columnsNotCaseSensitive.Add(key, column);
            }

            //create rows dictionaries, using first column as Key
            rowsCaseSensitive = new Dictionary<string, int>();
            rowsNotCaseSensitive = new Dictionary<string, int>(notCaseSensitiveComparer);

            for (int row = 0; row < columnsAndRows[0].Length; row++)
            {
                //if same name already in the list, change this to not have duplicates
                string key = columnsAndRows[0][row];
                while (rowsNotCaseSensitive.ContainsKey(key)) key += "#";

                rowsCaseSensitive.Add(key, row);
                rowsNotCaseSensitive.Add(key, row);
            }
        }

        /// <summary>
        /// If double array is for example [row][column], reverse to be [column][row]
        /// </summary>
        /// <param name="sDoubleArray"></param>
        private static string[][] ReverseDoubleArray(string[][] sDoubleArray)
        {
            List<List<string>> reversedList = new List<List<string>>(sDoubleArray[0].Length);
            for (int x = 0; x < sDoubleArray[0].Length; x++)
            {
                reversedList.Add(new List<string>(sDoubleArray.Length));
                for (int y = 0; y < sDoubleArray.Length; y++)
                {
                    //fix if in the example [row][column] to [column][row] we have some column longer than others
                    if (x >= sDoubleArray[y].Length)
                    {
                        string s = "";
                        for (int i = 0; i < sDoubleArray[y].Length; i++) s += sDoubleArray[y][i];
                        Debug.LogWarning($"Some arrays have different sizes. Counting from 0, row: {y} missing column {x}. It has only {sDoubleArray[y].Length} columns. {s}");
                        reversedList[x].Add(string.Empty);
                        continue;
                    }

                    //reverse
                    reversedList[x].Add(sDoubleArray[y][x]);
                }
            }

            //return as array
            string[][] reversedArray = new string[reversedList.Count][];
            for (int x = 0; x < reversedList.Count; x++)
                reversedArray[x] = reversedList[x].ToArray();

            return reversedArray;
        }

        #endregion
    }
}