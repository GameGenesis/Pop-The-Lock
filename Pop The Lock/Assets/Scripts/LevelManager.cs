using UnityEngine;

public static class LevelManager
{
    public static int totalLevelCount = 5;
    public static AnimationCurve levelCurve = AnimationCurve.Linear(1f, 0.2f, totalLevelCount, 1f);

    public static LevelProperties GetLevelProperties(int level)
    {
        LevelProperties levelProperties = new LevelProperties();
        float difficultyFactor = levelCurve.Evaluate(level);

        levelProperties.difficultyFactor = difficultyFactor;
        levelProperties.levelColor = Color.HSVToRGB(difficultyFactor, 0.4f, 0.4f);
        levelProperties.maxTurns = Mathf.RoundToInt(20 * difficultyFactor / 5) * 5;
        levelProperties.startingVelocity = Mathf.Max(60, 150 * difficultyFactor);
        levelProperties.maxVelocity = Mathf.Max(80, 300 * difficultyFactor);
        levelProperties.minAngleOffset = Mathf.RoundToInt(30f - (10f * difficultyFactor));
        levelProperties.maxAngleOffset = Mathf.RoundToInt(60f - (30f * difficultyFactor));

        return levelProperties;
    }
}

public struct LevelProperties
{
    public float difficultyFactor;

    public Color levelColor;

    public int maxTurns;

    public float startingVelocity;
    public float maxVelocity;

    public int minAngleOffset;
    public int maxAngleOffset;
}