using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UIFieldRule
{
    public string prefixName;
    public string typeName;
}

[Serializable,CreateAssetMenu(menuName = "UI/CreateAutoCreateInfoConfig")]
public class UIAutoCreateInfoConfig : ScriptableObject
{
    public List<UIFieldRule> uiInfoList;

    public string UIViewTemplatePath;
    public string UIControlTemplatePath;
    public string ScriptPath;
    public string PrefabPath;
}

