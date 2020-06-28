using System;
using System.Text.RegularExpressions;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct MainStruct //A serializable struct to store the title and columnheaders, which can be populated via Unity's JsonUtility.
{
    public string Title;
    public string[] ColumnHeaders;
}

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

        /*jsonString = "{\"Title\":\"Team Members\"," +
                     "\"ColumnHeaders\":[\"ID\",\"Name\",\"Role\",\"Nickname\"]," +
                     "\"Data\":[{\"ID\":\"001\",\"Name\":\"John Doe\",\"Role\":\"Engineer\",\"Nickname\":\"KillerJo\",}," +
                     "{\"ID\":\"023\",\"Name\":\"Claire Dawn\",\"Role\":\"Engineer\",\"Nickname\":\"Claw\",}," +
                     "{\"ID\":\"012\",\"Name\":\"Paul Beef\",\"Role\":\"Designer\",\"Nickname\":\"BeefyPaul\",}," +
                     "]" +
                     "}";*/ //Fake json string used for testing.

        string jsonPath = Path.Combine(Application.streamingAssetsPath, "JsonChallenge.json");

        if (File.Exists(jsonPath))
        {
            jsonString = File.ReadAllText(jsonPath);

            jsonString = RemoveCommasAndWhiteSpaces(jsonString); //A method that cleans up the json string.

            PopulateTable(jsonString);
        }
        else
        {
            title.text = "Json file missing"; //Lets the user know there's no file to read.
        }
    }

    private string RemoveCommasAndWhiteSpaces(string jsonString)
    {
        jsonString = Regex.Replace(jsonString, @"(""[^""\\]*(?:\\.[^""\\]*)*"")|\s+", "$1"); //A regular expression that removes white spaces from everywhere except between quotes.

        jsonString = jsonString.Replace(",]", "]"); //Removes commas so JsonUtility can read the string.
        jsonString = jsonString.Replace(",}", "}");

        return jsonString;
    }

    private void PopulateTable(string jsonString)
    {
        MainStruct mainStruct = JsonUtility.FromJson<MainStruct>(jsonString); //Fill up the struct.

        title.text = mainStruct.Title; //The table's title.

        ColumnsAndCells(mainStruct.ColumnHeaders, jsonString); //Instantiates columns and fills them up with their corresponding cells.
    }

    private void ColumnsAndCells (string[] columnHeaders, string jsonString)
    {
        string dataString = jsonString.Substring(jsonString.IndexOf("\"" + "Data" + "\":[") + 8); //Removes the title and column headers from the string for further splitting.

        string[] dataStrings = dataString.Split(new string[] { "{" , "},", "]}", "}" }, StringSplitOptions.RemoveEmptyEntries); //Splits the data array in as many elemnts as it has, removing brackets.

        foreach (string columnHeader in columnHeaders)
        {
            GameObject newColumn = Instantiate(columnPrefab, columnsTransform); //Columns are fitted within the table's layout group as they are instantiated.

            newColumn.GetComponentInChildren<Text>().text = columnHeader; //Column headers are given names.

            foreach (string dataElement in dataStrings)
            {
                string headerWithSymbols = "\"" + columnHeader + "\":\""; //Adds symbols for easy removal.

                string cellText = dataElement; //Once everything that doesn't belong is removed, this string will give the cell its text.

                if (!cellText.Contains(headerWithSymbols))
                {
                    cellText = String.Empty; //If the data element does not contain the current header, the cell will be shown empty.
                }
                else
                {
                    cellText = cellText.Substring(cellText.IndexOf(headerWithSymbols) + headerWithSymbols.Length); //Removes the header and leave the field's content.

                    cellText = cellText.Remove(cellText.IndexOf("\"")); //Removes everything from the field's content onwards.
                }

                GameObject newCell = Instantiate(cellPrefab, newColumn.transform.Find("Cells").transform); //Cells are fitted within the recently instantiated column's layout group.

                newCell.GetComponentInChildren<Text>().text = cellText; //The cell is given its text.
            }
        }
    }
}
