using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PositionTools
{
    static public Vector3 GetSpiralPosition(Vector3 startPosition, int index, float angleMultiplier = 40f, float initialDistance = 2f, float distanceMultiplier = 0.5f)
    {
        Vector3 spawnPosition = startPosition;
        if (index == 0)
            return spawnPosition;

        Vector3 offset = new Vector3(0f, 0f, (index * distanceMultiplier) + initialDistance);
        float degrees = angleMultiplier * index;
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float ty = offset.z;
        offset.x =  (sin * ty);
        offset.z =  (cos * ty);

        return spawnPosition + offset;
    }
}
