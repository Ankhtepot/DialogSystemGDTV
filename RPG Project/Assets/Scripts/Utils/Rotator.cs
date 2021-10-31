using UnityEngine;

namespace Utils
{
    public class Rotator : MonoBehaviour
    {
        [SerializeField] private GameObject rotatedObject;
        [SerializeField] private ERotationAxis rotateAroundAxis = ERotationAxis.X;
        [SerializeField] private float rotationSpeed = 10;

        private enum ERotationAxis
        {
            X, Y, Z
        }

        private void Update()
        {
            float speed = rotationSpeed * Time.deltaTime;
            rotatedObject.transform.Rotate(
                rotateAroundAxis == ERotationAxis.X ? speed : 0,
                rotateAroundAxis == ERotationAxis.Y ? speed : 0,
                rotateAroundAxis == ERotationAxis.Z ? speed : 0
            );
        }
    }
}
