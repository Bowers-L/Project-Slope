using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class MSChartParser : MonoBehaviour
{
    private const string chartsPath = "Assets/Charts";

    public void ParseCharts()
    {
        var info = new DirectoryInfo(chartsPath);
        FileInfo[] files = info.GetFiles();
        foreach (FileInfo f in files)
        {
            if (f.Extension == ".asset") {
                f.Delete();
            }
            if (f.Extension == ".chart")
            {
                Debug.Log($"Parsing File: {f.Name}");
                ParseFile(f);
            }
        }
    }

    //L: This is the wettest code you've ever seen, cuz it ain't DRY.
    public void ParseFile(FileInfo file)
    {
        FileStream stream = file.Open(FileMode.Open, FileAccess.Read);

        using (StreamReader sr = new StreamReader(stream))
        {
            float bpm = 0;
            int timeSignatureNum = 0;
            List<Note> notes = new List<Note>();
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();

                Debug.Log(line);

                if (line.Equals("[SyncTrack]"))
                {
                    sr.ReadLine();  //{
                    while (!line.Equals("}") && !sr.EndOfStream)
                    {
                        line = sr.ReadLine();
                        Debug.Log(line);
                        string[] tokens = line.Split(' ');
                        Debug.Log("SPLITTING INTO TOKENS");
                        for (int i = 0; i < tokens.Length; i++)
                        {
                            Debug.Log(tokens[i]);
                            if (tokens[i].Equals("TS"))
                            {
                                timeSignatureNum = int.Parse(tokens[i+1]);
                            } else if (tokens[i].Equals("B"))
                            {
                                bpm = 0.001f * int.Parse(tokens[i+1]);
                            }
                        }
                    }
                } else if (line.Equals("[ExpertSingle]"))
                {
                    sr.ReadLine();  //skip {
                    line = sr.ReadLine();
                    while (!line.Equals("}") && !sr.EndOfStream)
                    {
                        Debug.Log(line);
                        string[] tokens = line.Split(' ');
                        Debug.Log("SPLITTING INTO TOKENS");
                        int startI = 0;
                        for (int i = 0; i < tokens.Length; i++)
                        {
                            if (!tokens[i].Equals(""))
                            {
                                startI = i;
                                break;
                            }
                        }

                        //Debug.Log(tokens[startI]);
                        //Debug.Log(tokens[startI + 3]);
                        //Debug.Log(tokens[startI + 4]);
                        int moment = int.Parse(tokens[startI]);
                        int pitch = int.Parse(tokens[startI + 3]);
                        int length = int.Parse(tokens[startI + 4]);


                        Note newNote = new Note(moment, pitch, length);
                        notes.Add(newNote);
                        line = sr.ReadLine();
                    }
                }
            }

            //Print all the parameters so I know the file parsed right.
            Debug.Log($"BPM: {bpm}");
            Debug.Log($"Time Signature: {timeSignatureNum} over 4");
            foreach (Note note in notes)
            {
                Debug.Log(note.Description);
            }

            ChartData chart = ScriptableObject.CreateInstance<ChartData>();
            chart.bpm = bpm;
            chart.timeSignatureNum = timeSignatureNum;
            chart.notes = notes;
            chart.unitsPerBeat = Note.QUARTER;
            
            AssetDatabase.CreateAsset(chart, chartsPath + '/' + Path.GetFileNameWithoutExtension(file.Name) + ".asset");
        }
    }
}