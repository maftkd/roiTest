using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrameRater : MonoBehaviour
{
    private Text frameText;
    // Start is called before the first frame update
    void Start()
    {
        frameText = transform.GetComponent<Text>();
    }

    float fr;
    // Update is called once per frame
    void Update()
    {
        fr = 1 / Time.deltaTime;
        frameText.text = "FPS: " + Mathf.FloorToInt(fr);
    }
}
