using UnityEngine;

namespace redd096.CsvImporter
{
    /// <summary>
    /// Options used to Parse a file
    /// </summary>
    [System.Serializable]
    public struct FParseOptions
    {
        //normal parse
        [Tooltip("Remove spaces to every text in the .csv file")] public bool trimAll;
        [Tooltip("Csv files use double quotes to makes understand when a cell contains a comma. After parse, remove these double quotes? \n" +
            "- e.g. \"Mike have 3,14 apples\",Paola have 2 apples. These are two cells, separated by comma. The first one has double quotes to avoid split 3,14")]
        public bool removeDoubleQuotes;
        [Tooltip("If a column has every cell empty, remove from the list")] public bool removeEmptyColumns;
        [Tooltip("If a row has every cell empty, remove from the list")] public bool removeEmptyRows;

        //split array options (If there are more values in a cell, split in array)
        [Tooltip("If in a cell there are more values, for example a list of tags, you can split them in array")] public bool splitSingleCellInArray;
        [Tooltip("If in a cell there are more values, split them in array by using this as splitter string")] public string stringForSplitCellInArray;
        [Tooltip("Remove spaces in array elements")] public bool trimArrayElements;
        [Tooltip("Remove empty array elements")] public bool removeEmptyElements;
        [Tooltip("Remove duplicated array elements")] public bool removeDuplicatedElements;

        public static FParseOptions defaultValues = new FParseOptions()
        {
            trimAll = true,
            removeDoubleQuotes = true,
            removeEmptyColumns = true,
            removeEmptyRows = true,
            splitSingleCellInArray = false,
            stringForSplitCellInArray = ";",
            trimArrayElements = true,
            removeEmptyElements = true,
            removeDuplicatedElements = true
        };

        public static FParseOptions allTrue = new FParseOptions()
        {
            trimAll = true,
            removeDoubleQuotes = true,
            removeEmptyColumns = true,
            removeEmptyRows = true,
            splitSingleCellInArray = true,
            stringForSplitCellInArray = ";",
            trimArrayElements = true,
            removeEmptyElements = true,
            removeDuplicatedElements = true
        };
    }
}