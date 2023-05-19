using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using UnityEngine;
using YooAsset;

public class CharacterAppear
{
    public int CharacterId;
    public int DayOfWeek;
    public int startHour;
    public int startMinutes;
    public int endHour;
    public int endMinutes;
    public string CharacterImage;
}
public class ChatBubbleMessage
{
    public int ChatId;
    public int CharacterId;
    public int ChatType;//1 正常对话 2 订单需求
    public string Title;
    public string ContentYarn;
}

public class OrderMealInfo
{
    public Character Customer;
    public int MealId;
}


public static class GlobalFunctions
{
    // public static Action<bool> YooAssetsComplete;
    public static IEnumerator InitYooAssets(Action<bool> complete)
    {
        //todo 这部分初始化 之后移动一个专门地方做
        YooAssets.Initialize();

        var package = YooAssets.CreatePackage("DefaultPackage");
        YooAssets.SetDefaultPackage(package);
        
        var initParameters = new EditorSimulateModeParameters();
        initParameters.SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild("DefaultPackage");
        
        // var initParameters = new OfflinePlayModeParameters();
        yield return package.InitializeAsync(initParameters);
        Debug.Log($"YooAssets初始化完成");
        makeFakeDate();
        
        complete?.Invoke(true);
    }

    public static Dictionary<int, ChatBubbleMessage> BubbleMessages;
    
    public static List<CharacterAppear> appears;
    static void makeFakeDate()
    {
        appears = new List<CharacterAppear>();
        appears.Add(new CharacterAppear()
        {
            CharacterId = 1,
            DayOfWeek = 6,
            startHour = 17,
            startMinutes = 21,
            endHour = 20,
            endMinutes = 30,
            CharacterImage = "DeliveryMan"
        });
        
        appears.Add(new CharacterAppear()
        {
            CharacterId = 2,
            DayOfWeek = 6,
            startHour = 18,
            startMinutes = 21,
            endHour = 20,
            endMinutes = 30,
            CharacterImage = "FishMan"
        });

        BubbleMessages = new Dictionary<int, ChatBubbleMessage>();
        
        BubbleMessages.Add(1,new ChatBubbleMessage(){ChatId=1,CharacterId=1,ChatType = 1,Title="你好",ContentYarn=""});
        BubbleMessages.Add(2,new ChatBubbleMessage(){ChatId=1,CharacterId=1,ChatType = 1,Title="好热啊",ContentYarn=""});
        BubbleMessages.Add(3,new ChatBubbleMessage(){ChatId=1,CharacterId=2,ChatType = 1,Title="你好",ContentYarn=""});
        BubbleMessages.Add(4,new ChatBubbleMessage(){ChatId=1,CharacterId=2,ChatType = 1,Title="这里什么时候有这个摊位的？",ContentYarn=""});
        BubbleMessages.Add(5,new ChatBubbleMessage(){ChatId=1,CharacterId=1,ChatType = 2,Title="炒饭",ContentYarn=""});
    }
}


