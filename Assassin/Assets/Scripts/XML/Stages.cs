﻿using System;
using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot(ElementName = "Stages"), Serializable]
public class Stages
{
    [XmlElement(ElementName = "Stage")]
    public List<Stage> stage = null;
}

[XmlRoot(ElementName = "Stage"), Serializable]
public class Stage
{

    [XmlElement(ElementName = "StageNumber")]
    public int StageNumber = 0; //stage's number

    [XmlElement(ElementName = "StageArea")]
    public int StageArea = 0; // area of stage (which scene its in )


    [XmlElement(ElementName = "Warriors")]
    public int Warriors = 0; //warriors spawned in this stage

    [XmlElement(ElementName = "Hounds")]
    public int Hounds = 0; //hounds spawned in this stage

    [XmlElement(ElementName = "Bats")]
    public int Bats = 0; //bats spawned in this stage

    [XmlElement(ElementName = "UpgradeRewards")]
    public int UpgradeRewards = 0; //how many skill upgrade rewards the player get after beating the stage


    [XmlElement(ElementName = "UnlockedSkill")]
    public string UnlockedSkill = ""; //skill to be unlocked after beating the stage. leave at empty for no skill unlock


}