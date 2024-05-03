using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class EntityBase : MonoBehaviourPun, IPunObservable
{
    protected GameManager gameManager;
    protected string plName;
    public string EntityName { get => plName; set => SetName(value); }
  
    protected int id;
    public int ID { get => id; }

	[SerializeField] protected SpriteRenderer spriteRenderer;
	[SerializeField] protected GameObject gunPoint;
	[SerializeField] protected float shootSpeed;

    protected float health;
    protected float maxHealth = 100f;
    [SerializeField] private HealthBar healthBar;

    public float Health { get => health; }

    public Team Team { get => id < 2? Team.A : Team.B; }


    [SerializeField] private protected float moveForce = 3;
    [SerializeField] protected ParticleSystem hitParticles;

    protected ViewCone viewCone;
    protected EntityBase targetEntity;
    protected NPCstate npcState;


    private void Awake()
	{
        gameManager = GameUI_Manager.Instance.GameManager;
		gameManager.activeEntities.Add(this);
        GameUI_Manager.Instance.SetGameState(GameState.Running);
        health = maxHealth;
	}

	private void OnDestroy()
	{

        gameManager.activeEntities[id] = null;
	}

	protected void SetName(string value)
    {
        plName = value;
    }

    public void SetID(int _id)
    {
        if (_id < 2)
        {
            photonView.RPC(nameof(RPC_SetPlayerID), RpcTarget.All, _id);
        }
        else
        {
            photonView.RPC(nameof(RPC_SetPlayerID), RpcTarget.All, _id);
        }
    }
    [PunRPC]
    public void RPC_SetPlayerID(int _id)
    {
        id = _id;

        if(Team == Team.A)
        {

            spriteRenderer.color = Color.blue;
        }
        else
        {
            spriteRenderer.color = Color.red;
        }
    }

    public void DealDamage(int _otherId, float _damage)
    {
        photonView.RPC("RPC_DealDamage", RpcTarget.All,_otherId, _damage);
        photonView.RPC(nameof(RPC_EntityHitParticles), RpcTarget.All);

        if (healthBar != null)
        {
            
            SetPlayerHealth(health);
        } 
    }

    [PunRPC]
    public void RPC_DealDamage(int _otherId,float damage)
    {
        health -= damage;
       
        if (health <= 0 && photonView.IsMine)
        {
            gameManager.EntityDead(ID);

            gameManager.SetKillCount(_otherId);

            PhotonNetwork.Destroy(this.gameObject);
        }
        if (EntityName == "NPC")
        {
            for(int i = 0; i < gameManager.activeEntities.Count; i++)
            {
                if(gameManager.activeEntities[i] != null)
                {
                    if(gameManager.activeEntities[i].ID == _otherId)
                    {
                        targetEntity = gameManager.activeEntities[i];
                        viewCone.TargetObject = gameManager.activeEntities[i].gameObject;
                        npcState = NPCstate.Chase;
                    }
                }

            }
        }
    }

    public void SetPlayerHealth(float health)
    {
        photonView.RPC("RPC_SetPlayerHealth", RpcTarget.All, health);
    }

    [PunRPC]
    public void RPC_SetPlayerHealth(float health)
    {

        healthBar.SetHealth(health);

    }

    [PunRPC]
    public void RPC_EntityHitParticles()
    {
        hitParticles.Play();
    }
    protected void RegenerateHealth()
    {
        if (health < maxHealth -1 && health > 1)
        {
            health += 5 * Time.deltaTime;

            if (healthBar == null)
                return;
            SetPlayerHealth(health);
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
}
