/*
	Basic script to stop an objects rotating, if its parent is a rotating object
	
	Written by Andrew Milne
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopRotate : MonoBehaviour 
{

	Vector3 start_rotation;

	// Use this for initialization
	void Start () 
	{
		// Saves the objects starting rotation
		start_rotation = transform.eulerAngles;
	}
	
	// Update is called once per frame
	void Update () 
	{
		// Sets the objects rotation the starting rotation
		transform.eulerAngles = start_rotation;
	}
}
