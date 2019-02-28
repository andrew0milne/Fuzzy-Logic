/*
	Controls the input variable for a fuzzy inference system
	
	Also contains the Structs for lines and triangles
	
	Written by Andrew Milne
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Take in three points, and stores these as Lines ( A->B & B->C)
struct Triangle
{
	public Line line_left;
	public Line line_right;

	// Initialises the lines
	public void Init(Vector2 left_point, Vector2 mid_point, Vector2 right_point)
	{
		line_left.Init (left_point, mid_point);
		line_right.Init (mid_point, right_point);
	}
};

// Contains the equation of the line between point_1 and point_2
struct Line
{
	Vector2 point_1;
	Vector2 point_2;

	float m;
	float c;

	bool vertical;

	// Initialises the line
	public void Init(Vector2 p1, Vector2 p2)
	{
		vertical = false;
		point_1 = p1;
		point_2 = p2;

		// If the line is vertical is gradient is undefined, which isn't good for maths
		if (p2.x == p1.x) 
		{
			vertical = true;
		}
		// Calculates the equation of the line, in slope-intercept form
		// m being the gradient
		// c being the y intercept
		else 
		{
			m = (p2.y - p1.y) / (p2.x - p1.x);
			c = p1.y - m * p1.x;
		}
	}

	// Returns the y coordinate for a given x coordinate
	public float y(float x)
	{
		return m * x + c;
	}

	// Returns the x coordinate for a given y coordinate
	// If the line is horizontal, returns the x of point_1
	public float x(float y)
	{
		if (vertical) 
		{
			return point_1.x;
		} 
		else 
		{
			return (y - c) / m;
		}
	}

};

public class FuzzyInput : MonoBehaviour 
{
	public GameObject car;

	public Vector2 left_point; 
	public Vector2 mid_point; 
	public Vector2 right_point;

	Triangle tri;

	public float vert_line;
	float vert_line_transform;

	public float output;
	Vector2 pos;

	GameObject[] lines;
	public GameObject line_prefab;

	Toggle toggle;

	// Use this for initialization
	void Start () 
	{
		car = GameObject.FindGameObjectWithTag ("Car");
		toggle = GameObject.FindGameObjectWithTag ("Toggle").GetComponent<Toggle> ();

		lines = new GameObject[4];

		for (int i = 0; i < 4; i++) 
		{
			lines [i] = Instantiate(line_prefab, gameObject.transform);
		}

		tri.Init (left_point, mid_point, right_point);
		pos = new Vector2 (transform.position.x, transform.position.y);


	}

	void ToggleLines(GameObject[] go, bool tf)
	{
		foreach(GameObject g in go)
		{
			g.SetActive (tf);
		}
	}

	public void ChangeColour(Color colour)
	{
		foreach (GameObject g in lines) {
			g.GetComponent<Renderer> ().material.color = colour;
		}
	}

	// Update is called once per frame
	void Update () 
	{
		if (tag == "dInput") 
		{
			vert_line = car.GetComponent<Car> ().distance;
		}
		else if (tag == "vInput") 
		{
			vert_line = car.GetComponent<Car> ().velocity;
		}

		vert_line_transform = vert_line + pos.x;

		if (vert_line > mid_point.x) 
		{
			output = tri.line_right.y (vert_line);
		} 
		else if (vert_line < mid_point.x) 
		{
			output = tri.line_left.y (vert_line);
		}
		else 
		{
			output = mid_point.y;
		}

		ToggleLines (lines, toggle.isOn);

		if(toggle.isOn)
		{
			// The triangle
			lines[0].GetComponent<Lines>().SetPosition(left_point + pos, mid_point + pos);
			lines[1].GetComponent<Lines>().SetPosition(mid_point + pos, right_point + pos);

			// Vertical line
			lines[2].GetComponent<Lines>().SetPosition(new Vector2(vert_line_transform, pos.y + output), new Vector2(vert_line_transform, pos.y + 1.5f));

			//Horizontal line
			lines[3].GetComponent<Lines>().SetPosition(new Vector2(vert_line + pos.x, output + pos.y), new Vector2(10.0f, output + pos.y));
		}
	}
}
