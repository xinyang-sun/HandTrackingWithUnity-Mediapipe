using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public Transform item;
    public GameObject hands;
    public GameObject Middle;
    public int flag=0;
    public string post;

    // Update is called once per frame
    void Update()
    {
        DistanceHand();
        GrabbingItems();
        if (flag == 1)
        {
            item.parent = this.transform;
            item.transform.localPosition = new Vector3(-0.0524f,0.0138f,-0.0022f); //Vector3(-0.0524f,0.0138f,-0.0022f) Vector3(-0.05380f,0.0069f,-0.0838f)
            item.transform.localEulerAngles =new Vector3(0f,-90f,0f); //Vector3(0f,-90f,0f) Vector3(0f,-150f,-90f)
            flag = 2;
        }

        if(flag == 3)
        {
            item.parent = null;
            item.transform.localPosition = new Vector3(0.227500007f,1.07529998f,0.394f);
            item.transform.localEulerAngles =new Vector3(0f,0f,0f);
            flag = 0;
        }
    }

    private void GrabbingItems()
    {
        float DistanceItem;
        DistanceItem = (hands.transform.position - item.transform.position).magnitude;
        if(DistanceItem < 0.2 & post == "close")
        {
            flag = 1;
        }
        if(post == "open" & flag == 2)
        {
            flag = 3;
        }
    }
    private void DistanceHand()
    {
        float DistanceMH;
        DistanceMH = (hands.transform.position - Middle.transform.position).magnitude;
        if(DistanceMH<0.07)
        {
            post="close";
        }
        if(DistanceMH>0.09)
        {
            post="open";
        }
    }
}
