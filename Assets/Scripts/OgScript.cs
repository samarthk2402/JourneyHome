using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OgScript : MonoBehaviour
{
    public GameObject tongue;

    Vector3 tonguePosition;
    float tongueSize = 0;
    float desiredTongueSize;
    public float throwSpeed = 5;
    public float cooldown = 10;
    public float timer;
    private Vector3 initTongueSize;

    public enum State{
        Normal,
        ThrowingTongue,
        PullingTongue
    }

    public State state;

    // Start is called before the first frame update
    void Start()
    {
        initTongueSize = tongue.transform.localScale;
    }

    void Update(){
        switch(state){
        default:
        case State.Normal:
            if(timer > 0){
                GetComponent<EnemyAI>().enabled = true;
                timer -= Time.deltaTime;
            }

            break;
        case State.ThrowingTongue:
            ThrowTongue();
            break;
        case State.PullingTongue:
            BringTongueBack();
            break;
        }
    }

    public void StartTongue(Vector3 playerpos){
        GetComponent<EnemyAI>().enabled = false;
        var lookPos = tonguePosition;
        lookPos.y = transform.position.y;
        transform.LookAt(lookPos);
        //transform.LookAt(tonguePosition);
        tongue.transform.localScale = Vector3.zero;
        tongue.SetActive(true);
        //player.state = Player.State.HookshotThrown;
        tongueSize = 0f; 
        tonguePosition = playerpos;
        desiredTongueSize = Vector3.Distance(transform.position, tonguePosition);

        if(timer <= 0){
            state = State.ThrowingTongue;
        }
    }

    public void ThrowTongue(){
        //transform.LookAt(tonguePosition);
        tongue.transform.LookAt(tonguePosition);
        //StartCoroutine(IncreaseSize());
        tongueSize += throwSpeed * Time.deltaTime;
        tongue.transform.localScale = new Vector3(initTongueSize.x, initTongueSize.y, tongueSize);

        if(tongueSize > desiredTongueSize)
        {
            tongue.transform.localScale = new Vector3(initTongueSize.x, initTongueSize.y, desiredTongueSize);
            state = State.PullingTongue;
        }
    }

    public void BringTongueBack(){
        //transform.LookAt(tonguePosition);
        tongue.transform.LookAt(tonguePosition);
        //StartCoroutine(IncreaseSize());
        tongueSize -= throwSpeed * Time.deltaTime;
        tongue.transform.localScale = new Vector3(initTongueSize.x, initTongueSize.y, tongueSize);

        if(tongueSize <= 0)
        {
            StopTongue();
        }
    }

    public void StopTongue(){
        GetComponent<EnemyAI>().enabled = true;
        Debug.Log("Stopping grapple");
        tongue.transform.localScale = Vector3.zero;
        timer = cooldown;
        state = State.Normal;
        tongue.SetActive(false);
    }





}
