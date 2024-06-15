using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameUtility
{
    public static int GetCardTypeMask(CardType cardType)
    {
        if (!Enum.IsDefined(typeof(CardType), cardType)) return 0;
        return 1 << (int)cardType;
    }

    public static int GetCardTypeMask(CardType[] cardType)
    {
        if (cardType == null || cardType.Length == 0)
        {
            return 0;
        }

        int mask = 0;
        foreach (CardType type in cardType)
        {
            mask |= GetCardTypeMask(type);
        }

        return mask;
    }

    public static int RemoveFromMask(int mask, CardType removeType)
    {
        int removeMask = GetCardTypeMask(removeType);
        return RemoveFromMask(mask, removeMask);
    }

    public static int RemoveFromMask(int mask, int removeMask)
    {
        if (removeMask == 0)
        {
            return mask;
        }

        return mask & ~removeMask;
    }

    public static bool CheckCardTypeMask(int mask, CardType checkType, bool anyMask)
    {
        int checkMask = GetCardTypeMask(checkType);
        return CheckCardTypeMask(mask, checkMask, anyMask);
    }

    public static bool CheckCardTypeMask(int mask, int checkMask, bool anyMask)
    {
        int result = mask & checkMask;

        if (anyMask)
        {
            return result != 0;
        }
        else
        {
            return result == mask;
        }
    }

    public static Vector3 GetRandomScatterPosition(Vector3 initialPosition, float maxRadius = 1)
    {
        if (maxRadius <= 0f)
        {
            return initialPosition;
        }

        float randomRadius = Random.Range(0f, maxRadius);
        Vector3 randomScatter = Random.insideUnitCircle * randomRadius;

        return initialPosition + randomScatter;
    }

    public static Vector3 ClampPointInBounds(Bounds bounds, Vector3 point)
    {
        if (bounds == null || bounds.size == Vector3.zero) return point;

        Vector3 clampped = Vector3.zero;
        clampped.x = Mathf.Clamp(point.x, bounds.min.x, bounds.max.x);
        clampped.y = Mathf.Clamp(point.y, bounds.min.y, bounds.max.y);
        clampped.z = Mathf.Clamp(point.z, bounds.min.z, bounds.max.z);

        return clampped;
    }

    public static void SwitchScene(string sceneName, bool isAdditve = false)
    {
        LoadSceneMode mode = isAdditve ? LoadSceneMode.Additive : LoadSceneMode.Single;
        SceneManager.LoadScene(sceneName, mode);
    }

    public static void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public static void UnloadScene(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        SceneManager.UnloadSceneAsync(scene);
    }
}
