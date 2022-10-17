using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeCube : MonoBehaviour
{
    public GameObject Cube;
    public GameObject Camera;
    // Start is called before the first frame update
    void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(Click1);
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    public void Click1()
    {
        if(Cube.active == false)
        {
            Cube.active = true;
        }
        else
        {
            Cube.active = false;
        }
    }

    public void ChangeCamera()
    {
        if(Cube.active == false)
        {
            Cube.active = true;
            Camera.active = false;
        }
        else
        {
            Cube.active = false;
            Camera.active = true;
        }
    }
}
