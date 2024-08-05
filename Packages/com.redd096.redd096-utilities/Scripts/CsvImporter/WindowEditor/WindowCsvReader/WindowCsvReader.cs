#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace redd096.CsvImporter
{
    /// <summary>
    /// Show a window to read a file .csv
    /// </summary>
    public class WindowCsvReader : EditorWindow
    {
        Vector2 scrollPosition = Vector2.zero;

        string csvFilePath;
        bool showParseOptions = true;
        FParseOptions parseOptions = FParseOptions.defaultValues;

        FParseResult parseResult;
        Vector2 scrollDefaultCsv = Vector2.zero;
        Vector2 scrollParsedCsv = Vector2.zero;
        Vector2 scrollSplittedArrayCsv = Vector2.zero;

        /// <summary>
        /// Open Window from Editor
        /// </summary>
        [MenuItem("Tools/redd096/CSV Importer/CSV Reader")]
        static void OpenWindowCSV()
        {
            //open window (and set title)
            GetWindow<WindowCsvReader>("CSV Reader");
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            EditorGUILayout.Space(15);

            ShowCsvContainer();
            EditorGUILayout.Space(20);
            ShowParseOptions();
            EditorGUILayout.Space(20);

            //try parse csv
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            if (GUILayout.Button("Read .csv"))
            {
                parseResult = CsvImporter.ReadCsvAtPath(csvFilePath, parseOptions);
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(20);
            ShowDefaultCsv();
            EditorGUILayout.Space(20);
            ShowParsedCsv();
            EditorGUILayout.Space(20);
            ShowParsedSplittedArrayCsv();

            EditorGUILayout.Space(15);
            EditorGUILayout.EndScrollView();
        }

        #region window gui

        void ShowCsvContainer()
        {
            EditorGUILayout.LabelField("File .csv", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField(new GUIContent("File path:", ""), csvFilePath);
            if (GUILayout.Button("...", EditorStyles.miniButtonRight, GUILayout.Width(22)))
            {
                csvFilePath = EditorUtility.OpenFilePanel("Select file .csv", Application.dataPath, "csv");
            }
            EditorGUILayout.EndHorizontal();
            //csvFile = EditorGUILayout.ObjectField(csvFile, typeof(Object), false);
        }

        void ShowParseOptions()
        {
            //options
            showParseOptions = EditorGUILayout.BeginFoldoutHeaderGroup(showParseOptions, "Parse Options");

            if (showParseOptions)
            {
                EditorGUI.indentLevel++;

                //parse
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Parse", EditorStyles.boldLabel);
                parseOptions.trimAll = EditorGUILayout.Toggle(new GUIContent("Trim All", "Remove spaces to every text in the .csv file but first row"), parseOptions.trimAll);
                parseOptions.removeDoubleQuotes = EditorGUILayout.Toggle(new GUIContent("Remove double quotes", "Csv files use double quotes to makes understand when a cell contains a comma. After parse, remove these double quotes? \n- e.g. \"Mike have 3,14 apples\",Paola have 2 apples. These are two cells, separated by comma. The first one has double quotes to avoid split 3,14"), parseOptions.removeDoubleQuotes);
                parseOptions.removeEmptyColumns = EditorGUILayout.Toggle(new GUIContent("Remove empty columns", "If a column has every cell empty, remove from the list"), parseOptions.removeEmptyColumns);
                parseOptions.removeEmptyRows = EditorGUILayout.Toggle(new GUIContent("Remove empty rows", "If a row has every cell empty, remove from the list"), parseOptions.removeEmptyRows);

                //split single cell in array
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Split cell in array (If there are more values in a cell, split in array)", EditorStyles.boldLabel);
                parseOptions.splitSingleCellInArray = EditorGUILayout.Toggle(new GUIContent("Split cell in array", "If in a cell there are more values, for example a list of tags, you can split them in array"), parseOptions.splitSingleCellInArray);

                if (parseOptions.splitSingleCellInArray)
                {
                    EditorGUI.indentLevel++;
                    EditorGUIUtility.labelWidth += 60;

                    //split array options
                    parseOptions.stringForSplitCellInArray = EditorGUILayout.TextField(new GUIContent("String for Split:", "If in a cell there are more values, split them in array by using this as splitter string"), parseOptions.stringForSplitCellInArray);
                    parseOptions.trimArrayElements = EditorGUILayout.Toggle(new GUIContent("Trim Array Elements", "Remove spaces in array elements"), parseOptions.trimArrayElements);
                    parseOptions.removeEmptyElements = EditorGUILayout.Toggle(new GUIContent("Remove Empty Elements", "Remove empty array elements"), parseOptions.removeEmptyElements);
                    parseOptions.removeDuplicatedElements = EditorGUILayout.Toggle(new GUIContent("Remove Duplicated Elements", "Remove duplicated array elements"), parseOptions.removeDuplicatedElements);

                    EditorGUIUtility.labelWidth -= 60;
                    EditorGUI.indentLevel--;
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        void ShowDefaultCsv()
        {
            //default csv
            EditorGUILayout.LabelField("Default Csv", EditorStyles.boldLabel);

            //create one string
            string parsed = "";
            if (parseResult.DefaultFileContent != null)
            {
                parsed = parseResult.DefaultFileContent;
            }

            scrollDefaultCsv = EditorGUILayout.BeginScrollView(scrollDefaultCsv);
            Vector2 textSize = EditorStyles.whiteLabel.CalcSize(new GUIContent(parsed));
            EditorGUILayout.LabelField(parsed, EditorStyles.whiteLabel, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true), GUILayout.MinWidth(textSize.x), GUILayout.MinHeight(textSize.y));
            EditorGUILayout.EndScrollView();
        }

        void ShowParsedCsv()
        {
            //parsed csv
            EditorGUILayout.LabelField("Parsed Csv", EditorStyles.boldLabel);

            //create one string
            string parsed = "";
            if (parseResult.ParsedColumnsAndRows != null)
            {
                for (int row = 0; row < parseResult.ParsedColumnsAndRows[0].Length; row++)
                {
                    for (int column = 0; column < parseResult.ParsedColumnsAndRows.Length; column++)
                    {
                        //split columns: |stringToShow|TAB
                        parsed += $"|{parseResult.ParsedColumnsAndRows[column][row]}|\t";
                    }

                    //split rows with NEW LINE
                    parsed += "\n";
                }
            }

            scrollParsedCsv = EditorGUILayout.BeginScrollView(scrollParsedCsv);
            Vector2 textSize = EditorStyles.whiteLabel.CalcSize(new GUIContent(parsed));
            EditorGUILayout.LabelField(parsed, EditorStyles.whiteLabel, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true), GUILayout.MinWidth(textSize.x), GUILayout.MinHeight(textSize.y));
            EditorGUILayout.EndScrollView();
        }

        void ShowParsedSplittedArrayCsv()
        {
            //splitted array csv
            EditorGUILayout.LabelField("Parsed Splitted Array Csv", EditorStyles.boldLabel);

            //create one string
            string parsed = "";
            if (parseResult.ParsedColumnsAndRows != null && parseResult.ParsedSplittedInArray != null && parseResult.ParsedSplittedInArray.Count > 0)
            {
                for (int row = 0; row < parseResult.ParsedColumnsAndRows[0].Length; row++)
                {
                    for (int column = 0; column < parseResult.ParsedColumnsAndRows.Length; column++)
                    {
                        //split columns: |arrayOfStringToShow|TAB
                        parsed += "|";

                        Vector2Int coordinates = new Vector2Int(column, row);
                        for (int index = 0; index < parseResult.ParsedSplittedInArray[coordinates].Length; index++)
                        {
                            //split elements of array: /stringToShow (INDEX)/
                            parsed += $"/{parseResult.ParsedSplittedInArray[coordinates][index]} ({index})/";

                            //if not last array element, split from others with another TAB
                            if (index < parseResult.ParsedSplittedInArray[coordinates].Length - 1)
                                parsed += "\t";
                        }

                        parsed += $"|\t";
                    }

                    //split rows with NEW LINE
                    parsed += "\n";
                }
            }

            scrollSplittedArrayCsv = EditorGUILayout.BeginScrollView(scrollSplittedArrayCsv);
            Vector2 textSize = EditorStyles.whiteLabel.CalcSize(new GUIContent(parsed));
            EditorGUILayout.LabelField(parsed, EditorStyles.whiteLabel, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true), GUILayout.MinWidth(textSize.x), GUILayout.MinHeight(textSize.y));
            EditorGUILayout.EndScrollView();
        }

        #endregion
    }
}
#endif