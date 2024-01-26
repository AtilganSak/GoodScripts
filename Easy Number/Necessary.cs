using System;

public class Necessary
{
    public static string[] ScoreNames = new string[] { "", "K", "M", "B", "T", "aa", "ab", "ac", "ad", "ae", "af", "ag", "ah", "ai", "aj",
            "ak", "al", "am", "an", "ao", "ap", "aq", "ar", "as", "at", "au", "av", "aw", "ax", "ay", "az", "ba", "bb", "bc",
            "bd", "be", "bf", "bg", "bh", "bi", "bj", "bk", "bl", "bm", "bn", "bo", "bp", "bq", "br", "bs", "bt", "bu", "bv", "bw", "bx", "by", "bz", };
    public static string Convert(double Score)
    {
        string result;
        int i;

        for (i = 0; i < ScoreNames.Length; i++)
            if (Score < 999)
                break;
            else Score = Math.Floor(Score / 100f) / 10f;

        if (Score == Math.Floor(Score))
            result = Score.ToString() + ScoreNames[i];
        else result = Score.ToString("F1") + ScoreNames[i];
        return result;
    }
}
