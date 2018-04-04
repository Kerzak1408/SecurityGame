using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Extensions;
using Assets.Scripts.MapEditor;
using UnityEngine;

public abstract class BaseGameHandler : BaseHandler
{
    protected Game Game;
    protected Burglar Burglar;

    public virtual void Start(Game game)
    {
        Game = game;
        Burglar = Game.Map.Entities.First(entity => entity.HasScriptOfType<Burglar>()).GetComponent<Burglar>();
    }

    public abstract void Update();

    public virtual void GoalsCompleted(BaseCharacter baseCharacter)
    {
    }
}
