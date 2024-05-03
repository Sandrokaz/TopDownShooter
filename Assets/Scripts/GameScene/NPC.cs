using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;

public class NPC : EntityBase
{
	private NavMeshAgent agent;
	private Vector3 targetPos;
	public bool destroy;

	private void Start()
	{
		viewCone = GetComponent<ViewCone>();
		agent = GetComponent<NavMeshAgent>();
		npcState = NPCstate.Wander;
		gameManager = GameUI_Manager.Instance.GameManager;
		SetNewTargetPosition();
	}
	
	private void Update()
	{
		RegenerateHealth();
		CheckForEntitiesInRange();

		switch (npcState)
		{
			case NPCstate.Wander:
				Wander();
				break;

			case NPCstate.Shoot:
				if (targetEntity == null)
					return;
				RotateToward(targetEntity.transform.position);
				Shoot();
				break;

			case NPCstate.Chase:
				if (targetEntity == null)
					return;
				RotateToward(targetEntity.transform.position);
				Chase();
				break;
		}
	}

	private void Wander()
	{
		agent.stoppingDistance = 3;
		float distance = (targetPos - transform.position).magnitude;
		if ((distance - 0.15f) <= agent.stoppingDistance + agent.radius || targetPos == null)
		{
			SetNewTargetPosition();
		}

		agent.SetDestination(targetPos);
	}

	private void Chase()
	{
		agent.stoppingDistance = 20;
		agent.SetDestination(targetEntity.transform.position);
	}

	private void Shoot()
	{
		StartCoroutine(NPCshoot(1));
	}

	private IEnumerator NPCshoot(float _time)
	{
		yield return new WaitForSeconds(_time);
		photonView.RPC(nameof(RPC_PlayShootSound), RpcTarget.All);

		GameObject _bullet = PhotonNetwork.Instantiate("Bullet", gunPoint.transform.position, Quaternion.identity);

		_bullet.GetComponent<Rigidbody>().velocity = transform.forward * shootSpeed;
		_bullet.GetComponent<Bullet>().SetPlayer(ID);
		
		StopAllCoroutines();
	}

	[PunRPC]
	public void RPC_PlayShootSound()
	{
		GameAudioManager.Instance.PlayShootSound();
	}

	private void SetNewTargetPosition()
	{
		CustomRoom room = CustomRoom.Instance;
		float rndX = Random.Range(room.transform.position.x - room.HalfXScale, room.transform.position.x + room.HalfXScale);
		float rndZ = Random.Range(room.transform.position.z - room.HalfZScale, room.transform.position.z + room.HalfZScale);

		targetPos = new Vector3(rndX, 0, rndZ);
	}

	private void CheckForEntitiesInRange()
	{
		for (int i = 0; i < gameManager.activeEntities.Count; i++)
		{
			if(gameManager.activeEntities[i] != null)
            {
				if ((gameManager.activeEntities[i].transform.position - transform.position).magnitude < 50)
				{
					if (gameManager.activeEntities[i].Team != Team)
					{
						targetEntity = gameManager.activeEntities[i];
						viewCone.TargetObject = targetEntity.gameObject;
						return;
					}
				}
            }
		}

		viewCone.TargetObject = null;
		targetEntity = null;
	}

	public void SetNPCState(NPCstate _state)
	{
		if (_state != npcState)
		{
			photonView.RPC(nameof(RPC_SetNPCState), RpcTarget.All, _state);
		}
	}

	[PunRPC]
	public void RPC_SetNPCState(NPCstate _state)
	{
		npcState = _state;
	}

	public NPCstate GetCurrentState()
	{
		return npcState;
	}

	public void RotateToward(Vector3 targ)
	{
		targ.y = 0f;
		Vector3 objectPos = transform.position;
		targ.x = targ.x - objectPos.x;
		targ.z = targ.z - objectPos.z;
		float angle = Mathf.Atan2(targ.z, targ.x) * Mathf.Rad2Deg - 90;
		transform.rotation = Quaternion.Euler(new Vector3(0, -angle, 0));
	}

}