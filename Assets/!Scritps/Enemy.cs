using UnityEngine;


public abstract class Enemy : Ship
{

    public enum EnemyState
    {

        Flying,

        Diving
    }

    [SerializeField] private int points = 100;
    // How close the player has to be before this enemy fires
    [SerializeField] private float attackRange = 4f;
    private EnemyState state = EnemyState.Flying;
    // Which way it's diving; locked in when the dive starts
    private Vector2 diveDirection = Vector2.down;
    // True once it has fired during this dive, so it only fires once
    private bool hasFired;

    // Runs every frame
    private void Update()
    {
        if (state == EnemyState.Diving)
        {
            DiveMove();

            // Close enough and hasn't fired yet: fire once
            if (!hasFired && IsPlayerClose())
            {
                // Fire at the player
                Shoot();
                // Don't fire again this dive
                hasFired = true;
            }

            // Flew off the bottom of the screen: remove it (no points)
            if (transform.position.y < -6f)
            {
                // Tell GameManager it's gone
                GameManager.Instance.EnemyDestroyed(this);
                // Remove it from the scene
                Destroy(gameObject);
            }
        }
    }


    public void StartDive()
    {

        state = EnemyState.Diving;
        // Lock on to where the player is right now
        diveDirection = DirectionToPlayer();
    }

    // Works out which way the player is from this enemy, one unit long
    protected Vector2 DirectionToPlayer()
    {
        // Ask GameManager for the player
        Player player = GameManager.Instance.GetPlayer();

        // The player has been destroyed: aim straight down instead
        if (player == null)
        {
            // Straight down
            return Vector2.down;
        }

        // Player's position minus ours points from us to the player; normalized makes it one unit long
        return (player.transform.position - transform.position).normalized;
    }

    // True if the player exists and is within attackRange of this enemy
    protected bool IsPlayerClose()
    {
        // Ask GameManager for the player
        Player player = GameManager.Instance.GetPlayer();

        // No player: nothing is close
        if (player == null)
        {
            // Not close
            return false;
        }

        // Close if the distance between us is less than the range
        return Vector2.Distance(transform.position, player.transform.position) < attackRange;
    }


    protected virtual void DiveMove()
    {

        // Dive toward where the player was when the dive started
        Move(diveDirection);
    }


    public override void Shoot()
    {

        // Fire from the enemy's position, aimed at the player
        weapon.Shoot(transform.position, DirectionToPlayer());
    }


    public override void Die()
    {
        // Static class: no object needed, just the class name
        Score.AddPoints(points);
        // Static Instance: reach the one GameManager without a reference slot
        GameManager.Instance.EnemyDestroyed(this);
        // base.Die() runs Ship's version, which destroys the GameObject
        base.Die();
    }
}