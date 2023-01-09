using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using Microsoft.VisualBasic.FileIO;

public static class CsvUtils
{
    public static string[][] CsvToArray(string filePath)
    {
        List<string[]> rows = new List<string[]>();

        using (TextFieldParser parser = new TextFieldParser(filePath))
        {
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            while (!parser.EndOfData) 
            {
                rows.Add(parser.ReadFields());
            }
        }

        var data = rows.ToArray();
        
        string[][] transposeData = new string[data[0].Length][];

        for (int i = 0; i < data[0].Length; i++)
        {
            transposeData[i] = new string[data.Length];
        }

        for (int i = 0; i < data.Length; i++)
        {
            for (int j = 0; j < data[i].Length; j++)
            {
                transposeData[j][i] = data[data.Length - i - 1][j];
            }
        }
        
        return transposeData;
    }

    public static string GetLevelFilePath(string levelName)
    {
        var fullFilePath = Application.dataPath + "/Levels/" + levelName;
        return fullFilePath;
    }
    
}
