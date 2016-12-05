using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Collections.Generic;

[XmlRoot("Model")]
public class Model {

    [XmlElement("Settings")]
    public Settings Settings { get; set; }

    [XmlArray("Walls")]
    [XmlArrayItem("Wall", typeof(Wall))]
    public List<Wall> Walls { get; set; }

    [XmlArray("Gates")]
    [XmlArrayItem("Gate", typeof(Gate))]
    public List<Gate> Gates { get; set; }

    public Model()
    {
        Walls = new List<Wall>();
    }
}

[XmlRoot("Settings")]
public class Settings
{
    [XmlElement("MaxX")]
    public int MaxX { get; set; }

    [XmlElement("MaxY")]
    public int MaxY { get; set; }
}

public class BaseWallObject
{
    [XmlElement("Id")]
    public string Id { get; set; }

    [XmlElement("BottomLeftX")]
    public float BottomLeftX { get; set; }

    [XmlElement("BottomLeftY")]
    public float BottomLeftY { get; set; }

    [XmlElement("UpperRightX")]
    public float UpperRightX { get; set; }

    [XmlElement("UpperRightY")]
    public float UpperRightY { get; set; }

    public Vector3 LocalPosition
    {
        get
        {
            return new Vector3((float)(BottomLeftX + UpperRightX) / 2 - 0.5f, (float)(BottomLeftY + UpperRightY) / 2 - 0.5f, -1);
        }
    }

    public Vector3 LocalScale
    {
        get
        {
            return new Vector3(Mathf.Abs(UpperRightX - BottomLeftX), Mathf.Abs(UpperRightY - BottomLeftY), 0);
        }
    }

    public Vector3 Rotation
    {
        get;
        set;
    }

    public override string ToString()
    {
        return "Wall: " + "id: " + Id + "; Bottom left: [" + BottomLeftX + "," + BottomLeftY + "] ; Upper right: [" + UpperRightX + "," + UpperRightY + "]";
    }
}

[XmlRoot("Wall")]
public class Wall : BaseWallObject
{

}

[XmlRoot("Gate")]
public class Gate : BaseWallObject
{

}
