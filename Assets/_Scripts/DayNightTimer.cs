using UnityEngine;
using System.Collections;

public class DayNightTimer : MonoBehaviour {

    public float TimeCycleSpeed = 60.0f;

    private Color timeColor;

	// Use this for initialization
	void Start () {
        timeColor = Color.black;
	}
	
	// Update is called once per frame
	void Update () {

        float TimeCounter = (Time.time / TimeCycleSpeed);
        float TimeAngle = Helper.ClampAngle(Time.time / 360, -360, 360);

        if (TimeCounter < 1) {
            timeColor = new Color(TimeCounter, TimeCounter, TimeCounter);
        } else if (TimeCounter > 1 && TimeCounter < 2) {
            timeColor = new Color(2 - TimeCounter, 2 - TimeCounter, 2 - TimeCounter) + new Color((TimeCounter - 1) * 0.4f, (TimeCounter - 1) * 0.2f, (TimeCounter - 1) * 0.0f);
        }

        // change global light color
        light.color = timeColor;

        // change global light direction
        light.transform.Rotate(new Vector3(TimeAngle, 0, 0));

        Debug.Log(Time.time + "###" + TimeCounter + "?" + TimeAngle);

	}
}
