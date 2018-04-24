using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetKeyboardController : MonoBehaviour {
    public float speed = 1.0f;
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.W)|| Input.GetKey(KeyCode.UpArrow)) {
			transform.position += -transform.forward * Time.deltaTime * 10;
		}
		if (Input.GetKey(KeyCode.S)||Input.GetKey(KeyCode.DownArrow))
		{
			transform.position += transform.forward * Time.deltaTime * 10;
		}
		if (Input.GetKey(KeyCode.A)|| Input.GetKey(KeyCode.LeftArrow))
		{
			transform.Rotate(-transform.up*Time.deltaTime*45);
		}
		if (Input.GetKey(KeyCode.D)|| Input.GetKey(KeyCode.RightArrow))
		{
			transform.Rotate(transform.up * Time.deltaTime * 45);
		}
    }
}
