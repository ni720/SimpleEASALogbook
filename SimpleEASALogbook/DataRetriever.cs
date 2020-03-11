using System;
using System.Data;
using System.Data.SqlClient;

public interface IDataPageRetriever
{
    DataTable SupplyPageOfData(int lowerPageBoundary, int rowsPerPage);
}

public class DataRetriever : IDataPageRetriever
{
    private string tableName;
    private SqlCommand command;

    public DataRetriever(string connectionString, string tableName)
    {
        SqlConnection connection = new SqlConnection(connectionString);
        connection.Open();
        command = connection.CreateCommand();
        this.tableName = tableName;
    }

    private int rowCountValue = -1;

    public int RowCount
    {
        get
        {
            // Return the existing value if it has already been determined.
            if (rowCountValue != -1)
            {
                return rowCountValue;
            }

            // Retrieve the row count from the database.
            command.CommandText = "SELECT COUNT(*) FROM " + tableName;
            rowCountValue = (int)command.ExecuteScalar();
            return rowCountValue;
        }
    }

    private DataColumnCollection columnsValue;

    public DataColumnCollection Columns
    {
        get
        {
            // Return the existing value if it has already been determined.
            if (columnsValue != null)
            {
                return columnsValue;
            }

            // Retrieve the column information from the database.
            command.CommandText = "SELECT * FROM " + tableName;
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = command;
            DataTable table = new DataTable();
            table.Locale = System.Globalization.CultureInfo.InvariantCulture;
            adapter.FillSchema(table, SchemaType.Source);
            columnsValue = table.Columns;
            return columnsValue;
        }
    }

    private string commaSeparatedListOfColumnNamesValue = null;

    private string CommaSeparatedListOfColumnNames
    {
        get
        {
            // Return the existing value if it has already been determined.
            if (commaSeparatedListOfColumnNamesValue != null)
            {
                return commaSeparatedListOfColumnNamesValue;
            }

            // Store a list of column names for use in the
            // SupplyPageOfData method.
            System.Text.StringBuilder commaSeparatedColumnNames =
                new System.Text.StringBuilder();
            bool firstColumn = true;
            foreach (DataColumn column in Columns)
            {
                if (!firstColumn)
                {
                    commaSeparatedColumnNames.Append(", ");
                }
                commaSeparatedColumnNames.Append(column.ColumnName);
                firstColumn = false;
            }

            commaSeparatedListOfColumnNamesValue =
                commaSeparatedColumnNames.ToString();
            return commaSeparatedListOfColumnNamesValue;
        }
    }

    // Declare variables to be reused by the SupplyPageOfData method.
    private string columnToSortBy;
    private SqlDataAdapter adapter = new SqlDataAdapter();

    public DataTable SupplyPageOfData(int lowerPageBoundary, int rowsPerPage)
    {
        // Store the name of the ID column. This column must contain unique 
        // values so the SQL below will work properly.
        //columnToSortBy ??= this.Columns[0].ColumnName;

        if (!this.Columns[columnToSortBy].Unique)
        {
            throw new InvalidOperationException(String.Format(
                "Column {0} must contain unique values.", columnToSortBy));
        }

        // Retrieve the specified number of rows from the database, starting
        // with the row specified by the lowerPageBoundary parameter.
        command.CommandText = "Select Top " + rowsPerPage + " " +
            CommaSeparatedListOfColumnNames + " From " + tableName +
            " WHERE " + columnToSortBy + " NOT IN (SELECT TOP " +
            lowerPageBoundary + " " + columnToSortBy + " From " +
            tableName + " Order By " + columnToSortBy +
            ") Order By " + columnToSortBy;
        adapter.SelectCommand = command;

        DataTable table = new DataTable();
        table.Locale = System.Globalization.CultureInfo.InvariantCulture;
        adapter.Fill(table);
        return table;
    }
}