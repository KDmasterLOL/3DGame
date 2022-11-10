using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static CharacterAnimationController;

public class Zombie : MonoBehaviour, IEnemy
{
    private const float MAX_HEALTH = 100, MIN_DISTANCE_FROM_PLAYER = 2, DISTANCE_VIEW_PLAYER = 20, RELOAD_TIME = 3;

    private float _attackReload = RELOAD_TIME, _walkReload = RELOAD_TIME;
    private float _health = MAX_HEALTH;

    [SerializeField] protected Player player;
    private NavMeshAgent navMeshAgent;
    protected CharacterAnimationController _animController;


    protected virtual void Update()
    {
        _attackReload -= Time.deltaTime;
        _walkReload -= Time.deltaTime;
    }

    protected virtual void FixedUpdate()
    {
        Move();
    }


    public void Move()
    {
        var playerPos = player.transform.position;
        var selfPos = transform.position;
        var dist2player = Vector3.Distance(selfPos, playerPos);
        if (IsSeePlayer())
        {
            if (dist2player > MIN_DISTANCE_FROM_PLAYER)
            {
                navMeshAgent.destination = playerPos;
                _animController.SetAnimation(Animations.Walk);
            }
            else
            {
                _animController.SetAnimation(Animations.Walk, false);
                navMeshAgent.destination = selfPos;
                Attack();
            }
        }
        else if (navMeshAgent.destination == selfPos)
        {
            _animController.SetAnimation(Animations.Walk, false);
            WalkAround();
        }

    }
    private void WalkAround()
    {
        if (_walkReload < 0)
        {
            var dest = transform.position;
            dest.x += Random.Range(-5, 5);
            dest.z += Random.Range(-5, 5);
            _walkReload = RELOAD_TIME;
            Walk(dest);
        }
    }
    private void Walk(Vector3 dest)
    {
        navMeshAgent.destination = dest;
        _animController.SetAnimation(Animations.Walk);
    }
    public bool IsSeePlayer()
    {
        var direction2player = player.transform.position - transform.position;
        Physics.Raycast(transform.position, direction2player, out var ray, DISTANCE_VIEW_PLAYER);
        Debug.DrawRay(transform.position, direction2player);
        if (ray.collider != null && ray.collider.CompareTag(Tags.Player))
            return true;
        return false;
    }

    public void Attack()
    {
        if (_attackReload < 0)
        {
            _animController.SetAnimation(Animations.Attack);
            _attackReload = RELOAD_TIME;
        }
    }

    public void TakeHit(float damage)
    {
        _health -= damage;
        if (_health <= 0)
            Die();
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        _animController = new(GetComponent<Animator>());
    }
}
