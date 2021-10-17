using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

public class CameraSettingXML : MonoBehaviour {


    [XmlElement("X")]
    public int X;     

    [XmlElement("Y")]
    public int Y;     

    

}
