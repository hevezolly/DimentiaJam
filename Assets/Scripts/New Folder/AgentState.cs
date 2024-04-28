using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AgentState
{
    public abstract bool isGood {get;}
    protected AIManager Manager {get; private set;}

    private float lastCheckSanityLevel;

    public void Init(AIManager manager) 
    {
        Manager = manager;
        lastCheckSanityLevel = Manager.sanityLevel;
        Manager.sanityLevel.ValueChangeEvent.AddListener(OnSanityChanged);
        OnInit();
    }

    private void OnSanityChanged(float newSanity) 
    {
        var difference = Mathf.Abs(newSanity - lastCheckSanityLevel);
        if (difference < 0.15)
            return;
        if (isGood && newSanity < lastCheckSanityLevel) 
            Manager.TrySwitchToBad();
        else if (!isGood && newSanity > lastCheckSanityLevel)
            Manager.TrySwitchToGood();
        lastCheckSanityLevel = newSanity;        
    }

    protected virtual void OnInit() {}

    public virtual void Update() {}

    protected virtual void OnExit() {}

    public void Exit() 
    {
        OnExit();
        Manager.sanityLevel.ValueChangeEvent.RemoveListener(OnSanityChanged);
    }
}

public class IdleInitialState: AgentState {
    
    private Coroutine behaviour;

    public override bool isGood => true;

    protected override void OnInit()
    {
        Manager.SetSpeed(Manager.moveSpeed);
        Manager.LaunchCoroutine(Behaviour(), ref behaviour);
    }

    private IEnumerator Behaviour() 
    {
        do 
        {
            var position = Random.insideUnitCircle * Manager.maxSearchRadius;

            yield return Manager.GoTo(position);

            yield return new WaitForSeconds(Random.Range(Manager.minMaxWaitTime.Value.x, Manager.minMaxWaitTime.Value.y));
        } while (true);
    }

    public override void Update()
    {
        if (Vector2.Distance(Manager.transform.position, Manager.playerPosition) < Manager.interractRadius)
            Manager.SwitchState(new WatchOnPlayer());
    }
}

public class WatchOnPlayer: AgentState 
{
    public override bool isGood => true;

    public override void Update()
    {
        if (Vector2.Distance(Manager.transform.position, Manager.playerPosition) < Manager.interractRadius * 1.6) 
        {
            Manager.Ai.rotation = Quaternion.LookRotation(Vector3.forward, 
                (Vector3)Manager.playerPosition.Value - Manager.transform.position);
        }
        else 
        {
            Manager.SwitchState(new IdleInitialState());
        }
    }
}

public class ChasePlayer : AgentState
{
    public override bool isGood => false;

    protected override void OnInit()
    {
        Manager.Ai.isStopped = false;
        Manager.SetSpeed(Manager.runSpeed);
    }

    public override void Update()
    {
        Manager.Ai.destination = new Vector3(Manager.playerPosition.Value.x, Manager.playerPosition.Value.y, 
            Manager.transform.position.z);

        var hit = Physics2D.Raycast(Manager.transform.position, Manager.playerPosition - (Vector2)Manager.transform.position, Manager.maxSearchRadius, Manager.wallLayer);
        if (hit.collider != null && hit.collider.GetComponentInParent<PlayerTracker>() == null) 
        {
            Manager.SwitchState(new AgressiveInitialState());
        }
        Debug.DrawLine(Manager.transform.position, hit.point, Color.red);
    }
}

public class AgressiveInitialState : IdleInitialState
{
    public override bool isGood => false;

    protected override void OnInit()
    {
        base.OnInit();
        Manager.Ai.isStopped = false;
        Manager.SetSpeed(Manager.runSpeed);
    }

    public override void Update()
    {
        var hit = Physics2D.Raycast(Manager.transform.position, Manager.playerPosition - (Vector2)Manager.transform.position, Manager.maxSearchRadius, Manager.wallLayer);
        if (hit.collider != null && hit.collider.GetComponentInParent<PlayerTracker>() != null) 
        {
            Manager.SwitchState(new ChasePlayer());
        }
        Debug.DrawLine(Manager.transform.position, hit.point, Color.green);
        // Manager.Ai.destination = new Vector3(Manager.playerPosition.Value.x, Manager.playerPosition.Value.y, 
        //     Manager.transform.position.z);
    }
}
