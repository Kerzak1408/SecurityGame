using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Entities.Characters;
using Assets.Scripts.MapEditor;
using UnityEngine;

public class VisualContactGameHandler : BaseGameHandler
{
    private float timeToLaunchBurglar;
    private Camera guardCamera;
    private Plane[] planes;
    private SphereCollider burglarCollider;

    public override string Name
    {
        get { return "Visual Contact"; }
    }

    public override void Start(Game game)
    {
        base.Start(game);
        timeToLaunchBurglar = Random.value * 30 + 15;

        guardCamera = Game.Cameras.First(kvPair => kvPair.Second != null && kvPair.Second is Guard).First;
        
        burglarCollider = Burglar.GetComponent<SphereCollider>();
    }

    public override void Update()
    {
        if (timeToLaunchBurglar > 0)
        {
            timeToLaunchBurglar -= Time.deltaTime;
            if (timeToLaunchBurglar < 0)
            {
                Burglar.ChangePausedState();
            }
        }

        planes = GeometryUtility.CalculateFrustumPlanes(guardCamera);
        if (GeometryUtility.TestPlanesAABB(planes, burglarCollider.bounds))
        {
            Vector3 guardCameraPosition = guardCamera.transform.position;

            Ray ray = new Ray(guardCameraPosition, burglarCollider.transform.position + burglarCollider.center - guardCameraPosition);

            RaycastHit[] hits = Physics.RaycastAll(ray);
            float minDistance = hits.Min(hit => hit.distance);
            RaycastHit closestHit = hits.First(hit => hit.distance == minDistance);
            Debug.Log("Closest hit: " + closestHit.transform.name);
            if (closestHit.collider == burglarCollider)
            {
                Debug.Log("Burglar detected.");
                Game.FinishGame("Burglar detected. Simulation ended.");
            }
        }
    }

    public override void GoalsCompleted(BaseCharacter baseCharacter)
    {
        if (baseCharacter is Burglar)
        {
            Game.FinishGame("Burglar stole all the treasures and escaped.");
        }
    }
}
