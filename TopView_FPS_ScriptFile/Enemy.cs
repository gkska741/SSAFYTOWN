using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof (NavMeshAgent))]
public class Enemy : LivingEntity
{
    public enum State {Idle, Chasing, Attacking};
    State currentState;

    public GameObject deathEffect;
    public static event System.Action OndeathStatic;

    NavMeshAgent pathfinder;
    Transform target;
    LivingEntity targetEntity;

    float attackDistanceThreshold = 1.5f;
    float timeBetweenAttacks = 1;
    float damage = 1;

    float nextAttackTime;
    float myCollisionRadius;
    float targetCollisionRadius;

    bool hasTarget;

    void Awake()
    {
        pathfinder = GetComponent<NavMeshAgent>();

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            hasTarget = true;
            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetEntity = target.GetComponent<LivingEntity>();

            targetCollisionRadius = GetComponent<CapsuleCollider>().radius;
            myCollisionRadius = target.GetComponent<CapsuleCollider>().radius;
        }
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        pathfinder = GetComponent<NavMeshAgent>();

        if (hasTarget)
        {
            currentState = State.Chasing;
            targetEntity.OnDeath += OnTargetDeath;
            StartCoroutine(UpdatePath());
        }
    }

    public override void TakeHit (float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        AudioManager.instance.PlaySound("Impact", transform.position);
        if (damage >= health) 
        {
            if (OndeathStatic != null)
            {
                OndeathStatic();
            }
            AudioManager.instance.PlaySound("Enemy Death", transform.position);
            Destroy(Instantiate(deathEffect, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection))as GameObject, 2f);
        }
        base.TakeHit(damage, hitPoint, hitDirection);
    }

    public void SetCharacteristics(float moveSpeed, int hitsToKillPlayer, float enemyHealth)
    {
        pathfinder.speed = moveSpeed;
        if (hasTarget)
        {
            damage = (int)Mathf.Ceil(targetEntity.startingHealth / hitsToKillPlayer);
        }
        startingHealth = enemyHealth;
    }
    void OnTargetDeath()
    {
        hasTarget = false;
        currentState = State.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        if (hasTarget)
        {
            if (Time.time > nextAttackTime)
            {
                float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;

                if (sqrDstToTarget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2))
                {
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    AudioManager.instance.PlaySound("Enemy Attack", transform.position);
                    StartCoroutine(Attack());
                }
            }
        }
    }
    IEnumerator Attack()
    {
        currentState = State.Attacking;
        pathfinder.enabled = false;

        Vector3 originalPosition = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - dirToTarget * (myCollisionRadius + attackDistanceThreshold / 2);

        float attackSpeed = 3;
        float percent = 0;

        bool hasAppliedDamage = false;
        while (percent <= 1)
        {
            if (percent >= .5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                targetEntity.TakeDamage(damage);                
            }
            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            if (hasTarget)
            {
                transform.LookAt(target.position);
            }
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);
            yield return null;
        }
        currentState = State.Chasing;
        pathfinder.enabled = true;
    }
    IEnumerator UpdatePath()
    {
        float refreshRate = 0.5f;

        while (hasTarget)
        {
            if (currentState == State.Chasing)
            {
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold/2);
                if (!dead)
                {
                    pathfinder.SetDestination(targetPosition);
                }
            }
            yield return new WaitForSeconds(refreshRate);
        }

    }
}
