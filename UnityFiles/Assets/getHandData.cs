using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Leap;
using Leap.Unity;


public class getHandData : MonoBehaviour
{
    // leapServiceProvider
    private LeapServiceProvider leapServiceProvider;
    // csv data writer
    private StringBuilder csvStringBuilder;
    // file path
    private string filePath;
    // the handdata to be stored
    private List<Vector3> handData;
    // frame count
    private int frameCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        // initialize
        leapServiceProvider = FindObjectOfType<LeapServiceProvider>();

        csvStringBuilder = new StringBuilder();
        filePath = Application.dataPath + "/GestureData.csv";

    }

    // Update is called once per frame
    void Update()
    {
        // pasuse when 100 frames are being collected
        if (frameCount == 1000) {
            Time.timeScale = 0;
            Debug.Log("pause");
        } else {
            
            List<Vector3> handPosition = new List<Vector3>();

            if (leapServiceProvider.CurrentFrame != null)
            {
                if (leapServiceProvider.CurrentFrame.Hands.Count > 0) {
                    // collect data for every 10 frames. stop when 100 frames are collected. 
                    if (frameCount % 10 == 0) 
                    {
                        foreach (Hand hand in leapServiceProvider.CurrentFrame.Hands)
                        {
                            if (hand.IsRight) 
                            {
                                Vector3 palmPosition = hand.PalmPosition;
                                handPosition.Add(palmPosition);

                                Vector3 wristPosition = hand.WristPosition;
                                handPosition.Add(wristPosition);
                                
                                foreach (Finger finger in hand.Fingers)
                                {
                                    for (int i = 0; i < 4; i++)
                                    {
                                        Vector3 jointPosition = finger.Bone((Bone.BoneType)i).NextJoint;
                                        handPosition.Add(jointPosition);
                                    }
                                }
                                                    
                            }
                        }
                        // store the data 
                        foreach (Vector3 pos in handPosition)
                        {
                            // Debug.Log(pos);
                            csvStringBuilder.AppendFormat("{0},{1},{2}\n", pos.x, pos.y, pos.z);
                        }
                    }
                    frameCount++;
                    Debug.Log(frameCount);  
                }
            }

            
            
        }
        
    }

    // when collection is complete, quit the application and write into the csv file
    private void OnApplicationQuit()
    {
        File.WriteAllText(filePath, csvStringBuilder.ToString());
    }
}
  
