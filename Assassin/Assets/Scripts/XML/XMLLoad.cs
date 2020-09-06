using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class XMLLoad : Singleton<XMLLoad>
{
    public Stages stageList;
    public bool initialized = false;
    public void LoadStages()
    {
       TextAsset StagesXML = (TextAsset)Resources.Load("xml/Stages") ;

        // TextReader StagesXML = (TextReader)Resources.Load("xml/Stages.xml");
        XmlSerializer stageSerializer = new XmlSerializer(typeof(Stages));
        stageList = (Stages)stageSerializer.Deserialize(new StringReader(StagesXML.text));
    }
}
