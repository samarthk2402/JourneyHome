using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TongueColScript : MonoBehaviour
{
    public Transform frogPos;
    public Player player;
    public OgScript tongueScript;

    void Awake(){
        player = GameObject.Find("Player").GetComponent<Player>();
        tongueScript = GetComponentInParent<OgScript>();
    }

    void OnCollisionEnter(Collision col){
        Vector3 tongueDir = (frogPos.position - col.transform.position).normalized;
        if(col.gameObject.name == "Player"){
            player.state = Player.State.Stunned;
            player.velocity = tongueDir * tongueScript.throwSpeed;
            //player.controller.Move(tongueDir * throwSpeed * Time.deltaTime);
            Debug.Log(player.state);
        }
        //Debug.Log(col.gameObject.name);
    }
}
