using UnityEngine;

public class Enemy : MonoBehaviour {

    float probability;
    Vector3 directionVector;
    CharacterController controller;

	// Use this for initialization
	void Start () {
        probability = 0;
        directionVector = new Vector3(Random.value - 0.5f, Random.value - 0.5f,- 1);
        controller = gameObject.AddComponent<CharacterController>();
        controller.radius = 0.3f;
        transform.position = new Vector3(transform.position.x, transform.position.y, -1);
	}
	
	// Update is called once per frame
	void Update () {
        probability += Time.deltaTime/30;
        if (Random.value < probability)
        {
            directionVector = new Vector3(Random.value - 0.5f, Random.value - 0.5f, 0);
        }
        controller.Move(2.1f * Time.deltaTime * directionVector);
	}
}
