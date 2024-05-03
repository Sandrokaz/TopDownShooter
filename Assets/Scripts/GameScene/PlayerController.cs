using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : EntityBase
{
    [SerializeField] private Camera playerCam;
    [SerializeField] private GameObject playerObj;
    
    public Vector3 worldPosition;
    public Vector3 pointPos;

    private bool shootReady = true;
    private Rigidbody myRigidbody;
    void Start()
    {
        if (!photonView.IsMine)
        {
            playerCam.gameObject.SetActive(false);
        }
        else
        {
            GameUI_Manager.Instance.MainCamera.SetActive(false);
        }
        myRigidbody = GetComponent<Rigidbody>();

    }
    private void Update()
    {
        RegenerateHealth();

        pointPos = gunPoint.transform.position;
        if(photonView.IsMine)
        {
            UpdateLocalPlayer();
        }

        //if(destroy)
        //{
        //    GameUI_Manager.Instance.GameManager.EntityDead(ID);
        //
        //    GameUI_Manager.Instance.MainCamera.SetActive(true);
        //
        //    PhotonNetwork.Destroy(this.gameObject);
        //}
    }

    private void UpdateLocalPlayer()
    {
        float xMove = Input.GetAxisRaw("Horizontal");
        float zMove = Input.GetAxisRaw("Vertical");
        Vector3 moveDir = new Vector3(xMove, 0, zMove).normalized;
        myRigidbody.velocity = moveDir *  moveForce;


        Vector3 mousePos = Input.mousePosition;
        mousePos.z = playerCam.nearClipPlane;
        worldPosition = playerCam.ScreenToWorldPoint(mousePos);
        RotateToward(worldPosition, 4);
        Debug.DrawLine(transform.position, worldPosition, Color.red);

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Shoot();
        }
    }

    public void RotateToward(Vector3 targ, float speed)
    {
        targ.y = 0f;
        Vector3 objectPos = transform.position;
        targ.x = targ.x - objectPos.x;
        targ.z = targ.z - objectPos.z;
        float angle = Mathf.Atan2(targ.z, targ.x) * Mathf.Rad2Deg - 90;
        playerObj.transform.rotation = Quaternion.Euler(new Vector3(90,-angle, 0));
    }
    private void Shoot()
    {
        if (!shootReady)
            return;

		photonView.RPC(nameof(RPC_PlayShootSound), RpcTarget.All);

		GameObject _bullet = PhotonNetwork.Instantiate("Bullet", gunPoint.transform.position, Quaternion.identity);
        float xDir = worldPosition.x - gunPoint.transform.position.x;
        float zDir = worldPosition.z - gunPoint.transform.position.z;
        
        _bullet.GetComponent<Rigidbody>().velocity = (new Vector3(xDir,0,zDir)).normalized * shootSpeed;
        _bullet.GetComponent<Bullet>().SetPlayer(ID);
        StartCoroutine(YieldShoot(0.5f));
    }
    private IEnumerator YieldShoot(float _time)
    {
        shootReady = false;
        yield return new WaitForSeconds(_time);
        shootReady = true;

    }

	[PunRPC]
	public void RPC_PlayShootSound()
	{
		GameAudioManager.Instance.PlayShootSound();
	}
}
