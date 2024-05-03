using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ViewCone : MonoBehaviour
{
	private GameObject targetObject;
	public GameObject TargetObject { get => targetObject; set => targetObject = value; }
	private NPC npc;
	private NavMeshAgent agent;

	public float angleToTarget;
	private Vector3 targetDir;

	private void Start()
	{
		npc = transform.GetComponent<NPC>();
		agent = GetComponent<NavMeshAgent>();
	}
	private void Update()
	{
		if (targetObject == null)
		{
			if (npc.GetCurrentState() != NPCstate.Wander)
			{
				npc.SetNPCState(NPCstate.Wander);
			}
			return;
		}


		angleToTarget = AngleBetween(transform.position, targetObject.transform.position);
		if (angleToTarget <= 45)
		{
			RaycastHit hit;
			if (Physics.Raycast(transform.position, targetObject.transform.position - transform.position, out hit))
			{
				if (hit.collider.CompareTag("Player"))
				{
					float distance = targetDir.magnitude;
					if (distance <= agent.stoppingDistance)
					{
						npc.SetNPCState(NPCstate.Shoot);
					}
					else
					{
						npc.SetNPCState(NPCstate.Chase);
					}
				}
				else
				{
					if (npc.GetCurrentState() != NPCstate.Wander)
					{
						npc.SetNPCState(NPCstate.Wander);
					}
				}
			}
		}
		else
		{
			if (npc.GetCurrentState() != NPCstate.Wander)
			{
				npc.SetNPCState(NPCstate.Wander);
			}
		}
	}
	float AngleBetween(Vector3 from, Vector3 to)
	{
		targetDir = to - from;
		float angle = Vector3.Angle(targetDir, transform.forward);
		return angle;
	}
	void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawLine(transform.position, transform.position + transform.forward);

		Gizmos.color = Color.red;
		if (targetObject == null)
			return;
		Gizmos.DrawLine(transform.position, targetObject.transform.position);
	}
}
