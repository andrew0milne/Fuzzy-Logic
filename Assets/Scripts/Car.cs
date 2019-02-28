/*
	Controls the car for a fuzzy inference system application
	
	Does the final step in the defuzzification
	
	Takes in the various values from the game's UI
	
	Written by Andrew Milne
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Car : MonoBehaviour 
{	
	public float distance;

	public float velocity;

	float total_area_cog;
	float total_area;

	public float acceleration;

	public GameObject[] outputs;
	public GameObject[] actual_outputs;

	public GameObject[] max_outputs;

	public float speed;
	public float max_dist;

	Vector2 start_pos;
	Vector2 prev_pos;

	public GameObject road;

	public Slider sSlider;
	public Slider disSlider;

	public InputField vInput;
	public InputField dInput;

	public Text acc_output;
	public Text s_output;
	public Text dis_output;

	public GameObject line_prefab;
	public GameObject graphs;
	GameObject output_line;

	Toggle paused;
	Toggle graphs_on;

	Vector2[] output_pos;

	// Use this for initialization
	void Start () 
	{
		// Initialises the car
		distance = Mathf.Clamp ((transform.position.x - road.transform.position.x) / max_dist, -1, 1);
		velocity = 0.0f;
		acceleration = 0.0f;
		
		// Reads in the starting values from the UI
		sSlider.value = speed;
		disSlider.value = max_dist;

		// Initialises the crisp output for the FIS graph
		output_line = Instantiate (line_prefab, graphs.transform);
		
		// Creates the array for the graphical output
		output_pos = new Vector2[2];
		output_pos [0] = new Vector2 (0.0f, 0.0f);
		output_pos [1] = new Vector2 (0.0f, -1.0f);

		// Find the two UI toggles
		paused = GameObject.FindGameObjectWithTag("Pause").GetComponent<Toggle>();
		graphs_on = GameObject.FindGameObjectWithTag("Toggle").GetComponent<Toggle>();

		start_pos = transform.position;
		prev_pos = start_pos;

		actual_outputs = new GameObject[outputs.Length];

		max_outputs = new GameObject[5];

		Transform[] temp;

		// Find the correct output objects which are in the children gameobjects
		for(int i = 0; i < outputs.Length; i++)
		{
			temp = outputs [i].GetComponentsInChildren<Transform> ();
			foreach (Transform t in temp) 
			{
				if (t.tag == "Output") 
				{
					actual_outputs [i] = t.gameObject;
				} 

			}
		}
	}
	
	// Final step of the defuzzification
	// Finds the maximum values for the output rules
	// Then works out the area/centre of gravity for the resulting trapezoids
	// Sums these and then calculates the output acceleration
	float CalcuateAcc()
	{
		// Find the maximum value for each of the outputted rules
		foreach (GameObject go in actual_outputs) 
		{
			Output op = go.GetComponent<Output> ();

			// Makes all rules black
			go.GetComponent<Output> ().display_output = false;
			go.GetComponent<Output> ().ChangeColour (Color.black);

			if (max_outputs [op.rule] != null) 
			{
				// If the current rules output is greater than the previous output for that rule
				if (op.horz_line > max_outputs [op.rule].GetComponent<Output> ().horz_line) 
				{
					// Updates the max_output
					max_outputs [op.rule] = go;
				}
			} 
			else 
			{
				max_outputs [op.rule] = go;
			}
		}

		// Reset the total_area and total_area_cog
		total_area = 0.0f;
		total_area_cog = 0.0f;

		// Sums the maximum area and cog*area for each rule, and their colour to cyan
		for (int i = 0; i < 5; i++) 
		{
			total_area += max_outputs [i].GetComponent<Output> ().area_final;
			total_area_cog += max_outputs [i].GetComponent<Output> ().area_cog_final;

			max_outputs [i].GetComponent<Output> ().display_output = true;
			max_outputs [i].GetComponent<Output> ().ChangeColour (Color.cyan);
		}

		// Calculates the output acceleration
		float acc = total_area_cog / total_area;

		// Draws the final output line on the outpur graph
		if (output_line.GetComponent<Lines> () != null) 
		{
			if (graphs_on.isOn) 
			{
				output_line.GetComponent<Lines> ().SetPosition (new Vector2 (7.66f + acc, -10f), new Vector2 (7.66f + acc, -11.0f));
			} 
			else 
			{
				output_line.GetComponent<Lines> ().SetPosition (new Vector2 (0.0f, 0.0f), new Vector2 (0.0f, 0.0f));
			}
		}

		return acc;
	}

	// Updates the UI with the various display values
	void UpdateUI(bool paused)
	{
		speed = sSlider.value;
		max_dist = disSlider.value;

		acc_output.text = acceleration.ToString("F3");

		s_output.text = speed.ToString ("F2");
		dis_output.text = max_dist.ToString ("F2");
		
		// If the game is not paused then display the car's current velocity and distance
		if (!paused) 
		{
			vInput.text = velocity.ToString ("F3");
			dInput.text = distance.ToString ("F3");
		}
	}

	// Converts the text in the input field to a float and clamps it between -1 and 1
	float GetInputNum(InputField in_f)
	{
		float num = 0.0f;

		if (in_f.text.Length > 0) 
		{
			num = float.Parse (in_f.text);
			num = Mathf.Clamp (num, -1.0f, 1.0f);
		}
	
		return num;
	}

	// Update is called last, once per frame
	void LateUpdate () 
	{
		// Get the acceleration from the FIS
		acceleration = CalcuateAcc ();
		
		UpdateUI (paused.isOn);

		// If the game isn't paused
		if (!paused.isOn) 
		{
			// Calculates the distance between the car and the racing line, the scales it so its between -1 and 1
			distance = Mathf.Clamp ((transform.position.x - road.transform.position.x) / max_dist, -1, 1);
			
			// Adds the acceleration to the car's current velocity
			velocity += acceleration * speed;

			Vector2 current_pos = Vector2.zero;
			current_pos.x = velocity;
			
			// Zeros the car's rotation before moving it so it doesn't affect its trajectory
			// Then rotates the car to the correct angle, based on the car's velocity
			transform.eulerAngles = new Vector3 (0.0f, 0.0f, 0.0f);
			transform.Translate (current_pos);
			transform.eulerAngles = new Vector3 (0.0f, 0.0f, -velocity * 30.0f);


		} 
		// If the game is paused, sets the distance and velocity to the values that the user inputs
		else 
		{
			Vector2 dist = road.transform.position;
			dist.y -= 1.5f;
			dist.x += GetInputNum (dInput);
			transform.position = dist;

			distance = GetInputNum (dInput);
			velocity = GetInputNum (vInput);

		}
	}
}
