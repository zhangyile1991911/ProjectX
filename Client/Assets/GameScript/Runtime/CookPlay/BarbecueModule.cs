using System;
using System.Collections.Generic;
using cfg.food;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Tilemaps;
using YooAsset;
using Random = UnityEngine.Random;

namespace GameScript.CookPlay
{
    public class BarbecueModule : CookModule
    {
        public bool IsDebug = false;
        public Grid BarbecueGrid;
        public Tilemap BarbecueMap;
        public BoxCollider2D RoastArea;
        public TileBase Placeable;
        public TileBase Obstacle;
        public Transform FoodGroup;
        public List<Animation> FanAnimations;

        [InspectorName("距离热度比例")]
        public AnimationCurve heatCurve;

        private HashSet<Vector3Int> _occupiedGrid;

        private Vector2 _leftBottom;
        private Vector2 _rightTop;

        private List<RoastFood> _roastFoods;
        

        private BarbecueRecipeDifficulty _curRecipeDifficulty;
        private float _addition;//额外加速
        private Subject<bool> _completeTopic;

        private float _remainTimer;

        private BarbecueWindow _barbecueWindow;

        // private void Init()
        // {
        //     
        //     
        //
        // }

        private MenuInfo _tbMenuInfo;
        void SetBarbecueInfo(PickFoodAndTools recipe)
        {
            // Init();
            _tbMenuInfo = DataProviderModule.Instance.GetMenuInfo(recipe.MenuId);
            if (_tbMenuInfo == null)
            {
                Debug.LogError($"recipe.MenuId {recipe.MenuId} == null");
            }

            loadRoastFood(recipe.CookFoods);
            LoadQTEConfig(recipe.QTEConfigs,RoastArea.transform);
            _recipe = recipe;
            
            _barbecueWindow = UIManager.Instance.Get(UIEnum.BarbecueWindow) as BarbecueWindow;
            
        }
        

        private async void loadRoastFood(List<ItemTableData> foods)
        {
            var handler = YooAssets.LoadAssetAsync<GameObject>("Assets/GameRes/Prefabs/AllFood/BarbecueFood/RoastFood.prefab");
            await handler.ToUniTask(this);
            var roastFoodPrefab = handler.AssetObject as GameObject;
            var mainCamera = Camera.main;
            int total = 0;
            for (int i = 0; i < foods.Count; i++)
            {
                for (int y = 0; y < foods[i].Num; y++)
                {
                    RoastFood roastFoodObj = null;
                    if (total < _roastFoods.Count)
                    {
                        roastFoodObj = _roastFoods[total];
                        roastFoodObj.gameObject.SetActive(true);
                    }
                    else
                    {
                        var go = Instantiate(roastFoodPrefab,FoodGroup);
                        go.name = ZString.Format("food{0}", total);
                        roastFoodObj = go.GetComponent<RoastFood>();
                        _roastFoods.Add(roastFoodObj);
                    }
                
                    var tbItem = DataProviderModule.Instance.GetItemBaseInfo(foods[i].Id);
                    roastFoodObj.Init(tbItem.UiResPath,_curRecipeDifficulty.Level,mainCamera);
                
                    var x = Random.Range(8.9f, 12.74f);
                    roastFoodObj.transform.localPosition = new Vector3(x, 0, 0);
                    roastFoodObj.Module = this;
                    total++;
                }
            }

            for (;total < _roastFoods.Count;total++)
            {
                _roastFoods[total].gameObject.SetActive(false);
            }
            
        }

