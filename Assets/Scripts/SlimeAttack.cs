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
    [SerializeField] MultiAimConstraint leftArm;
    [SerializeField] MultiAimConstraint rightArm;

    private bool hasWaited = true;
    //private bool canStart = true;

    void Awake(){
        var player = GameObject.Find("Player");
        var animator = GetComponentInChildren<Animator>();
        var rigBuilder = GetComponentInChildren<RigBuilder>();
        
        animator.enabled = false;

        leftArm.weight = 0;
        rightArm.weight = 0;
        Debug.Log(player.transform);
        WeightedTransformArray sources = new WeightedTransformArray();
        sources.Add(new WeightedTransform(player.transform, leftArm.weight));
        leftArm.data.sourceObjects = sources;
        rightArm.data.sourceObjects = sources;

        rigBuilder.Build();
        animator.enabled = true;
    }


    void Update(){
        var player = GameObject.Find("Player");
        Debug.Log(player.transform);
        WeightedTransformArray sources = new WeightedTransformArray();
        sources.Add(new WeightedTransform(player.transform, leftArm.weight));
        leftArm.data.sourceObjects = sources;
        rightArm.data.sourceObjects = sources;
    }

    public void ThrowSlime(Vector3 playerPos){

        // if(canStart){
        //     Throw(leftArm, playerPos, lefthandPos, -0.5f);
        //     canStart = false;
        // }

        // if(canStart){
        //     Throw(rightArm, playerPos, righthandPos, 0.5f);
        //     canStart = false;
        // }

        Throw(rightArm, playerPos, righthandPos, 0.5f);
        Throw(leftArm, playerPos, lefthandPos, -0.5f);

    }

    void Throw(MultiAimConstraint arm, Vector3 playerPos, Transform armPos, float offset){
        StopCoroutine(DecreaseWeight(arm));
        if(arm.weight<=0){
            StartCoroutine(IncreaseWeight(arm));
        }

        if(hasWaited && arm.weight>=1){
            var sb = Instantiate(slimeBall, armPos.position, Quaternion.identity);
            sb.GetComponent<Rigidbody>().AddForce((playerPos+new Vector3(offset, 5, 0)-transform.position)*throwForce, ForceMode.Impulse);
            StopCoroutine(IncreaseWeight(arm));
            StartCoroutine(DecreaseWeight(arm));
            //canStart = true;
        }
    }

    private IEnumerator IncreaseWeight(MultiAimConstraint arm){
        hasWaited = false;
        while(arm.weight<1){
            yield return new WaitForSeconds(0.05f);
            arm.weight += 0.2f;
            //Debug.Log(leftArm.weight);

        }
        hasWaited = true;
        yield break;
        // yield return null;
    } 

    private IEnumerator DecreaseWeight(MultiAimConstraint arm){
        while(arm.weight>0){
            arm.weight -= 0.2f;
            //Debug.Log(leftArm.weight);
            yield return new WaitForSeconds(0.05f);

        }

        yield break;
        // yield return null;
    } 
}
