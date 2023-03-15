using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeAttack : MonoBehaviour
{
    [SerializeField] GameObject slimeBall;
    [SerializeField] Transform handPos;
    [SerializeField] float throwForce = 5f;

    public void ThrowSlime(Vector3 playerPos){
        var sb = Instantiate(slimeBall, handPos.position, Quaternion.identity);
        sb.GetComponent<Rigidbody>().AddForce((playerPos+new Vector3(-0.5f, 5, 0)-transform.position)*throwForce, ForceMode.Impulse);
    }
}
