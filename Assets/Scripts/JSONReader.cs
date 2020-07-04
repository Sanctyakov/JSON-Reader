using System;
using System.Text.RegularExpressions;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class JSONReader : MonoBehaviour
{
    public Text title; //Public variables. References set within the inspector.

    public GameObject columnPrefab, cellPrefab; //Prefabs for columns and cells, with layout groups to store cells and content fitters for text.

    public Transform columnsTransform; //A layout group for storing columns.

    void Start()
    {
        LoadJson(); //The main function.
    }

    public void LoadJson()
    {
        string jsonString = String.Empty;

        //Fake json string used for testing.

        /*jsonString = "{\"Title\":\"Team Members\"," +
                     "\"ColumnHeaders\":[\"ID\",\"Name\",\"Role\",\"Nickname\"]," +
                     "\"Data\":[{\"ID\":\"001\",\"Name\":\"John Doe\",\"Role\":\"Engineer\",\"Nickname\":\"KillerJo\",}," +
                     "{\"ID\":\"023\",\"Name\":\"Claire Dawn\",\"Role\":\"Engineer\",\"Nickname\":\"Claw\",}," +
                     "{\"ID\":\"012\",\"Name\":\"Paul Beef\",\"Role\":\"Designer\",\"Nickname\":\"BeefyPaul\",}," +
                     "]" +
                     "}";*/

        string jsonPath = Path.Combine(Application.streamingAssetsPath, "JsonChallenge.json"); //File path is StreamingAssets path.

        if (File.Exists(jsonPath))
        {
            jsonString = File.ReadAllText(jsonPath);

            jsonString = Regex.Replace(jsonString, @"(""[^""\\]*(?:\\.[^""\\]*)*"")|\s+", "$1"); //A regular expression that removes white spaces from everywhere except between quotes.

            title.text = GetFieldContent("Title", jsonString); //The table's title.

            string[] columnHeaders = GetArrayElements(jsonString, ":[\"", "],\"Data\":[{\"", new string[] { "[", "\"", ",", "]" });

            string[] dataStrings = GetArrayElements(jsonString, "\"Data\":[", "]}", new string[] { "{", ",},", "},", "]}", "}" });

            foreach (string columnHeader in columnHeaders)
            {
                GameObject newColumn = Instantiate(columnPrefab, columnsTransform); //Columns are fitted within the table's layout group as they are instantiated.

                newColumn.GetComponentInChildren<Text>().text = columnHeader; //Column headers are given names.

                foreach (string dataString in dataStrings)
                {
                    GameObject newCell = Instantiate(cellPrefab, newColumn.transform.Find("Cells").transform); //Cells are fitted within the recently instantiated column's layout group.

                    newCell.GetComponentInChildren<Text>().text = GetFieldContent(columnHeader, dataString); //The cell is given its text.
                }
            }
        }
        else
        {
            title.text = "Json file missing"; //Lets the user know there's no file to read.
        }
    }

    private string GetFieldContent(string field, string content) //Locates a field and returns its content.
    {
        field = "\"" + field + "\":\""; //Add symbols for easy removal.

        if (!content.Contains(field))
        {
            content = String.Empty; //If the data element does not contain the current header, the cell will be shown empty.
        }
        else
        {
            content = Isolate(content, field, "\"");
        }

        return content;
    }

    private string[] GetArrayElements (string anyString, string start, string end, string[] separators) //Locates an array and returns its elements.
    {
        anyString = Isolate(anyString, start, end);

        string[] returnArray = anyString.Split(separators, StringSplitOptions.RemoveEmptyEntries);

        return returnArray;
    }

    private string Isolate (string anyString, string start, string end) //Isolates a substring from the rest of the string.
    {
        anyString = anyString.Substring(anyString.IndexOf(start) + start.Length); //Leaves the start string out.

        anyString = anyString.Remove(anyString.IndexOf(end)); //Removes everything from the end string onwards.

        return anyString;
    }
}
