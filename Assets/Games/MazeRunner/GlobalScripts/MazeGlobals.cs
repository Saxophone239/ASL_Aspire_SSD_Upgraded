using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGlobals
{
    public enum MazeRunnerDifficulty
    {
        Easy,
        Medium,
        Hard
    }

    public enum Vocab
    {
        Chemistry,
        Biology,
        FoodWeb,
        PartsOfTheCell
    }

    public static TextAsset vocabJson;
    public static MazeRunnerDifficulty difficulty = MazeRunnerDifficulty.Hard;
    public static Vocab vocabList = Vocab.Chemistry;

}
