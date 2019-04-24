using System;
using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Text.RegularExpressions;

namespace ReadIAT
{
    public partial class IAT
    {
        /// <summary>
        /// Contains data from one of the sub tables in an IAT file
        /// </summary>
        private class SubTable
        {
            /// <summary>
            /// The IAT object which owns the table
            /// </summary>
            private IAT iat;

            /// <summary>
            /// Table name
            /// </summary>
            public string Name { get; }            

            /// <summary>
            /// Row containing table title
            /// </summary>
            private int title_row;

            /// <summary>
            /// Column containing all table titles
            /// </summary>
            private int title_col = 2;

            /// <summary>
            /// Number of rows in the table
            /// </summary>
            private int rows = 0;

            /// <summary>
            /// Number of columns in the table
            /// </summary>
            private int cols = 0;

            /// <summary>
            /// List of row names in the table
            /// </summary>
            public List<string> RowNames { get; } = new List<string>();

            /// <summary>
            /// List of column names in the table
            /// </summary>
            public List<string> ColumnNames { get; } = new List<string>();

            /// <summary>
            /// List containing secondary row data found in some tables
            /// </summary>
            public List<string> ExtraNames { get; } = new List<string>();

            /// <summary>
            /// Grid of table data
            /// </summary>
            private string[,] data;

            /// <summary>
            /// Looks through an IAT for the given name and attempts to load data into the object
            /// </summary>
            /// <param name="name">Name of the table to search for</param>
            /// <param name="iat">IAT to search through</param>
            public SubTable(string name, IAT iat)
            {
                this.iat = iat;
                Name = name;                

                // Check the table exists
                if (!FindTable()) throw new Exception($"'{name}' table not found, invalid dataset");

                // Attenot to find the rows and columns of the table
                LoadRows();
                LoadCols();

                // Attempt to load table data once rows/columns are found
                LoadExtra();
                LoadData();
            }

            /// <summary>
            /// Checks if the table exists in the IAT file, and sets the title row if it finds the table
            /// </summary>
            /// <returns></returns>
            private bool FindTable()
            {
                // Find all non-empty cells
                var non_empty =
                    from cells
                    in iat.Part.Worksheet.Descendants<Cell>()
                    where cells.DataType != null
                    select cells;

                // Find only the cells which contain text (not numeric data)
                // Note: This comparison only works on non_empty cells (not null valued)
                var has_text =
                    from cells
                    in non_empty
                    where cells.DataType.Value == CellValues.SharedString
                    select cells;

                // Check if the text in the cells matches the title text
                string value;
                foreach (var cell in has_text)
                {
                    value = iat.StringTable.SharedStringTable.ElementAt(int.Parse(cell.InnerText)).InnerText;
                    if (value == Name)
                    {
                        // Find the Reference of the Cell containing the title
                        Regex reg = new Regex(@"([A-Z]+)(\d+)");
                        Match match = reg.Match(cell.CellReference.InnerText);

                        // Find the number of the title row
                        int.TryParse(match.Groups[2].Value, out title_row);
                        return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// Finds the name of each row, assuming at least one row exists in a table
            /// </summary>
            private void LoadRows()
            {
                do
                {
                    // Count the number of rows in the table
                    rows++;

                    // Add the row name to the list, if name is missing, row is named after rank
                    string title = iat.GetCellValue(iat.Part, title_row + rows, title_col);
                    if (title == "") title = rows.ToString();
                    RowNames.Add(title);
                }
                // End of the table is marked by empty cells
                while (iat.GetCellValue(iat.Part, title_row + 1 + rows, title_col) != "");
                return;
            }

            /// <summary>
            /// Finds the title of each column, assuming at least one column exists in a table
            /// </summary>
            private void LoadCols()
            {
                do
                {
                    // Count the number of columns in the table
                    cols++;

                    // If title is missing, column is named after rank
                    string title = iat.GetCellValue(iat.Part, title_row, title_col + 1 + cols);
                    if (title == "") title = cols.ToString();
                    ColumnNames.Add(title);
                }
                // End of the table is marked by empty cells
                while (iat.GetCellValue(iat.Part, title_row + 1, title_col + 2 + cols) != "");
                return;
            }

            /// <summary>
            /// Loads the extra column of row data
            /// </summary>
            private void LoadExtra()
            {
                // Find rows containing the table data
                var table_rows = iat.Part.Worksheet.Descendants<Row>().Skip(title_row).Take(rows);

                // Find the extra data cell in each row
                foreach (Row row in table_rows)
                {
                    Cell cell = row.Descendants<Cell>().ElementAt(title_col);
                    ExtraNames.Add(iat.ParseCell(cell));
                }
                return;
            }

            /// <summary>
            /// Loads the data contained within the table
            /// </summary>
            private void LoadData()
            {
                // Initialise data array *after* the number of rows and columns is found
                data = new string[rows, cols];

                // Select the rows which contain the table
                var table_rows = iat.Part.Worksheet.Descendants<Row>().Skip(title_row).Take(rows);

                // Go over each row in the table
                int r = 0;
                foreach (Row row in table_rows)
                {
                    // Select the cells in the row which are part of the table
                    var cells = row.Descendants<Cell>().Skip(title_col + 1).Take(cols);

                    // Find the value of the cell data and store it
                    int c = 0;
                    foreach (Cell cell in cells)
                    {
                        data[r, c] = iat.ParseCell(cell);
                        c++;
                    }
                    r++;
                }
            }

            /// <summary>
            /// Converts a string to a new data type, if possible.
            /// If not, return the default type and log the error.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="s"></param>
            /// <returns></returns>
            private T ConvertData<T>(string s)
            {
                try
                {
                    T value = (T)Convert.ChangeType(s, typeof(T));
                    return value;
                }
                catch (FormatException)
                {
                    Error.Write($"Table '{Name}' contains an empty cell.", iat);
                    return default(T);
                }
                catch (IndexOutOfRangeException)
                {
                    Error.Write($"Table '{Name}' tried to access data outside the table.", iat);
                    return default(T);
                }
                catch (InvalidCastException)
                {
                    T t = default(T);
                    Error.Write($"Table '{Name}' expected type {t.GetType().Name}, but did not find it.", iat);
                    return default(T);
                }
                catch
                {
                    Error.Write($"Table '{Name}' contains an unknown error.", iat);
                    return default(T);
                }
                
            }

            /// <summary>
            /// Accesses the data at the given location
            /// </summary>
            /// <param name="r">Row position</param>
            /// <param name="c">Column position</param>
            public T GetData<T>(int r, int c) => ConvertData<T>(data[r, c]);

            /// <summary>
            /// Selects a row of data from the table in the given type.
            /// Invalid type casts return default values.
            /// </summary>
            /// <param name="r">Row number</param>
            public List<T> GetRowData<T>(int r)
            {
                return Enumerable.Range(0, data.GetLength(1))
                    .Select(c => ConvertData<T>(data[r, c]))
                    .ToList();
            }

            /// <summary>
            /// Selects a column of data from the table in the given type.
            /// Invalid type casts return default values.
            /// </summary>
            /// <param name="c">Column number</param>
            public List<T> GetColData<T>(int c)
            {
                return Enumerable.Range(0, data.GetLength(0))
                    .Select(r => ConvertData<T>(data[r, c]))
                    .ToList();
            }

        }
    }
}

