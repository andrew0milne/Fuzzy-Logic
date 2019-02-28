/*
	Defuzzification for a fuzzy inference system, using the centre of gravity method
	
	Written by Andrew Milne
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class Output : MonoBehaviour 
{
	public int rule;

	public Vector2 left_point; 
	public Vector2 mid_point; 
	public Vector2 right_point;

	public float horz_line;

	public GameObject input_1;
	public GameObject input_2;

	Triangle tri;

	public float area_final;
	public float area_cog_final;

	Vector2 pos;

	GameObject car;

	GameObject[] lines;

	GameObject[] output_lines;

	public GameObject line_prefab;

	public float area1, area2, area3;
	public float cog1, cog2, cog3;

	public bool display_output = false;

	Toggle toggle;

	public bool debug = false;



	// Use this for initialization
	void Start () 
	{
		// The output triangle
		tri.Init(left_point, mid_point, right_point);
		
		pos = transform.position;
		
		// Gets the car object
		car = GameObject.FindGameObjectWithTag ("Car");
		
		// Gets the 'graph' toggle
		toggle = GameObject.FindGameObjectWithTag ("Toggle").GetComponent<Toggle> ();

		// Creates the arrays for the output lines
		lines = new GameObject[2];
		output_lines = new GameObject[3];

		// Creates the lines renderers
		for (int i = 0; i < 2; i++) 
		{
			lines [i] = Instantiate(line_prefab, gameObject.transform);
		}
		for (int i = 0; i < 3; i++) 
		{
			output_lines [i] = Instantiate(line_prefab, gameObject.transform);
		}
	}
	
	// Calculates the area of a rectangle, using its top corners. Assumes the bottom corners are on the x-axis
	float RectArea(Vector2 top_right, Vector2 top_left)
	{
		float width, height;

		width = top_right.x - top_left.x;

		height = top_left.y;

		return Mathf.Abs(width * height);
	}

	// Turns the GameObjects in 'go' on or off depending on the boolean tf
	void ToggleLines(GameObject[] go, bool tf)
	{
		foreach(GameObject g in go)
		{
			g.SetActive (tf);
		}
	}

	// Sets the colours of the line renderers to 'colour', in the 'lines' and 'output_lines' arrays
	public void ChangeColour(Color colour)
	{
		foreach (GameObject g in lines) 
		{
			g.GetComponent<Renderer> ().material.color = colour;
		}

		foreach (GameObject g in output_lines) 
		{
			g.GetComponent<Renderer> ().material.color = colour;
		}
	}

	// Returns the GameObject with the lower output value, also sets the colour of said object to yellow and the other to black
	GameObject GetMinOuput(GameObject g1, GameObject g2)
	{
		g1.GetComponent<FuzzyInput> ().ChangeColour (Color.black);
		g2.GetComponent<FuzzyInput> ().ChangeColour (Color.black);

		if (g1.GetComponent<FuzzyInput> ().output < g2.GetComponent<FuzzyInput> ().output) 
		{
			g1.GetComponent<FuzzyInput> ().ChangeColour (Color.yellow);
			return g1;
		}
		else
		{
			g2.GetComponent<FuzzyInput> ().ChangeColour (Color.yellow);
			return g2;
		}
	}

	// Update is called once per frame
	void Update () 
	{
		// Gets the minimum value from the two input rules
		GameObject min = GetMinOuput (input_1, input_2);
		horz_line = min.GetComponent<FuzzyInput> ().output;

		// Calculates where this horizontal line intersects with the rule triangle
		Vector2 left_intersect = new Vector2 (tri.line_left.x(horz_line), horz_line);
		Vector2 right_intersect = new Vector2 (tri.line_right.x(horz_line), horz_line);

		// Using the tqo points of intersection, calculated above, and the two bottom points of the traingle
		// The area of the resting trapezium is calculated
		/*   _ _ _ _ 
		   /|      |\
		  / |      | \
		 /  |      |  \			
		/_ _|_ _ _ |_ _\	
		*/
		// Area of the rectangle
		area1 = RectArea (right_intersect, left_intersect);
		// Area of the two triangles
		area2 = RectArea (left_intersect, new Vector2 (left_point.x, horz_line)) / 2.0f;
		area3 = RectArea (right_intersect, new Vector2 (right_point.x, horz_line)) / 2.0f;

		// Calcultes the centre of gravity (COG) of the three shapes
		cog1 = (left_intersect.x + right_intersect.x) / 2.0f;
		cog2 = left_point.x + ((2.0f / 3.0f) * (left_intersect.x - left_point.x));
		cog3 = right_point.x + ((2.0f / 3.0f) * (right_intersect.x - right_point.x));

		// calculates the area * cog of each shape and then sums them
		area_cog_final = (area1 * cog1) + (area2 * cog2) + (area3 * cog3);
		// The total area of the trapezium
		area_final = area1 + area2 + area3;

		// Turns the lines on/off depending on the graph toggle in the UI
		ToggleLines (lines, toggle.isOn);
		ToggleLines (output_lines, toggle.isOn);

		// draws the FIS graphs
		if (toggle.isOn) 
		{
			// The triangle
			lines [0].GetComponent<Lines> ().SetPosition (left_point + pos, mid_point + pos);
			lines [1].GetComponent<Lines> ().SetPosition (mid_point + pos, right_point + pos);

			Vector2 new_pos = pos;
			new_pos.y = -11.0f;
	
			ToggleLines (output_lines, display_output);

			// Output polygon
			if (display_output) {

				output_lines [0].GetComponent<Lines> ().SetPosition (left_intersect + new_pos, left_point + new_pos);
				output_lines [1].GetComponent<Lines> ().SetPosition (right_intersect + new_pos, left_intersect + new_pos);
				output_lines [2].GetComponent<Lines> ().SetPosition (right_intersect + new_pos, right_point + new_pos);
			} 
		} 
	}
}
