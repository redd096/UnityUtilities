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

        //split array options (If there are more values in a cell, split in array)
        [Tooltip("If in a cell there are more values, for example a list of tags, you can split them in array")] public bool splitSingleCellInArray;
        [Tooltip("If in a cell there are more values, split them in array by using this as splitter string")] public string stringForSplitCellInArray;
        [Tooltip("Remove spaces in array elements")] public bool trimArrayElements;
        [Tooltip("Remove empty array elements")] public bool removeEmptyElements;
        [Tooltip("Remove duplicated array elements")] public bool removeDuplicatedElements;

        public static FParseOptions defaultValues = new FParseOptions()
        {
            trimAll = true,
            splitSingleCellInArray = false,
            stringForSplitCellInArray = ";",
            trimArrayElements = true,
            removeEmptyElements = true,
            removeDuplicatedElements = true
        };

        public static FParseOptions allTrue = new FParseOptions()
        {
            trimAll = true,
            splitSingleCellInArray = true,
            stringForSplitCellInArray = ";",
            trimArrayElements = true,
            removeEmptyElements = true,
            removeDuplicatedElements = true
        };
    }
}