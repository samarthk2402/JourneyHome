using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class SlimeAttack : MonoBehaviour
{
    [SerializeField] GameObject slimeBall;
    [SerializeField] Transform lefthandPos;
    [SerializeField] Transform righthandPos;
    [SerializeField] float throwForce = 5f;
    [SerializeField] float throwTime = 10000;
    [SerializeField] MultiAimConstraint leftArm;
    [SerializeField] MultiAimConstraint rightArm;

    private bool hasWaited = true;


    void Start(){
        leftArm.weight = 0;
        rightArm.weight = 0;
    }

    public void ThrowSlime(Vector3 playerPos){

        // var hasThrown = false;

        // for(float i=0; i<1 ; i+=0.02f){
        //      leftArm.weight = i;
        //      Debug.Log(leftArm.weight);
        // }
        // while(!hasThrown){
        //     if(hasWaited){
        //         leftArm.weight += 0.1f;
        //         StartCoroutine(Wait());
        //         Debug.Log(leftArm.weight);
        //     }
        

        //     if (leftArm.weight>=0.5f){
        //         hasThrown = true;
        //     }
        // }

        StartCoroutine(IncreaseWeight());

        if(hasWaited){
            var sb = Instantiate(slimeBall, lefthandPos.position, Quaternion.identity);
            sb.GetComponent<Rigidbody>().AddForce((playerPos+new Vector3(-0.5f, 5, 0)-transform.position)*throwForce, ForceMode.Impulse);
            StartCoroutine(DecreaseWeight());
        }
        //Debug.Log(leftArm.weight);

        // while(leftArm.weight>0){
        //     elapsedTime += Time.deltaTime;
        //     float pcComplete = elapsedTime / throwTime;
        //     leftArm.weight = Mathf.Lerp(1, 0, pcComplete);
        //     Debug.Log(leftArm.weight);
        // }
        
        // for(float i=1; i>0 ; i-=0.02f){
        //     leftArm.weight = i;
        // }


    }

    private IEnumerator IncreaseWeight(){
        hasWaited = false;
        while(leftArm.weight<1){

            leftArm.weight += 0.2f;
            Debug.Log(leftArm.weight);
            yield return new WaitForSeconds(0.1f);

        }
        hasWaited = true;
        yield return null;
    } 

    private IEnumerator DecreaseWeight(){
        while(leftArm.weight>0){

            leftArm.weight -= 0.2f;
            Debug.Log(leftArm.weight);
            yield return new WaitForSeconds(0.1f);

        }
        yield return null;
    } 
}
