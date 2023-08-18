using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowCode : MonoBehaviour
{
    private Vector3 fixedLocalPos;

    void Start()
    {
        fixedLocalPos = transform.position - transform.parent.position;
    }

    void Update()
    {
        transform.position = (transform.parent.position) + fixedLocalPos;
    }
}
