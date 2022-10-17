using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeCamera : MonoBehaviour
{
    public GameObject MainCamera;
    public GameObject Camera;
    // Start is called before the first frame update
    void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(ChangeCamera1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeCamera1()
    {
        if(MainCamera.active == false)
        {
            MainCamera.active = true;
            Camera.active = false;
        }
        else
        {
            MainCamera.active = false;
            Camera.active = true;
        }
    }
}
