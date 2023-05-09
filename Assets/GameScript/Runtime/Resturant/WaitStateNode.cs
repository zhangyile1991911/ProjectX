using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

//临时定义了表格数据
internal class CharacterAppear
{
    public int CharacterId;
    public int DayOfWeek;
    public int startHour;
    public int startMinutes;
    public int endHour;
    public int endMinutes;
    public string CharacterImage;
}

public class WaitStateNode : IStateNode
{
    private StateMachine _machine;
    private CompositeDisposable _handles;
    
    // private IDisposable _clockTopic;
    // private IDisposable _fiveSecondTimer;
    
    private CharacterMgr _characterMgr;
    private RestaurantEnter _restaurant;
    private RestaurantWindow _restaurantWindow;
    private UIManager _uiManager;

    private List<CharacterAppear> appears;
    public void OnCreate(StateMachine machine)
    {
        _machine = machine;
        _restaurant = machine.Owner as RestaurantEnter;
        _handles = new CompositeDisposable();
        makeFakeDate();
    }

    void makeFakeDate()
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
        
    }
    
    public void OnEnter()
    {
        _characterMgr = UniModule.GetModule<CharacterMgr>();
        
        var _clocker = UniModule.GetModule<Clocker>();
        _clocker.Topic.Subscribe(TimeGoesOn).AddTo(_handles);

        // _fiveSecondTimer = Observable.Interval(TimeSpan.FromSeconds(5)).Subscribe(fiveSecondLoop);
        
        _uiManager = UniModule.GetModule<UIManager>();
        _uiManager.OpenUI(UIEnum.RestaurantWindow, null, null);

        var eventModule = UniModule.GetModule<EventModule>();
        eventModule.CharBubbleSub.Subscribe(GenerateChatBubble).AddTo(_handles);
    }

    public void OnExit()
    {
        _handles?.Clear();
    }
    
    public void OnUpdate()
    {
        // foreach (var one in _restaurant.CharacterEnumerable)
        // {
        //     one.CurBehaviour.Update();
        // }
    }

    private async void TimeGoesOn(DateTime dateTime)
    { //时间流逝
        var characters = checkWhoAppear(dateTime);
        if (characters == null) return;
        
        foreach (var one in characters)
        {
            if (_restaurant.ExistCharacter(one.CharacterId))
            {
                continue;
            }
            if (!_restaurant.HaveEmptySeat())
            {
                break;
            }
            var man = await _characterMgr.CreateCharacter(one.CharacterId,one.CharacterImage);
            _restaurant.CharacterTakeRandomSeat(man);

            var seatPoint = _restaurant.TakeSeatPoint(man.SeatIndex);
            var spawnPoint = _restaurant.RandSpawnPoint();
            
            man.CurBehaviour = new CharacterEnterScene(spawnPoint,seatPoint);
        }
    }

    private List<CharacterAppear> checkWhoAppear(DateTime dateTime)
    {
        return appears.Where(one => one.DayOfWeek == (int)dateTime.DayOfWeek)
            .Where(one => dateTime.Hour >= one.startHour)
            .Where(one => dateTime.Minute >= one.startMinutes).ToList();
    }
    
    private void GenerateChatBubble(Character character)
    {
        var chatId = character.HaveChatId();
        if (chatId > 0)
        {
            var uiWindow = _uiManager.Get(UIEnum.RestaurantWindow) as RestaurantWindow;
            uiWindow.GenerateChatBubble(chatId,character,OnClickBubble);
        }
    }

    public void OnClickBubble(ChatBubble bubble)
    {
        _machine.ChangeState<DialogueStateNode>();
    }
}
