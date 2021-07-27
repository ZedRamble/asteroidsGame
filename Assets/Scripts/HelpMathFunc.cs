using UnityEngine;

public class HelpMathFunc : MonoBehaviour
{
    public static float GetPercent(float x, int percent)
    {
        return (x / 100) * percent;
    }

    public static Vector2 GetCoordinatesCalcAngle(Vector2 pos, float angle)
    {
        float cs = Mathf.Cos(angle);
        float sn = Mathf.Sin(angle);

        float rx = pos.x * cs - pos.y * sn;
        float ry = pos.x * sn + pos.y * cs;
        Vector2 dirBullet = new Vector2(rx, ry);
        return dirBullet;
    }
}
