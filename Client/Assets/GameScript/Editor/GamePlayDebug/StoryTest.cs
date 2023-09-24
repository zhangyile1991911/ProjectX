using System;
using System.Collections.Generic;
using cfg.character;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class StoryTest : MonoBehaviour
{
    public Camera MainCamera;
    public Canvas SettingUI;
    public Transform SeatGroupTran;
    public Transform ShopKeeper;
    public TMP_InputField CharacterIds;
    public TMP_InputField StoryId;
    public TMP_Dropdown WeatherSelect;

    public Button StartBtn;
    // Start is called before the first frame update

    private List<Transform> seatNode;
    private List<Transform> ownerSeatnode;
    private RestaurantCharacter mainCharacter;
    private CharacterDialogWindow _dialogWindow;
    private List<int> charIds;
    void Start()
    {
        seatNode = new List<Transform>(4);
        for (int i =0;i < SeatGroupTran.childCount;i++)
        {
            var node = SeatGroupTran.GetChild(i);
            seatNode.Add(node);
        }

        ownerSeatnode = new(2);
        for (int i =0;i < ShopKeeper.childCount;i++)
        {
            var node = ShopKeeper.GetChild(i);
            ownerSeatnode.Add(node);
        }
        
        
        StartBtn.onClick.AddListener(StartDialogue);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _dialogWindow.NextLine();    
        }
    }

    async void StartDialogue()
    {
        charIds ??= new List<int>(5);
        
        var characterIds = CharacterIds.text;
        var idStr = characterIds.Split(';');
        
        foreach (var str in idStr)
        {
            var id = Int32.Parse(str);
            charIds.Add(id);
        }
        
        await loadCharacter(charIds);

        var storyId = Int32.Parse(StoryId.text);
        await loadDialogue(storyId);

        UIManager.Instance.UICamera.transform.position = MainCamera.transform.position;
        
        SettingUI.gameObject.SetActive(false);
    }


    async UniTask loadCharacter(List<int> charIds)
    {
        int seatIndex = 0;
        foreach (var cid in charIds)
        {
            var charObj = await CharacterMgr.Instance.CreateCharacter(cid);
            charObj.SeatOccupy = seatIndex;
            charObj.transform.position = seatNode[seatIndex].position;
            if (mainCharacter == null)
                mainCharacter = (RestaurantCharacter)charObj;
            
            seatIndex++;
        }
        
        var shopKeeper = await CharacterMgr.Instance.CreateCharacter(10004);
        var skTrans = shopKeeper.transform;
        switch (mainCharacter.SeatOccupy)
        {
            case 0:
            case 1:
                skTrans.position = ownerSeatnode[0].position;
                skTrans.localScale = Vector3.one;
                break;
            case 2:
            case 3:
                skTrans.position = ownerSeatnode[1].position;
                skTrans.localScale = new Vector3(-1,1,1);
                break;
        }
    }

    async UniTask loadDialogue(int dialogueId)
    {
        var openData = new CharacterDialogData();
        
        var _curBubbleTB = DataProviderModule.Instance.GetCharacterBubble(dialogueId);
        openData.StoryResPath = _curBubbleTB.DialogueContentRes;
        openData.StoryStartNode = _curBubbleTB.DialogueStartNode;
        openData.StoryComplete = DialogueComplete;
        openData.StoryCharacter = mainCharacter;
        
        await UIManager.Instance.OpenUI(UIEnum.CharacterDialogWindow,openData,UILayer.Center);
        
        _dialogWindow = UIManager.Instance.Get(UIEnum.CharacterDialogWindow) as CharacterDialogWindow;
        _dialogWindow.DialogueRunner.AddCommandHandler<int>("OrderMeal",OrderMealCommand);
        _dialogWindow.DialogueRunner.AddCommandHandler<string>("omakase",omakase);
        _dialogWindow.DialogueRunner.AddCommandHandler<int,string>("HybridOrder",HybridOrder);
        _dialogWindow.DialogueRunner.AddCommandHandler<int>("OrderDrink",OrderDrinkCommand);
        _dialogWindow.DialogueRunner.AddCommandHandler<int>("AddFriend",AddNPCFriendlyValue);
        _dialogWindow.DialogueRunner.AddCommandHandler("CharacterLeave",CharacterLeave);
        _dialogWindow.DialogueRunner.AddCommandHandler<int>("AddNewMenu",AddNewMenuData);
        
        _dialogWindow.DialogueRunner.AddCommandHandler<int>("KickOut",KickOut);
        _dialogWindow.DialogueRunner.AddCommandHandler<int,int>("AdvanceChapter",AdvanceChapter);
        
    }
    
    private void DialogueComplete()
    {
        SettingUI.gameObject.SetActive(true);
        for (int i = 0; i < charIds.Count; i++)
        {
            var charObj = CharacterMgr.Instance.GetCharacterById(charIds[i]);
            CharacterMgr.Instance.RemoveCharacter(charObj);
        }
        
    }
    
    private void AddNPCFriendlyValue(int val)
    {
      
    }
    
    private void omakase(string desc)
    {
       
    }
    
    private void HybridOrder(int menuId,string tags)
    {
        
    }
    
    private void OrderMealCommand(int menuId)
    {
      
    }

    private void OrderDrinkCommand(int drinkId)
    {
    }

    private void CharacterLeave()
    {
        mainCharacter.CurBehaviour = new CharacterLeave();
    }
    private void KickOut(int characterId)
    {
        var characterObj = CharacterMgr.Instance.GetCharacterById(characterId);
        if (characterObj == null)
        {
            Debug.LogError($"KickOut characterId = {characterId} can't find character");
            return;
        }
        characterObj.CurBehaviour = new CharacterLeave();
    }
    
    private void AdvanceChapter(int characterId,int groupId)
    {
        
    }
    
    private void AddNewMenuData(int menuId)
    {
      
    }
    
}
