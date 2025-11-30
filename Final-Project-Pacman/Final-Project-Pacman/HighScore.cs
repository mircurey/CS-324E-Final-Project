using System;
using System.IO;

public class HighScore
{
    private readonly string filePath = "highscore.txt";
    public int Value { get; private set; }

    public HighScore()
    {
        Load();
    }

    public void Load()
    {
        try
        {
            if (File.Exists(filePath))
            {
                string text = File.ReadAllText(filePath);
                Value = int.Parse(text);
            }
            else
            {
                Value = 0;
            }
        }
        catch
        {
            Value = 0; 
        }
    }


    public void Save(int score)
    {
        if (score > Value)
        {
            Value = score;
            File.WriteAllText(filePath, Value.ToString());
        }
    }
}
