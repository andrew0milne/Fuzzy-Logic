/*
	Basic script to set the positions in a LineRenderer with two points
	
	Written by Andrew Milne
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(LineRenderer))]
public class Lines : MonoBehaviour 
{

	LineRenderer line;

	float z_pos = 0.0f;

	// Use this for initialization
	void Start () 
	{
		// Gets the attached LineRenderer
		line = GetComponent<LineRenderer> ();
	}
	
	// Sets the two positions of the line renderer to 'pos_1' and 'pos_2'
	// Assumes the LineRenderer has only two points
	public void SetPosition(Vector2 pos_1, Vector2 pos_2)
	{
		// Sets the coloured LineRenderer in front so its easier to see
		if (GetComponent<Renderer> ().material.color == Color.black) 
		{
			z_pos = -4.0f;
		} 
		else 
		{
			z_pos = -5.0f;
		}
		line.SetPosition (0, new Vector3 (pos_1.x, pos_1.y, z_pos));
		line.SetPosition (1, new Vector3 (pos_2.x, pos_2.y, z_pos));
	}
}
