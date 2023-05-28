//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Bright.Serialization;
using SimpleJSON;


namespace cfg
{ 
   
public sealed partial class Tables
{
    public character.TbBaseInfo TbBaseInfo {get; }
    public character.TbSchedule TbSchedule {get; }
    public character.TbCharacterBubble TbCharacterBubble {get; }
    public food.TbFoodInfo TbFoodInfo {get; }

    public Tables(System.Func<string, JSONNode> loader)
    {
        var tables = new System.Collections.Generic.Dictionary<string, object>();
        TbBaseInfo = new character.TbBaseInfo(loader("character_tbbaseinfo")); 
        tables.Add("character.TbBaseInfo", TbBaseInfo);
        TbSchedule = new character.TbSchedule(loader("character_tbschedule")); 
        tables.Add("character.TbSchedule", TbSchedule);
        TbCharacterBubble = new character.TbCharacterBubble(loader("character_tbcharacterbubble")); 
        tables.Add("character.TbCharacterBubble", TbCharacterBubble);
        TbFoodInfo = new food.TbFoodInfo(loader("food_tbfoodinfo")); 
        tables.Add("food.TbFoodInfo", TbFoodInfo);
        PostInit();

        TbBaseInfo.Resolve(tables); 
        TbSchedule.Resolve(tables); 
        TbCharacterBubble.Resolve(tables); 
        TbFoodInfo.Resolve(tables); 
        PostResolve();
    }

    public void TranslateText(System.Func<string, string, string> translator)
    {
        TbBaseInfo.TranslateText(translator); 
        TbSchedule.TranslateText(translator); 
        TbCharacterBubble.TranslateText(translator); 
        TbFoodInfo.TranslateText(translator); 
    }
    
    partial void PostInit();
    partial void PostResolve();
}

}