using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Codice.Client.Common;
using UnityEditor;
using UnityEngine;
using Yarn.Compiler;

public class UIClassAutoCreate
{
    private GameObject uiRootGo;

    private class UIDeclaration
    {
        public string DeclarationCode;
        public string InitFindCode;
    }
    private Dictionary<string,UIDeclaration> allNodeInfos = new Dictionary<string, UIDeclaration>();

    //引用了其他的prefab
    private string IgnoreCommonName = "Ins_";
    private UIAutoCreateInfoConfig infoConfig;
    
    public void Create(string uiClassName, GameObject uiRootGo,UIAutoCreateInfoConfig config)
    {
        this.uiRootGo = uiRootGo;
        allNodeInfos.Clear();
        
        infoConfig = config;
        
        FindGoChild(uiRootGo.transform,true);
        
        if (allNodeInfos.Count <= 0)
        {
            Debug.Log("<color=#ff0000>组件数量为0，请确认组件命名是否正确！</color>");
        }
        
        var allDeclaration = new StringBuilder();
        var allFindCode = new StringBuilder();

        foreach (var node in allNodeInfos)
        {
            allDeclaration.Append(node.Value.DeclarationCode);
            allFindCode.Append(node.Value.InitFindCode);
        }
        
        //找到生成UI类模板文件
        var templateAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(config.UIViewTemplatePath);
        var templateFile = templateAsset.text; 
        
        //替换类名
        templateFile = templateFile.Replace("{0}",uiClassName);
        //替换成员变量声明
        templateFile = templateFile.Replace("{1}",allDeclaration.ToString());
        //替换find代码
        templateFile = templateFile.Replace("{2}",allFindCode.ToString());

        string uiVIewFilePath = string.Format("{0}{1}View.cs", config.ScriptPath,uiClassName);
        if (File.Exists(uiVIewFilePath))
        {
            if (EditorUtility.DisplayDialog("警告", "检测到脚本,是否覆盖", "确定","取消"))
            {
                SaveFile(templateFile,uiVIewFilePath);
            }
        }
        else
        {
            SaveFile(templateFile,uiVIewFilePath);
        }

        //生成控制代码
        var controlTemplateFile = AssetDatabase.LoadAssetAtPath<TextAsset>(config.UIControlTemplatePath).text;
        string uiControllerFilePath = string.Format("{0}{1}Control.cs", config.ScriptPath, uiClassName);
        controlTemplateFile = controlTemplateFile.Replace("{0}", uiClassName);
        if (!File.Exists(uiControllerFilePath))
        {
            SaveFile(controlTemplateFile,uiControllerFilePath);
        }
    }

    void SaveFile(string content,string filePath)
    {
        if(File.Exists(filePath))
            File.Delete(filePath);

        using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
        {
            using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
            {
                sw.Write(content);
            }
        }
        Debug.Log("创建成功:"+filePath);
        AssetDatabase.Refresh();
    }

    private void FindGoChild(Transform ts,bool isRoot)
    {
        if (!isRoot)
        {
            CheckUINode(ts);
            if (ts.name.StartsWith(IgnoreCommonName)) return;
        }

        for (int i = 0; i < ts.childCount; i++)
        {
            FindGoChild(ts.GetChild(i),false);
        }
    }

    private void CheckUINode(Transform child)
    {
        //1 确定成员 类型名字
        var fieldTypeInfo = DetermineExportType(child.name);
        if (fieldTypeInfo == null) return;

        string classFieldName = child.name;
        
        var DefineNodeCode = string.Format("\tpublic {0} {1};\n", fieldTypeInfo.typeName, classFieldName);
        //2 UI节点全路径
        var path = GetFullNodePath(child);
        //3 生成查找语句
        var findNodeCode = string.Format("\t\t{0} = go.transform.Find(\"{1}\").GetComponent<{2}>();\n",
            classFieldName, path, fieldTypeInfo.typeName);

        if (allNodeInfos.ContainsKey(classFieldName))
        {
            throw new Exception("组件重名!"+path);
        }

        var one = new UIDeclaration()
        {
            DeclarationCode = DefineNodeCode,
            InitFindCode = findNodeCode
        };
        allNodeInfos.Add(classFieldName,one);
    }

    private UIFieldRule DetermineExportType(string transformName)
    { 
        return infoConfig.uiInfoList.
            Where(one => transformName.Contains(one.prefixName)).
            Select(one=>one).
            FirstOrDefault();
    }

    private string GetFullNodePath(Transform transform)
    {
        string path = transform.name;
        while (transform.parent != uiRootGo.transform)
        {
            transform = transform.parent;
            path = transform.name + "/" + path;
        }

        return path;
    }
}
