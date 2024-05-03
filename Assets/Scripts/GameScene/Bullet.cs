using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPun, IPunObservable
{
    private int entityID;
    private float damage;
	public Team Team { get => entityID < 2 ? Team.A : Team.B; }
    void Start()
    {
    }

	public void SetPlayer(int _entityID)
    {
        entityID = _entityID;
        //punRPC comunicate onwer ID
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out EntityBase _entity))
        {
            if (_entity.Team != Team)
            {

                _entity.DealDamage(entityID,20);
            }
        }

        if(photonView.IsMine)
			PhotonNetwork.Destroy(this.gameObject);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
}
