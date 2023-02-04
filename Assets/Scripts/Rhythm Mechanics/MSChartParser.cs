using System.IO;

using UnityEngine;
using UnityEngine.Windows;

public class MSChartParser : MonoBehaviour
{
    private const string chartsPath = "Assets/Charts";

    public void ParseCharts()
    {
        var info = new DirectoryInfo(chartsPath);
        FileInfo[] files = info.GetFiles();
        foreach (FileInfo f in files)
        {
            Debug.Log(f.Extension);
            if (f.Extension == ".chart")
            {
                Debug.Log($"Parsing File: {f.Name}");
                ParseFile(f);
            }
        }
    }

    public void ParseFile(FileInfo file)
    {
        file.Open(FileMode.Open, FileAccess.Read);


    }
}
