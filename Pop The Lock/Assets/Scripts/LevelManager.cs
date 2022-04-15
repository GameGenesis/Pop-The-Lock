using UnityEngine;

public static class LevelManager
{
    public static int totalLevelCount = 80;
    public static float colorSpectrumRange = 10f;
    public static AnimationCurve levelCurve = AnimationCurve.Linear(1f, 0.2f, totalLevelCount, 1f);
    public static AnimationCurve colorCurve = AnimationCurve.Linear(0f, 0.2f, colorSpectrumRange, 1f);

    public static LevelProperties GetLevelProperties(int level)
    {
        LevelProperties levelProperties = new LevelProperties();
        float levelCurveFactor = levelCurve.Evaluate(level);

        levelProperties.difficultyFactor = levelCurveFactor;
        levelProperties.levelColor = Color.HSVToRGB(colorCurve.Evaluate(level % colorSpectrumRange), 0.6f, 0.6f);
        levelProperties.maxTurns = Mathf.Min(totalLevelCount, level);
        levelProperties.velocityMultiplier = 1.045f - (0.02f * levelCurveFactor);
        levelProperties.startingVelocity = 40 + 135 * levelCurveFactor;
        levelProperties.maxVelocity = 90 + 185 * levelCurveFactor;
        levelProperties.minAngleOffset = Mathf.RoundToInt(35f - (10f * levelCurveFactor));
        levelProperties.maxAngleOffset = Mathf.RoundToInt(55f - (30f * levelCurveFactor));

        return levelProperties;
    }
}

public struct LevelProperties
{
    public float difficultyFactor;

    public Color levelColor;

    public int maxTurns;

    public float velocityMultiplier;
    public float startingVelocity;
    public float maxVelocity;

    public int minAngleOffset;
    public int maxAngleOffset;
}