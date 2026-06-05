using System.Collections.Generic;
using UnityEngine;

public static class EnemySpawnPositionResolver
{
    public static Vector3 GetSpawnPosition(
        Transform playerTarget,
        Transform fallbackTransform,
        float spawnRadius,
        int spawnPositionAttempts,
        bool restrictSpawnToMapBounds,
        Collider2D mapBoundsCollider,
        Vector2 spawnAreaCenter,
        Vector2 spawnAreaSize,
        float mapBoundsPadding)
    {
        Vector3 center = playerTarget != null ? playerTarget.position : fallbackTransform.position;
        for (int attempt = 0; attempt < spawnPositionAttempts; attempt++)
        {
            Vector2 direction = Random.insideUnitCircle.normalized;
            if (direction.sqrMagnitude <= 0.0001f)
            {
                direction = Vector2.right;
            }

            Vector3 candidate = center + (Vector3)(direction * spawnRadius);
            if (!restrictSpawnToMapBounds || IsInsideMapBounds(
                candidate,
                restrictSpawnToMapBounds,
                mapBoundsCollider,
                spawnAreaCenter,
                spawnAreaSize,
                mapBoundsPadding))
            {
                return candidate;
            }
        }

        Vector2 fallbackDirection = Random.insideUnitCircle.normalized;
        if (fallbackDirection.sqrMagnitude <= 0.0001f)
        {
            fallbackDirection = Vector2.right;
        }

        return ClampToMapBounds(
            center + (Vector3)(fallbackDirection * spawnRadius),
            restrictSpawnToMapBounds,
            mapBoundsCollider,
            spawnAreaCenter,
            spawnAreaSize,
            mapBoundsPadding);
    }

    public static List<Vector3> BuildSingleSpawnPositions(
        Vector3 center,
        int batchSize,
        bool restrictSpawnToMapBounds,
        Collider2D mapBoundsCollider,
        Vector2 spawnAreaCenter,
        Vector2 spawnAreaSize,
        float mapBoundsPadding)
    {
        var positions = new List<Vector3>(batchSize);
        Vector3 clampedCenter = ClampToMapBounds(
            center,
            restrictSpawnToMapBounds,
            mapBoundsCollider,
            spawnAreaCenter,
            spawnAreaSize,
            mapBoundsPadding);

        for (int index = 0; index < batchSize; index++)
        {
            positions.Add(clampedCenter);
        }

        return positions;
    }

    public static List<Vector3> BuildGroupSpawnPositions(
        Vector3 center,
        int batchSize,
        float spreadRadius,
        bool restrictSpawnToMapBounds,
        Collider2D mapBoundsCollider,
        Vector2 spawnAreaCenter,
        Vector2 spawnAreaSize,
        float mapBoundsPadding)
    {
        var positions = new List<Vector3>(batchSize);
        if (batchSize <= 0)
        {
            return positions;
        }

        if (batchSize == 1 || spreadRadius <= 0f)
        {
            positions.Add(ClampToMapBounds(
                center,
                restrictSpawnToMapBounds,
                mapBoundsCollider,
                spawnAreaCenter,
                spawnAreaSize,
                mapBoundsPadding));
            return positions;
        }

        float angleOffset = Random.Range(0f, 360f);
        for (int index = 0; index < batchSize; index++)
        {
            float angle = angleOffset + (360f * index / batchSize);
            float radians = angle * Mathf.Deg2Rad;
            float ringRadius = spreadRadius * Random.Range(0.65f, 1f);
            Vector3 offset = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0f) * ringRadius;
            positions.Add(ClampToMapBounds(
                center + offset,
                restrictSpawnToMapBounds,
                mapBoundsCollider,
                spawnAreaCenter,
                spawnAreaSize,
                mapBoundsPadding));
        }

        return positions;
    }

    public static Rect GetMapBoundsRect(
        Collider2D mapBoundsCollider,
        Vector2 spawnAreaCenter,
        Vector2 spawnAreaSize,
        float mapBoundsPadding)
    {
        if (mapBoundsCollider != null)
        {
            Bounds bounds = mapBoundsCollider.bounds;
            float minX = bounds.min.x + mapBoundsPadding;
            float minY = bounds.min.y + mapBoundsPadding;
            float maxX = bounds.max.x - mapBoundsPadding;
            float maxY = bounds.max.y - mapBoundsPadding;
            if (maxX < minX)
            {
                float centerX = bounds.center.x;
                minX = centerX;
                maxX = centerX;
            }

            if (maxY < minY)
            {
                float centerY = bounds.center.y;
                minY = centerY;
                maxY = centerY;
            }

            return Rect.MinMaxRect(minX, minY, maxX, maxY);
        }

        Vector2 halfSize = spawnAreaSize * 0.5f;
        float xMin = spawnAreaCenter.x - halfSize.x + mapBoundsPadding;
        float yMin = spawnAreaCenter.y - halfSize.y + mapBoundsPadding;
        float xMax = spawnAreaCenter.x + halfSize.x - mapBoundsPadding;
        float yMax = spawnAreaCenter.y + halfSize.y - mapBoundsPadding;
        if (xMax < xMin)
        {
            xMin = spawnAreaCenter.x;
            xMax = spawnAreaCenter.x;
        }

        if (yMax < yMin)
        {
            yMin = spawnAreaCenter.y;
            yMax = spawnAreaCenter.y;
        }

        return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
    }

    private static bool IsInsideMapBounds(
        Vector3 position,
        bool restrictSpawnToMapBounds,
        Collider2D mapBoundsCollider,
        Vector2 spawnAreaCenter,
        Vector2 spawnAreaSize,
        float mapBoundsPadding)
    {
        if (!restrictSpawnToMapBounds)
        {
            return true;
        }

        Rect bounds = GetMapBoundsRect(mapBoundsCollider, spawnAreaCenter, spawnAreaSize, mapBoundsPadding);
        return position.x >= bounds.xMin
            && position.x <= bounds.xMax
            && position.y >= bounds.yMin
            && position.y <= bounds.yMax;
    }

    private static Vector3 ClampToMapBounds(
        Vector3 position,
        bool restrictSpawnToMapBounds,
        Collider2D mapBoundsCollider,
        Vector2 spawnAreaCenter,
        Vector2 spawnAreaSize,
        float mapBoundsPadding)
    {
        if (!restrictSpawnToMapBounds)
        {
            return position;
        }

        Rect bounds = GetMapBoundsRect(mapBoundsCollider, spawnAreaCenter, spawnAreaSize, mapBoundsPadding);
        position.x = Mathf.Clamp(position.x, bounds.xMin, bounds.xMax);
        position.y = Mathf.Clamp(position.y, bounds.yMin, bounds.yMax);
        return position;
    }
}
