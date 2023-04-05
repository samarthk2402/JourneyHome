using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
    private IEnumerator WaitToDestroy(){
        yield return new WaitForSeconds(3);
        Destroy(this.gameObject);
    }

    void Update(){
        StartCoroutine(WaitToDestroy());
    }
}
