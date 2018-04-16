using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetKeyboardController : MonoBehaviour {
    public float speed = 1.0f;
	
	// Update is called once per frame
	void Update () {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        var move = new Vector3(moveHorizontal, 0, moveVertical);
        transform.position += move * Time.deltaTime * speed;
    }
}
