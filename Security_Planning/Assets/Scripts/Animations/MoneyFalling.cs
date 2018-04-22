using UnityEngine;

namespace Assets.Scripts.Animations
{
    public class MoneyFalling : MonoBehaviour
    {
        private void Update ()
        {
            // Rotate each money randomly and let it fall. When it is too low, return it up.
            foreach (Transform child in transform)
            {
                float xRotation = -Random.value;
                float yRotation = Random.value;
                float zRotation = Random.value;
                child.position += Time.deltaTime * Vector3.down;
                child.Rotate(360 * Time.deltaTime * new Vector3(xRotation, yRotation, zRotation));
                if (child.localPosition.z > 1.5f)
                {
                    child.localPosition -= 3 * Vector3.forward;
                }
            }
        }
    }
}
