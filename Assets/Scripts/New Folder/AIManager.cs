using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.PlayerLoop;


[RequireComponent(typeof(Animator))]
public class AIManager : MonoBehaviour
{
    public ScriptableValue<float> sanityLevel;
    public ScriptableValue<Vector2> playerPosition;
    public ScriptableValueField<float> moveSpeed;
    public ScriptableValueField<float> runSpeed;
    public ScriptableValueField<Vector2> minMaxWaitTime;
    public ScriptableValueField<float> delayBeforeNewPath;
    public ScriptableValueField<float> maxOnPathTime;
    public ScriptableValueField<float> maxSearchRadius;
    public ScriptableValueField<float> interractRadius;
    public ScriptableValueField<float> sanityDamangePerSecond;
    public ScriptableValueField<float> health;
    public ScriptableValueField<float> sanityPunishmentForKill;
    public LayerMask wallLayer;
    public Pool bloodPool;

    [SerializeField] private RuntimeAnimatorController goodAnimation;
    [SerializeField] private RuntimeAnimatorController badAnimation;
    [SerializeField] private CircleCollider2D triggerCollider;

    public Animator Animator {get; private set;}

    public IAstarAI Ai {get; private set;}
    private AgentState currentState;
    private Transform target;
    private float currentHealth;

    private bool CurrentIsGood = true;

    private void Awake()
    {
        currentHealth = health;
        Ai = GetComponent<IAstarAI>();
        Animator = GetComponent<Animator>();

        
    }

    private void Start()
    {
        var probability = Random.value;
        if (probability >= sanityLevel) 
            SwitchState(new AgressiveInitialState());
        else
            SwitchState(new IdleInitialState());        
    }

    public void TrySwitchToBad() 
    {
        var probability = Random.value;
        if (probability >= sanityLevel)
            SwitchState(new AgressiveInitialState());
    }

    public void TrySwitchToGood() 
    {
        var probability = Random.value;
        if (probability < sanityLevel)
            SwitchState(new IdleInitialState());
    }

    public void LaunchCoroutine(IEnumerator process, ref Coroutine coroutine) 
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
        
        coroutine = StartCoroutine(process);
    }

    public void SwitchState(AgentState state) 
    {
        StopAllCoroutines();
        var swap = currentState == null;
        currentState?.Exit();
        Ai.isStopped = true;
        currentState = state;
        currentState.Init(this);
        if ((swap || currentState.isGood != CurrentIsGood) && currentState.isGood)
            ChangeVisualsToIdle();
        else if (!currentState.isGood)
            ChangeVisualsToAgressive();
    }

    private void ChangeVisualsToIdle() 
    {
        Animator.runtimeAnimatorController = goodAnimation;
        CurrentIsGood = true;
    }

    private void ChangeVisualsToAgressive() 
    {
        Animator.runtimeAnimatorController = badAnimation;
        CurrentIsGood = false;
    }

    public void SetSpeed(float speed) 
    {
        Ai.maxSpeed = speed;
    }

    public void Damage(float damage) 
    {
        currentHealth -= damage;
        bloodPool.Get<PooledItem>(transform.position, transform.rotation);
        if (currentHealth < 0) 
        {
            Die();
        }
    }

    private void OnDestroy()
    {
        currentState?.Exit();
    }

    private void Die() 
    {
        if (CurrentIsGood) 
        {
            sanityLevel.Value -= sanityPunishmentForKill;
        }

        Destroy(gameObject);
    }

    public IEnumerator GoTo(Vector2 position) 
    {
        target = null;
        Ai.isStopped = false;
        Ai.destination = new Vector3(position.x, position.y, transform.position.z);
        yield return new WaitForSeconds(delayBeforeNewPath.Value);

        var time = Time.time;
        do {
            Ai.destination =  new Vector3(position.x, position.y, transform.position.z);
            yield return null;
        } while (time + maxOnPathTime > Time.time && !Ai.reachedDestination);
        Ai.isStopped = true;
    }

    public IEnumerator GoTo(Transform target) 
    {
        this.target = target;
        Ai.isStopped = false;
        Ai.destination = new Vector3(target.position.x, target.position.y, transform.position.z);
        var time = Time.time;
        do {
            Ai.destination =  new Vector3(target.position.x, target.position.y, transform.position.z);
            yield return null;
        } while(time + delayBeforeNewPath > Time.time);


        time = Time.time;
        do {
            Ai.destination =  new Vector3(target.position.x, target.position.y, transform.position.z);
            yield return null;
        } while (time + maxOnPathTime > Time.time && !Ai.reachedDestination);
        Ai.isStopped = true;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.TryGetComponent(out AIManager otherAi)) 
        {
            Ai.Move((transform.position - otherAi.transform.position).normalized * triggerCollider.radius * 0.5f * Time.fixedDeltaTime);
        }
        var player = other.GetComponentInParent<PlayerTracker>();
        if (player != null) 
        {
            if (!CurrentIsGood)
                sanityLevel.Value -= sanityDamangePerSecond * Time.fixedDeltaTime;
            Ai.Move((transform.position - player.transform.position).normalized * triggerCollider.radius * Time.fixedDeltaTime);
        }
    }

    private void Update()
    {
        Animator.SetFloat("MoveSpeed", Ai.velocity.magnitude);
        currentState.Update();
    }

    private void OnDrawGizmosSelected()
    {
        if (maxSearchRadius.HasValue) 
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, maxSearchRadius);
        }

        if (interractRadius.HasValue) 
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interractRadius);
        }
    }
}
