using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.IO;

public class Main : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Model model = DeserializeModel("Models/Demo.xml");
        InitializeModel(model);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void InitializeModel(Model model)
    {
        GameObject canvas = GameObject.Find("Canvas");
        foreach (Wall wall in model.Walls)
        {
            GameObject WallObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            WallObject.transform.parent = canvas.transform;
            
            WallObject.transform.Rotate(wall.Rotation);
            WallObject.transform.localPosition = wall.LocalPosition;
            WallObject.transform.localScale = wall.LocalScale;
            WallObject.name = "Wall";
        }
        foreach (Gate gate in model.Gates)
        {
            GameObject GateObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            GateObject.transform.parent = canvas.transform;

            GateObject.transform.Rotate(gate.Rotation);
            GateObject.transform.localPosition = gate.LocalPosition;
            GateObject.transform.localScale = gate.LocalScale;
            GateObject.name = "Gate";
            GateObject.GetComponent<Renderer>().material.color = new Color(0, 1, 0);
            GateObject.GetComponent<Renderer>().material.shader = Shader.Find("Sprites/Default");
            GateObject.GetComponent<BoxCollider>().isTrigger = true;
            Vector3 size = GateObject.GetComponent<BoxCollider>().size;
            GateObject.GetComponent<BoxCollider>().size = new Vector3(size.x * 10, 1.5f*size.y);
            GateObject.AddComponent<GateOpen>();
        }
    }

    #region XML Deserialization
    private Model DeserializeModel(string filename)
    {
        // Creates an instance of the XmlSerializer class;
        // specifies the type of object to be deserialized.
        XmlSerializer serializer = new XmlSerializer(typeof(Model));
        // If the XML document has been altered with unknown 
        // nodes or attributes, handles them with the 
        // UnknownNode and UnknownAttribute events.
        serializer.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);
        serializer.UnknownAttribute += new XmlAttributeEventHandler(serializer_UnknownAttribute);

        // A FileStream is needed to read the XML document.
        FileStream fs = new FileStream(filename, FileMode.Open);
        
        return (Model) serializer.Deserialize(fs);
    }

    private void serializer_UnknownNode(object sender, XmlNodeEventArgs e)
    {
        Debug.Log("Unknown Node:" + e.Name + "\t" + e.Text);
    }

    private void serializer_UnknownAttribute
    (object sender, XmlAttributeEventArgs e)
    {
        System.Xml.XmlAttribute attr = e.Attr;
        Debug.Log("Unknown attribute " + attr.Name + "='" + attr.Value + "'");
    }
    #endregion
}
