using System;
using System.Data.Common;
//using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace landmarktest
{
public class HandTracking : MonoBehaviour
{
    // Start is called before the first frame update
    public UDPReceive udpReceive;
    public List<GameObject> handPoints;
    

    [SerializeField]
    private float thumbModelLength = 0.03f;
    private float scale;
    private DepthCalibrator depthCalibrator = new DepthCalibrator(-0.0719f, 0.439f);
    private TransformLink[] transformLinkers;
    public string LinkType = "None";
    int flagR=0;
    int flagL=0;
    float Rx0 = 0;
    float Ry0 = 0;
    float Rz0 = 0;
    float RzD = 0;
    float Lx0 = 0;
    float Ly0 = 0;
    float Lz0 = 0;
    float LzD = 0;

    void Awake()
    {
        transformLinkers = this.transform.GetComponentsInChildren<TransformLink>();
    }


    // Update is called once per frame
    void Update()
    {
        string data = udpReceive.data;
        data = data.Remove(0, 1);
        data =data.Remove(data.Length-1, 1);
        string data1 = data;
        print(data);
        string[] points = null;
        string[] pointsLeft = null;
        string[] pointsRight = null;
        if(data.Contains("Left") == true && data.Contains("Right") == true)
        {
            data =data.Remove(data.LastIndexOf("Right")-3);
            data =data.Remove(0,data.LastIndexOf("Left")+6);
            pointsLeft = data.Split(',');
            data1 =data1.Remove(0,data1.LastIndexOf("Right")+7);
            pointsRight = data1.Split(',');
            
            //print(data1);
        }
        else if(data.Contains("Left") == true && data.Contains("Right") == false)
        {
            data =data.Remove(0,data.LastIndexOf("Left")+6);
            pointsLeft = data.Split(',');           
            print("OnlyL"+data);
        }
        else if(data.Contains("Left") == false && data.Contains("Right") == true)
        {
            data1 =data1.Remove(0,data1.LastIndexOf("Right")+7);
            pointsRight = data1.Split(',');           
            print("OnlyR"+data1);
        }


        //updateLandmarkPosition
        if(LinkType == "Left" && pointsLeft != null)
        {
            for (int i = 1; i<handPoints.Count; i++)
            {

            float x = float.Parse(pointsLeft[i*4])-float.Parse(pointsLeft[0]);
            float y = float.Parse(pointsLeft[i*4+1])-float.Parse(pointsLeft[1]);
            float z = float.Parse(pointsLeft[i*4+2])-float.Parse(pointsLeft[2]);
            print("X"+x);

            LzD = float.Parse(pointsLeft[i*4+3]);

            if (x == 0 && y == 0 && z == 0)
                return;

            handPoints[i].transform.localPosition = new Vector3(x,y,z);         

            }
        }
        
        if(LinkType == "Right" && pointsRight != null)
        {
            for (int i = 1; i<handPoints.Count; i++)
            {

            float x = float.Parse(pointsRight[i*4])-float.Parse(pointsRight[0]);
            float y = float.Parse(pointsRight[i*4+1])-float.Parse(pointsRight[1]);
            float z = float.Parse(pointsRight[i*4+2])-float.Parse(pointsRight[2]);

            RzD = float.Parse(pointsRight[i*4+3]);

            if (x == 0 && y == 0 && z == 0)
                return;

            handPoints[i].transform.localPosition = new Vector3(x,y,z);         

            }
        }

        
        if(LinkType == "Right" && pointsRight != null)
        {
            float RHx=0.557769716f;
            float RHy=0.728625596f;
            float RHz=0.126790136f;
            float depth = depthCalibrator.GetDepthFromThumbLength(scale);
            if (flagR == 0)
            {
                Rx0 = float.Parse(pointsRight[0]);
                Ry0 = float.Parse(pointsRight[1]);
                Rz0 =RzD;
                flagR=1;
            }
            this.transform.localPosition = new Vector3((float.Parse(pointsRight[1])-Ry0)/1000+RHx,(-float.Parse(pointsRight[0])+Rx0)/1000+RHy, RHz+(RzD-Rz0)/200);
            //print(RHz+(zD-z0)/100);
        
        

        //updateLandmarkScale
        var pointA = new Vector3(float.Parse(pointsRight[0]), float.Parse(pointsRight[1]), float.Parse(pointsRight[2]));
        var pointB = new Vector3(float.Parse(pointsRight[4]), float.Parse(pointsRight[5]), float.Parse(pointsRight[6]));
        var thumbDetectedLength = Vector3.Distance(pointA, pointB);
        if (thumbDetectedLength == 0)
            return;
        scale = thumbModelLength / thumbDetectedLength;
        this.transform.localScale = new Vector3(scale, scale, scale);
        }

        if(LinkType == "Left" && pointsLeft != null)
        {
            float LHx=0.460089773f;
            float LHy=0.420398116f;
            float LHz=0.129199326f;
            float depth = depthCalibrator.GetDepthFromThumbLength(scale);
            if (flagL == 0)
            {
                Lx0 = float.Parse(pointsLeft[0]);
                Ly0 = float.Parse(pointsLeft[1]);
                Lz0 =LzD;
                flagL=1;
            }
            this.transform.localPosition = new Vector3((float.Parse(pointsLeft[1])-Ly0)/1000+LHx,(-float.Parse(pointsLeft[0])+Lx0)/1000+LHy, LHz+(LzD-Lz0)/200);
            //print(RHz+(zD-z0)/100);
        
        

        //updateLandmarkScale
        var pointA = new Vector3(float.Parse(pointsLeft[0]), float.Parse(pointsLeft[1]), float.Parse(pointsLeft[2]));
        var pointB = new Vector3(float.Parse(pointsLeft[4]), float.Parse(pointsLeft[5]), float.Parse(pointsLeft[6]));
        var thumbDetectedLength = Vector3.Distance(pointA, pointB);
        if (thumbDetectedLength == 0)
            return;
        scale = thumbModelLength / thumbDetectedLength;
        this.transform.localScale = new Vector3(scale, scale, scale);
        }


        updateWristRotation();
        foreach (var linker in transformLinkers)
        {
            linker.UpdateTransform();
        }

    }

    private void updateWristRotation()
    {
        var wristTransform = handPoints[0].transform;
        var indexFinger = handPoints[5].transform.position;
        var middleFinger = handPoints[9].transform.position;

        var vectorToMiddle = middleFinger - wristTransform.position;
        var vectorToIndex = indexFinger - wristTransform.position;

        Vector3.OrthoNormalize(ref vectorToMiddle, ref vectorToIndex);

        Vector3 normalVector = Vector3.Cross(vectorToIndex, vectorToMiddle);

        wristTransform.rotation = Quaternion.LookRotation(normalVector, vectorToIndex);

    }
}
}