        private void StartBarbecue()
        {
            IsCooking = true;
            _addition = 0;
            _remainTimer = 0;
            
            // this.UpdateAsObservable()
            //     .Where(_ => Input.GetMouseButtonDown(0))
            //     .Subscribe(CheckHit).AddTo(_handler);
            // this.UpdateAsObservable()
            //     .Where(_ => Input.GetMouseButton(0) && _isDrag)
            //     .Subscribe(MoveFood).AddTo(_handler);
            // this.UpdateAsObservable()
            //     .Where(_ => Input.GetMouseButtonUp(0) && _isDrag) 
            //     .Subscribe(PutFood).AddTo(_handler);
            // this.UpdateAsObservable()
            //     .Where(_=>Input.GetMouseButtonDown(1))
            //     .Subscribe(CheckFlip).AddTo(_handler);
            
            for (int i = 0; i < _roastFoods.Count; i++)
            {
                _roastFoods[i].StartRoast(_handler);
            }

            _barbecueWindow = UIManager.Instance.Get(UIEnum.BarbecueWindow) as BarbecueWindow;
            
            this.UpdateAsObservable()
                .Where(_ => _roastFoods.Count > 0&&IsCooking)
                .Subscribe(RoastFood)
                .AddTo(_handler);
            
            // this.UpdateAsObservable()
            //     .Where(_ => IsCooking && Input.GetKeyDown(KeyCode.Space))
            //     .Subscribe(_ =>
            //     {//每按一次空格就加速一点
            //         _addition += _curRecipeDifficulty.FanAddValue*Time.deltaTime;
            //         _addition = Mathf.Clamp(_addition, 0, _curRecipeDifficulty.AddValueLimit);
            //         AccelerateFanAnimation(1.0f+_addition/_curRecipeDifficulty.AddValueLimit);
            //     })
            //     .AddTo(_handler);
            
            // this.UpdateAsObservable()
            //     .Where(_ => IsCooking && Input.anyKeyDown)
            //     .Subscribe(ListenQteInput)
            //     .AddTo(_handler);

            // Observable.Interval(TimeSpan.FromSeconds(1))
            //     .Where(_=>IsCooking)
            //     .Subscribe(TemperatureAttenuate).AddTo(_handler);
            // this.UpdateAsObservable()
            //     .Where(_=>IsCooking)
            //     .Subscribe(TemperatureAttenuate).AddTo(_handler);

            
            Observable.Interval(TimeSpan.FromSeconds(1))
                .Where(_=>IsCooking)
                .Subscribe(ListenProgress).AddTo(_handler);

            var timer = Observable
                .Timer(TimeSpan.FromSeconds(_curRecipeDifficulty.duration))
                .Select(_=>false);


            _completeTopic?.Dispose();
            _completeTopic = new Subject<bool>();
            Observable.Amb(timer, _completeTopic).Subscribe(GameOver).AddTo(_handler);
        }

        private void PressAccelerate()
        {
            if (Input.GetKey(KeyCode.Space))
            {
                _addition += _curRecipeDifficulty.FanAddValue*Time.deltaTime;
                _addition = Mathf.Clamp(_addition, 0, _curRecipeDifficulty.AddValueLimit);
                AccelerateFanAnimation(1.0f+_addition/_curRecipeDifficulty.AddValueLimit);    
            }
        }

        private void TemperatureAttenuate(Unit param)
        {
            if (!Input.GetKey(KeyCode.Space)) return;
            _addition -= _curRecipeDifficulty.Attenuation*Time.deltaTime;
            _addition = Mathf.Clamp(_addition, 0, _curRecipeDifficulty.AddValueLimit);
            if (_addition > 0)
            {
                StartFanAnimation();
            }
            else
            {
                StopFanAnimation();
            }
        }

        private void ListenProgress(long param)
        {
            _remainTimer += 1;
            float percent = _remainTimer / _curRecipeDifficulty.duration;
            
            for (int i = 0;i < _tbQteInfos.Count;i++)
            {
                var qteInfo = _tbQteInfos[i];
                if (percent >= qteInfo.StartArea && percent < qteInfo.EndArea)
                {
                    _barbecueWindow.ShowQTETip(qteInfo.QteId);
                    break;
                }
            }
        }

        private void StartFanAnimation()
        {
            for (int i = 0; i < FanAnimations.Count; i++)
            {
                var one = FanAnimations[i];
                one.clip.legacy = true;
                one["FanRotate"].speed = 1.0f;
                if(!one.isPlaying) one.Play();
            }
        }

        private void StopFanAnimation()
        {
            for (int i = 0; i < FanAnimations.Count; i++)
            {
                FanAnimations[i].Stop();
            }
        }

        private void AccelerateFanAnimation(float speed)
        {
            for (int i = 0; i < FanAnimations.Count; i++)
            {
                var one = FanAnimations[i];
                if (one.isPlaying)
                {
                    one["FanRotate"].speed = speed;    
                }
            }
        }
        
        private void RoastFood(Unit param)
        {
            PressAccelerate();
            TemperatureAttenuate(param);
            ListenQteInput(param);
            int count = 0;
            foreach (var oneFood in _roastFoods)
            {
                if(!oneFood.inBox)continue;
                var distance = Vector2.Distance(oneFood.transform.position, RoastArea.transform.position);
                distance = Mathf.Clamp(distance, 0, (RoastArea.size.x + RoastArea.offset.x) / 2f);
                var heat = heatCurve.Evaluate(distance);
                heat *= Time.deltaTime;
                heat += _addition*Time.deltaTime;
                oneFood.AddHeat(heat);
                if (oneFood.IsComplete())
                {
                    count++;
                }
            }
            
            if (count == _roastFoods.Count)
            {
                _completeTopic.OnNext(true);
                _completeTopic.OnCompleted();
            }
        }
        
        public bool EntireInArea(Vector2 lb,Vector2 rt)
        {
            if (lb.x > _leftBottom.x && lb.y > _leftBottom.y)
            {
                if (rt.x < _rightTop.x && rt.y < _rightTop.y)
                {
                    return true;
                }
            }
            return false;
        }
        public Vector3 SnapCoordinateToGrid(Vector3 position)
        {
            Vector3Int posInGrid = BarbecueGrid.WorldToCell(position);
            return BarbecueGrid.GetCellCenterWorld(posInGrid);
        }

