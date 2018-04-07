using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities.Characters;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Characters.Goals;
using Entities.Characters.Goals;
using Assets.Scripts.Extensions;
using UnityEngine;

namespace Entities.Characters.Behaviours
{
    public class CollectEverythingBehaviour : BaseBehaviour
    {
        private Queue<BaseGoal> goals;
        private BaseGoal currentGoal;

        public CollectEverythingBehaviour(BaseCharacter character) : base(character)
        {
        }

        public override void Start()
        {
            goals = GenerateGoals(character);
        }

        public static Queue<BaseGoal> GenerateGoals(BaseCharacter character, bool navigateBack=true)
        {
            Map currentMap = character.Map;
            var result = new Queue<BaseGoal>();
            IEnumerable<GameObject> moneyEntities = currentMap.Entities.Where(go => go.HasScriptOfType<MoneyEntity>());
            IOrderedEnumerable<GameObject> orderedEntities = moneyEntities.OrderBy(
                entity => Vector3.Distance(character.transform.position, entity.transform.position));
            foreach (GameObject moneyObject in orderedEntities)
            {
                TileNode closestTile = currentMap.GetClosestTile(moneyObject.transform.position);
                result.Enqueue(new MoneyGoal(character, closestTile.Position, moneyObject));
            }

            if (navigateBack)
            {
                result.Enqueue(new NavigationGoal(character, character.Position));
            }
            return result;
        }

        public override void Update()
        {
            if (currentGoal == null || currentGoal.IsFinished)
            {
                if (goals.Count > 0)
                {
                    currentGoal = goals.Dequeue();
                    currentGoal.Activate();
                }
                else
                {
                    character.GoalsCompleted();
                    currentGoal = null;
                    return;
                }
            }
            currentGoal.Update();
        }
    }
}
