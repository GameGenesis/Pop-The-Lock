using UnityEngine;

public static class LevelManager
{
    public static int totalLevelCount = 40;
    public static float colorSpectrumRange = 10f;
    public static AnimationCurve levelCurve = AnimationCurve.Linear(1f, 0.2f, totalLevelCount, 1f);
    public static AnimationCurve colorCurve = AnimationCurve.Linear(0f, 0.1f, colorSpectrumRange, 1f);

    public static LevelProperties GetLevelProperties(int level)
    {
        LevelProperties levelProperties = new LevelProperties();
        float levelCurveFactor = levelCurve.Evaluate(level);

        levelProperties.difficultyFactor = levelCurveFactor;
        levelProperties.levelColor = Color.HSVToRGB(colorCurve.Evaluate(level % colorSpectrumRange), 0.6f, 0.6f);
        levelProperties.maxTurns = level;
        levelProperties.startingVelocity = Mathf.Max(60, 180 * levelCurveFactor);
        levelProperties.maxVelocity = Mathf.Max(90, 300 * levelCurveFactor);
        levelProperties.minAngleOffset = Mathf.RoundToInt(30f - (10f * levelCurveFactor));
        levelProperties.maxAngleOffset = Mathf.RoundToInt(60f - (30f * levelCurveFactor));

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