        public Vector3Int SnapWorldPositionInGrid(Vector3 position)
        {
            return BarbecueGrid.WorldToCell(position);
        }

        // public Vector3 CellCenterInWorld(Vector3Int cellPos)
        // {
        //     return BarbecueGrid.GetCellCenterWorld(cellPos);
        // }
        
        public void FillArea(BoundsInt box)
        {
            foreach (var pos in box.allPositionsWithin)
            {
                BarbecueMap.SetTile(pos,_occupiedGrid.Contains(pos) ? Obstacle : Placeable);
                // Debug.Log($"PossessGrid.Contains(pos) = ${PossessGrid.Contains(pos)}");
            }
        }
        
        public void ClearArea(BoundsInt box)
        {
            foreach (var pos in box.allPositionsWithin)
            {
                BarbecueMap.SetTile(pos,null);
            }
        }
        
        public bool CheckAreaEmpty(BoundsInt box)
        {
            foreach(var pos in box.allPositionsWithin)
            {
                if (_occupiedGrid.Contains(pos))
                {
                    return false;
                }
            }
            return true;
        }

        public void LockArea(BoundsInt box,RoastFood food)
        {
            foreach (var pos in box.allPositionsWithin)
            {
                _occupiedGrid.Add(pos);
            }
            _roastFoods.Add(food);
        }

        public void UnlockArea(BoundsInt box,RoastFood food)
        {
            foreach (var pos in box.allPositionsWithin)
            {
                _occupiedGrid.Remove(pos);
            }
            _roastFoods.Remove(food);
        }
        
        private void ListenQteInput(Unit param)
        {
            if (!Input.anyKeyDown) return;
            for (int i = 0;i < _tbQteInfos.Count;i++)
            {
                var one = _tbQteInfos[i];
                var tbQte = DataProviderModule.Instance.GetQTEInfo(one.QteId);
                var percent = _remainTimer / _curRecipeDifficulty.duration;
                // Debug.Log($" ListenQteInput {percent} one.StartArea = {one.StartArea} one.EndArea = {one.EndArea}");
                if (percent >= one.StartArea && percent <= one.EndArea)
                {
                    var keyDown = Input.GetKeyDown((KeyCode)tbQte.KeyCode);
                    var clicked = _result.QTEResult[one.QteId]; 
                    // Debug.Log($" ListenQteInput {percent} keyDown = {keyDown} clicked = {clicked}");
                    if (keyDown&&clicked==false)
                    {
                        Debug.Log($"ListenQteInput 播放QTE动画");
                        _qteAnimations[i].gameObject.SetActive(true);
                        _qteAnimations[i].Play();
                        _result.QTEResult[one.QteId] = true;
                        _result.Tags.Add(tbQte.Tag);
                        break; 
                    }
                }
            }
        }
        
        public override void Init(PickFoodAndTools foodAndTools,RecipeDifficulty difficulty)
        {
            base.Init(foodAndTools,difficulty);
            
            var size = RoastArea.size;
            var offset = RoastArea.offset;
            var halfWidth = (size.x + offset.x) / 2f;
            var halfHeight = (size.y + offset.y) / 2f;
            Debug.Log($"halfWidth = {halfWidth} halfHeight = {halfHeight}");

            var position = RoastArea.transform.position;
            _leftBottom = new Vector2
            {
                x = position.x - halfWidth,
                y = position.y - halfHeight
            };

            _rightTop = new Vector2
            {
                x = position.x + halfWidth,
                y = position.y + halfHeight
            };
            _occupiedGrid = new HashSet<Vector3Int>();
            _roastFoods ??= new List<RoastFood>();
            
            SetBarbecueInfo(foodAndTools);
            _curRecipeDifficulty = difficulty as BarbecueRecipeDifficulty;
        }
        
        public override void StartCook()
        {
            StartBarbecue();
        }
        
        private void GameOver(bool success)
        {//todo 清理资源
            IsCooking = false;
            _barbecueWindow = null;

            //隐藏
            foreach (var one in _roastFoods)
            {
                one.Reset();
                one.gameObject.SetActive(false);
            }

            //生成评分
            var provider = DataProviderModule.Instance;
            foreach (var food in _recipe.CookFoods)
            {
                var tbFood = provider.GetFoodBaseInfo(food.Id);
                foreach (var one in tbFood.Tag)
                {
                    _result.Tags.Add(one);
                }
                foreach (var one in tbFood.OppositeTag)
                {
                    _result.Tags.Add(one);    
                }
            }

            if (IsDebug)
            {
                //消耗时间
                var clocker = UniModule.GetModule<Clocker>();
                clocker.AddMinute(_tbMenuInfo.CostTime);    
            }
            
            FinishCook?.Invoke(_result);
            
            base.UnloadRes();

            StopFanAnimation();
        }
    }
}

/*
 *

 */