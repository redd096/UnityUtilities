using System.Collections.Generic;
using UnityEngine;

namespace redd096.CsvImporter
{
    /// <summary>
    /// Result after parse a file
    /// </summary>
    public struct FParseResult
    {
        #region results

        /// <summary>
        /// Return true if there is a csv to parse
        /// </summary>
        public bool IsSuccess;

        /// <summary>
        /// Default file content, not parsed
        /// </summary>
        public string DefaultFileContent;

        /// <summary>
        /// File parsed and splitted in columns and rows
        /// </summary>
        public string[][] ParsedColumnsAndRows;

        /// <summary>
        /// File parsed and splitted in columns and rows. Then every cell content is splitted in array
        /// </summary>
        public Dictionary<Vector2Int, string[]> ParsedSplittedInArray;

        #endregion

        #region rows and columns to help find cells

        /// <summary>
        /// Use first column to define rows. Key is cell content, int the row index. NB content could be parsed if setted in FParseOptions
        /// </summary>
        public Dictionary<string, int> Rows;

        /// <summary>
        /// Use first column to define rows. Key is cell content, int the row index. NB content could be parsed if setted in FParseOptions
        /// </summary>
        public Dictionary<string, int> RowsNotCaseSentive;

        /// <summary>
        /// Use first row to define columns. Key is cell content, int the column index. NB content could be parsed if setted in FParseOptions
        /// </summary>
        public Dictionary<string, int> Columns;

        /// <summary>
        /// Use first row to define columns. Key is cell content, int the column index. NB content could be parsed if setted in FParseOptions
        /// </summary>
        public Dictionary<string, int> ColumnsNotCaseSensitive;

        /// <summary>
        /// Get row by content in the first column of the .csv file. NB content could be parsed if setted in FParseOptions
        /// </summary>
        public int GetRowByName(string rowName, bool caseSensitive = false)
        {
            return caseSensitive ? Rows[rowName] : RowsNotCaseSentive[rowName];
        }

        /// <summary>
        /// Get column by content in the first row of the .csv file. NB content could be parsed if setted in FParseOptions
        /// </summary>
        public int GetColumnByName(string columnName, bool caseSensitive = false)
        {
            return caseSensitive ? Columns[columnName] : ColumnsNotCaseSensitive[columnName];
        }

        #endregion

        #region helper utilities

        /// <summary>
        /// File parsed and splitted in columns and rows. Get content of cell at column and row
        /// </summary>
        public string GetCellContent(string column, string row, bool caseSensitive = false)
        {
            return ParsedColumnsAndRows[GetColumnByName(column, caseSensitive)][GetRowByName(row, caseSensitive)];
        }

        /// <summary>
        /// File parsed and splitted in columns and rows. Get content of cell at column and row
        /// </summary>
        public string GetCellContent(int column, string row, bool caseSensitive = false)
        {
            return ParsedColumnsAndRows[column][GetRowByName(row, caseSensitive)];
        }

        /// <summary>
        /// File parsed and splitted in columns and rows. Get content of cell at column and row
        /// </summary>
        public string GetCellContent(string column, int row, bool caseSensitive = false)
        {
            return ParsedColumnsAndRows[GetColumnByName(column, caseSensitive)][row];
        }

        /// <summary>
        /// File parsed and splitted in columns and rows. Get content of cell at column and row
        /// </summary>
        public string GetCellContent(int column, int row)
        {
            return ParsedColumnsAndRows[column][row];
        }

        #endregion

        #region helper utilities array

        /// <summary>
        /// File parsed and splitted in columns and rows. Then every cell content is splitted in array. Get array of cell at column and row
        /// </summary>
        public string[] GetCellArrayContent(string column, string row, bool caseSensitive = false)
        {
            return ParsedSplittedInArray[new Vector2Int(GetColumnByName(column, caseSensitive), GetRowByName(row, caseSensitive))];
        }

        /// <summary>
        /// File parsed and splitted in columns and rows. Then every cell content is splitted in array. Get array of cell at column and row
        /// </summary>
        public string[] GetCellArrayContent(int column, string row, bool caseSensitive = false)
        {
            return ParsedSplittedInArray[new Vector2Int(column, GetRowByName(row, caseSensitive))];
        }

        /// <summary>
        /// File parsed and splitted in columns and rows. Then every cell content is splitted in array. Get array of cell at column and row
        /// </summary>
        public string[] GetCellArrayContent(string column, int row, bool caseSensitive = false)
        {
            return ParsedSplittedInArray[new Vector2Int(GetColumnByName(column, caseSensitive), row)];
        }

        /// <summary>
        /// File parsed and splitted in columns and rows. Then every cell content is splitted in array. Get array of cell at column and row
        /// </summary>
        public string[] GetCellArrayContent(int column, int row, bool caseSensitive = false)
        {
            return ParsedSplittedInArray[new Vector2Int(column, row)];
        }

        #endregion
    }
}