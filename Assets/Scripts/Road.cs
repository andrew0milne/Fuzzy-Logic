/*
	Basic script for controlling the racing line
	
	Takes in user input, and also controls the road cylinder
	
	Written by Andrew Milne
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Road : MonoBehaviour 
{

	public float speed;
	float extra_speed = 1.0f;

	Vector2 pos;

	public Toggle control;
	public GameObject UI;
	public GameObject pause_menu;

	public GameObject road;
	public GameObject[] road_objs;

	float angle = 0.0f;

	// Use this for initialization
	void Start () 
	{
		pos = transform.position;
	}

	// Update is called once per frame
	void Update () 
	{
		pos = transform.position;
		pos.y = -0.05f;

		// If the right arrow is pressed, move the object right, until it gets to the edge
		if(Input.GetKey(KeyCode.RightArrow))
		{
			pos.x += speed * Time.deltaTime * extra_speed;
			if (pos.x > 19.0f) 
			{
				pos.x = 19.0f;
			}

		}

		// If the left arrow is pressed, move the object left, until it gets to the edge
		if(Input.GetKey(KeyCode.LeftArrow))
		{
			pos.x -= speed * Time.deltaTime * extra_speed;
			if (pos.x < 3.0f) 
			{
				pos.x = 3.0f;
			}
		}

		// Doubles the speed if Ctrl is pressed
		if (Input.GetKeyDown (KeyCode.LeftControl)) 
		{
			extra_speed = 2.0f;
		}
		else if (Input.GetKeyUp (KeyCode.LeftControl)) 
		{
			extra_speed = 1.0f;
		}
		
		// Updates the objects position
		transform.position = pos;

		// Displays the control menu if the control toggle is on, opposite for the game UI
		UI.SetActive (!control.isOn);
		pause_menu.SetActive (control.isOn);

		// Rotates the cylinder road
		angle -= 50.0f * Time.deltaTime;
		road.transform.eulerAngles = new Vector3 (angle, 0.0f, 90.0f);


	}
}
