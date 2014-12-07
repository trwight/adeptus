﻿using UnityEngine;
using System.Collections;
using BreadcrumbAi;

[System.Serializable]
public class EnemySounds
{
    public AudioClip audio_hit_1, audio_hit_2, audio_dead_1, audio_dead_2, audio_melee_attack_1, audio_melee_attack_2;
}

public class Enemy : Entity 
{
    public EnemySounds audioClips;
    public enum EnemyType { Melee, Ranged, Special };
    public EnemyType enemyType;

    // used for xp system
    //public float expOnDeath;
    //private PlayerStats player;

    public GameObject bloodPrefab;
    public GameObject specialPrefab;
    private Transform player;
    private Ai ai;
    private AudioSource audioSource;
    private GameController gameController;
    private WeaponOne playerWeaponOne;
    private PlayerStats playerOne;

    //Weapon Components
    public Transform handHold;
    private WeaponOne weaponOne;
    public WeaponOne[] weapons;
    public Rigidbody rangedProjectilePrefab;
    private Animator animator;
    private bool _animAttack;

    // Attack Rates
    private float rangedAttackNext = 0.0f;
    private float rangedAttackRate = 2.0f;
    private float meleeAttackNext = 0.0f;
    private float meleeAttackRate = 1.0f;

    // Health Bar
    private Enemy thisEnemy;
    private float maxHealth;
    private GameGUI gui;
    [HideInInspector]public bool _isHit, _isHitBySpit, _isHitByRock, _isHitByWave, _isHitByBolt, _isHitByRoar, _isHitBySong, _isHitByFireBurst, _isHitByDrain;


    void Start()
    {
        // this player object used to add xp to the player for levelling up
        //player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();

        ai = GetComponent<Ai>();
        animator = GetComponent<Animator>();
        audioSource = gameObject.AddComponent<AudioSource>();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        playerWeaponOne = GameObject.FindGameObjectWithTag("WeaponOne").GetComponent<WeaponOne>();
        playerOne = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        gui = GameObject.FindGameObjectWithTag("GUI").GetComponent<GameGUI>();
        thisEnemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Enemy>();
        maxHealth = thisEnemy.health;
        GameObject go = GameObject.FindGameObjectWithTag("Player");
        if (go)
        {
            player = go.transform;
        }
        EquipWeapon(0);
    }

    void Update()
    {
        EnemyHealthManager();
    }

    void FixedUpdate()
    {
        Attack();
    }

    private void Attack()
    {
        if (player)
        {
            if (ai.lifeState == Ai.LIFE_STATE.IsAlive)
            {
                if (enemyType != EnemyType.Ranged)
                {
                    if (ai.attackState == Ai.ATTACK_STATE.CanAttackPlayer && Time.time > meleeAttackNext)
                    {
                        meleeAttackNext = Time.time + meleeAttackRate;
                        float rand = Random.value;
                        if (rand <= 0.4f)
                        {
                            audioSource.clip = audioClips.audio_melee_attack_1;
                        }
                        else
                        {
                            audioSource.clip = audioClips.audio_melee_attack_2;
                        }
                        audioSource.PlayOneShot(audioSource.clip);
                        player.GetComponent<DemoPlayerControls>()._isHit = true;
                        player.GetComponent<DemoPlayerControls>().Bleed(transform.rotation);
                        _animAttack = true;
                    }
                    else
                    {
                        _animAttack = false;
                    }
                }
                else
                {
                    if (ai.attackState == Ai.ATTACK_STATE.CanAttackPlayer && Time.time > rangedAttackNext)
                    {
                        rangedAttackNext = Time.time + rangedAttackRate;
                        Rigidbody spit = Instantiate(rangedProjectilePrefab, transform.position + transform.forward + transform.up, transform.rotation) as Rigidbody;
                        spit.AddForce(transform.forward * 500);
                        _animAttack = true;
                    }
                    else
                    {
                        _animAttack = false;
                    }
                }
            }
        }
    }

    void EquipWeapon(int i)
    {
        if (weaponOne)
        {
            Destroy(weaponOne.gameObject);

        }

        weaponOne = Instantiate(weapons[i], handHold.position, handHold.rotation) as WeaponOne;
        weaponOne.transform.parent = handHold;
        animator.SetFloat("Weapon ID", weaponOne.weaponID);
    }

    private void EnemyHealthManager()
    {
        if (_isHit)
        {
            thisEnemy.TakeDamage(playerWeaponOne.damage);
            //thisEnemy.TakeDamage(1);
            _isHit = false;
        }

        if (_isHitBySpit)
        {
            thisEnemy.TakeDamage(playerWeaponOne.damage);
            //thisEnemy.TakeDamage(1);
            _isHitBySpit = false;
        }

        if (_isHitByRock)
        {
            thisEnemy.TakeDamage(playerWeaponOne.damage);
            //thisEnemy.TakeDamage(1);
            _isHitByRock = false;
        }

        if (_isHitByWave)
        {
            thisEnemy.TakeDamage(playerWeaponOne.damage);
            //thisEnemy.TakeDamage(1);
            _isHitByWave = false;
        }

        if (_isHitByBolt)
        {
            thisEnemy.TakeDamage(playerWeaponOne.damage);
            _isHitByBolt = false;
        }

        if (_isHitByRoar)
        {
            thisEnemy.TakeDamage(playerWeaponOne.damage);
            _isHitByRoar = false;
        }

        if (_isHitBySong)
        {
            thisEnemy.TakeDamage(playerWeaponOne.damage);
            _isHitBySong = false;
        }

        if (_isHitByFireBurst)
        {
            thisEnemy.TakeDamage(playerWeaponOne.damage);
            _isHitByFireBurst = false;
        }

        if (_isHitByDrain)
        {
            thisEnemy.TakeDamage(playerWeaponOne.damage);
            playerOne.health++;
            _isHitByDrain = false;
        }

        gui.SetEnemyHealth(thisEnemy.health / maxHealth, thisEnemy.health);
    }

    public override void Die()
    {
        //player.AddExperience(expOnDeath);
        thisEnemy.health = 0;
        _isHit = false;
        _isHitBySpit = false;
        _isHitByRock = false;
        _isHitByWave = false;
        _isHitByBolt = false;
        _isHitByRoar = false;
        _isHitBySong = false;
        _isHitByFireBurst = false;
        _isHitByDrain = false;
        EnemyHealthManager();
        gameController.FightOver(true);
        base.Die();
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.name.Contains("FireSpit"))
        {
            _isHitBySpit = true;
        }

        if (col.collider.name.Contains("Rock"))
        {
            _isHitByRock = true;
        }

        if (col.collider.name.Contains("KrakenWave"))
        {
            _isHitByWave = true;
        }

        if (col.collider.name.Contains("ThunderBolt Projectile"))
        {
            _isHitByBolt = true;
        }

        if (col.collider.name.Contains("Roar"))
        {
            _isHitByRoar = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Roar")
        {
            _isHitByRoar = true;
        }

        if (other.tag == "Song")
        {
            _isHitBySong = true;
        }

        if (other.tag == "FireBurst")
        {
            _isHitByFireBurst = true;
        }

        if (other.tag == "Drain")
        {
            _isHitByDrain = true;
        }
    }
}
