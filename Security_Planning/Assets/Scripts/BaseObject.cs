using UnityEngine;

namespace Assets.Scripts
{
    public abstract class BaseObject : MonoBehaviour
    {
        private bool destroy;
        private float timeCounter;
        private float timeOfDestroy;

        public virtual void StartGame() { }

        protected virtual void Start() { }

        protected virtual void Update ()
        {
            Debug.Log("name = " + name + " Destroy = " + destroy);
            if (destroy)
            {
                Debug.Log("Destroy timeCounter = " + timeCounter + " timeOfDestroy = " + timeOfDestroy);
                if (timeCounter > timeOfDestroy)
                {
                    Destroy(gameObject);
                }
                else
                {
                    timeCounter += Time.deltaTime;
                }
            }
        }

        protected void DestroyAfterTimeout(float timeout)
        {
            destroy = true;
            timeOfDestroy = timeout;
        }
    }
}
