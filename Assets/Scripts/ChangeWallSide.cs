using UnityEngine;

public class ChangeWallSide : MonoBehaviour
{
    public static void ChangeWall(Transform _transform, Vector2 maxVal, Vector2 minVal)
    {
        if (_transform.position.x > maxVal.x)
            _transform.position = new Vector3(minVal.x, -_transform.position.y, _transform.position.z);
        else if (_transform.position.x < minVal.x)
            _transform.position = new Vector3(maxVal.x, _transform.position.y, _transform.position.z);

        if (_transform.position.y > maxVal.y)
            _transform.position = new Vector3(_transform.position.x, minVal.y, _transform.position.z);
        else if (_transform.position.y < minVal.y)
            _transform.position = new Vector3(_transform.position.x, maxVal.y, _transform.position.z);

    }
}
