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
    public character.TbCharacterBubble TbCharacterBubble {get; }
    public character.TbCword TbCword {get; }
    public character.TbBehaviourGroup TbBehaviourGroup {get; }
    public character.TbScheduleGroup TbScheduleGroup {get; }
    public character.TbCharacterPhase TbCharacterPhase {get; }
    public food.TbMaterial TbMaterial {get; }
    public food.TbMenuInfo TbMenuInfo {get; }
    public TbItem TbItem {get; }
    public TbQTE TbQTE {get; }
    public food.TbCookPrefab TbCookPrefab {get; }
    public food.TbFlavour TbFlavour {get; }
    public food.TbQTEGroup TbQTEGroup {get; }
    public system.TbWeatherGroup TbWeatherGroup {get; }
    public system.TbDayScheduler TbDayScheduler {get; }
    public common.TbGlobalConfig TbGlobalConfig {get; }
    public common.TbFlavourStrs TbFlavourStrs {get; }
    public common.TbCookToolStrs TbCookToolStrs {get; }
    public phone.TbAppBaseInfo TbAppBaseInfo {get; }
    public phone.app_news app_news {get; }

    public Tables(System.Func<string, JSONNode> loader)
    {
        var tables = new System.Collections.Generic.Dictionary<string, object>();
        TbBaseInfo = new character.TbBaseInfo(loader("character_tbbaseinfo")); 
        tables.Add("character.TbBaseInfo", TbBaseInfo);
        TbCharacterBubble = new character.TbCharacterBubble(loader("character_tbcharacterbubble")); 
        tables.Add("character.TbCharacterBubble", TbCharacterBubble);
        TbCword = new character.TbCword(loader("character_tbcword")); 
        tables.Add("character.TbCword", TbCword);
        TbBehaviourGroup = new character.TbBehaviourGroup(loader("character_tbbehaviourgroup")); 
        tables.Add("character.TbBehaviourGroup", TbBehaviourGroup);
        TbScheduleGroup = new character.TbScheduleGroup(loader("character_tbschedulegroup")); 
        tables.Add("character.TbScheduleGroup", TbScheduleGroup);
        TbCharacterPhase = new character.TbCharacterPhase(loader("character_tbcharacterphase")); 
        tables.Add("character.TbCharacterPhase", TbCharacterPhase);
        TbMaterial = new food.TbMaterial(loader("food_tbmaterial")); 
        tables.Add("food.TbMaterial", TbMaterial);
        TbMenuInfo = new food.TbMenuInfo(loader("food_tbmenuinfo")); 
        tables.Add("food.TbMenuInfo", TbMenuInfo);
        TbItem = new TbItem(loader("tbitem")); 
        tables.Add("TbItem", TbItem);
        TbQTE = new TbQTE(loader("tbqte")); 
        tables.Add("TbQTE", TbQTE);
        TbCookPrefab = new food.TbCookPrefab(loader("food_tbcookprefab")); 
        tables.Add("food.TbCookPrefab", TbCookPrefab);
        TbFlavour = new food.TbFlavour(loader("food_tbflavour")); 
        tables.Add("food.TbFlavour", TbFlavour);
        TbQTEGroup = new food.TbQTEGroup(loader("food_tbqtegroup")); 
        tables.Add("food.TbQTEGroup", TbQTEGroup);
        TbWeatherGroup = new system.TbWeatherGroup(loader("system_tbweathergroup")); 
        tables.Add("system.TbWeatherGroup", TbWeatherGroup);
        TbDayScheduler = new system.TbDayScheduler(loader("system_tbdayscheduler")); 
        tables.Add("system.TbDayScheduler", TbDayScheduler);
        TbGlobalConfig = new common.TbGlobalConfig(loader("common_tbglobalconfig")); 
        tables.Add("common.TbGlobalConfig", TbGlobalConfig);
        TbFlavourStrs = new common.TbFlavourStrs(loader("common_tbflavourstrs")); 
        tables.Add("common.TbFlavourStrs", TbFlavourStrs);
        TbCookToolStrs = new common.TbCookToolStrs(loader("common_tbcooktoolstrs")); 
        tables.Add("common.TbCookToolStrs", TbCookToolStrs);
        TbAppBaseInfo = new phone.TbAppBaseInfo(loader("phone_tbappbaseinfo")); 
        tables.Add("phone.TbAppBaseInfo", TbAppBaseInfo);
        app_news = new phone.app_news(loader("phone_app_news")); 
        tables.Add("phone.app_news", app_news);
        PostInit();

        TbBaseInfo.Resolve(tables); 
        TbCharacterBubble.Resolve(tables); 
        TbCword.Resolve(tables); 
        TbBehaviourGroup.Resolve(tables); 
        TbScheduleGroup.Resolve(tables); 
        TbCharacterPhase.Resolve(tables); 
        TbMaterial.Resolve(tables); 
        TbMenuInfo.Resolve(tables); 
        TbItem.Resolve(tables); 
        TbQTE.Resolve(tables); 
        TbCookPrefab.Resolve(tables); 
        TbFlavour.Resolve(tables); 
        TbQTEGroup.Resolve(tables); 
        TbWeatherGroup.Resolve(tables); 
        TbDayScheduler.Resolve(tables); 
        TbGlobalConfig.Resolve(tables); 
        TbFlavourStrs.Resolve(tables); 
        TbCookToolStrs.Resolve(tables); 
        TbAppBaseInfo.Resolve(tables); 
        app_news.Resolve(tables); 
        PostResolve();
    }

    public void TranslateText(System.Func<string, string, string> translator)
    {
        TbBaseInfo.TranslateText(translator); 
        TbCharacterBubble.TranslateText(translator); 
        TbCword.TranslateText(translator); 
        TbBehaviourGroup.TranslateText(translator); 
        TbScheduleGroup.TranslateText(translator); 
        TbCharacterPhase.TranslateText(translator); 
        TbMaterial.TranslateText(translator); 
        TbMenuInfo.TranslateText(translator); 
        TbItem.TranslateText(translator); 
        TbQTE.TranslateText(translator); 
        TbCookPrefab.TranslateText(translator); 
        TbFlavour.TranslateText(translator); 
        TbQTEGroup.TranslateText(translator); 
        TbWeatherGroup.TranslateText(translator); 
        TbDayScheduler.TranslateText(translator); 
        TbGlobalConfig.TranslateText(translator); 
        TbFlavourStrs.TranslateText(translator); 
        TbCookToolStrs.TranslateText(translator); 
        TbAppBaseInfo.TranslateText(translator); 
        app_news.TranslateText(translator); 
    }
    
    partial void PostInit();
    partial void PostResolve();
}

}