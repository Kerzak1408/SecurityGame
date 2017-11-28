using UnityEngine;

namespace Assets.Scripts.Animations
{
    public class MoneyFalling : MonoBehaviour
    {
	
        void Update ()
        {
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
