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
    public GameObject tongueCol;
    public Player player;
    public ParticleSystem ps;

    public enum State{
        Normal,
        ThrowingTongue,
        PullingTongue
    }

    public State state;

    void Awake(){
        player = GameObject.Find("Player").GetComponent<Player>();
    }

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
            //player.state = Player.State.Normal;

            break;
        case State.ThrowingTongue:
            ThrowTongue();
            break;
        case State.PullingTongue:
            BringTongueBack();
            break;
        }


        //tongueCol.transform.position = new Vector3(tongue.transform.position.x, tongue.transform.position.y, tongue.transform.position.z + tongue.transform.localScale.z/2);
        //tongueCol.transform.position = new Vector3(tongue.transform.position.x, tongue.transform.position.y, tongue.transform.position.z);
        tongueCol.transform.Rotate(tongue.transform.localRotation.x, tongue.transform.localRotation.y, tongue.transform.localRotation.z, Space.Self);
        tongueCol.transform.localPosition = tongue.transform.localPosition + new Vector3(0, 0, tongue.transform.localScale.z);
        //Debug.Log(player.state);
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

        if(tongueSize <= 4){
            player.velocity = Vector3.zero;
            player.state = Player.State.Normal;
        }

        if(tongueSize <= 0)
        {
            StopTongue();
        }
    }

    public void StopTongue(){
        GetComponent<EnemyAI>().enabled = true;
        tongue.transform.localScale = Vector3.zero;
        timer = cooldown;
        state = State.Normal;
        tongue.SetActive(false);

    }

    public void ToxicGas(){
        GameObject player = GameObject.Find("Player");
        var lookPos = player.transform.position;
        lookPos.y = transform.position.y;
        transform.LookAt(lookPos);
        ps.Play();
        StartCoroutine(player.GetComponent<PlayerTarget>().TakeDamageOverTime(10, 5));

    }
    





}
