using UnityEngine;  
using System.Collections;  

public class Rotate : MonoBehaviour {  

	public Transform t1;    //开始位置  
	public Transform t2;     //结束位置  
	// Update is called once per frame  
	void Update () {  

		//两者中心点  
		Vector3 center = (t1.transform .position + t2.transform.position) * 0.5f;  

		center += new Vector3(1, 0, 1);  

		Vector3 start = t1.transform.position - center;  
		Vector3 end = t2.transform.position - center;  

		//弧形插值  
		transform.position = Vector3.Slerp(start,end,Time.time/10.0f);  
		transform.position += center;  

	}  
}  