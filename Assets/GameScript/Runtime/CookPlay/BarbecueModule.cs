using System;
using System.Collections.Generic;
using Cysharp.Text;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Tilemaps;
using YooAsset;
using Random = UnityEngine.Random;

namespace GameScript.CookPlay
{
    public class BarbecueModule : MonoBehaviour
    {
        public Grid BarbecueGrid;
        public Tilemap BarbecueMap;
        public BoxCollider2D RoastArea;
        public TileBase Placeable;
        public TileBase Obstacle;
        public Transform FoodGroup;
        public List<Animation> FanAnimations;
        public TextMeshProUGUI GameOverText;
        public TextMeshProUGUI TimerText;
        [InspectorName("距离热度比例")]
        public AnimationCurve heatCurve;

        private HashSet<Vector3Int> _occupiedGrid;

        private Vector2 _leftBottom;
        private Vector2 _rightTop;

        private List<RoastFood> _roastFoods;

        private CompositeDisposable _handler;

        private bool _isStart;

        private BarbecueRecipe _curRecipe;
        private float _addition;//额外加速
        private Subject<bool> _completeTopic;

        private float _remainTimer;
        // Start is called before the first frame update
        void Start()
        {
            Init();
        }

        private void Init()
        {
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
            _roastFoods = new List<RoastFood>();

            _handler = new CompositeDisposable();
            
        }

        public void SetBarbecueFood(BarbecueRecipe recipe)
        {
            _curRecipe = recipe;
            _roastFoods?.Clear();
            _roastFoods ??= new List<RoastFood>(10);
        }

        public void AddRoastFood(int fid)
        {
            var go = YooAssets.LoadAssetSync<GameObject>("Assets/GameRes/Prefabs/RoastGrape.prefab").AssetObject as GameObject;
            var ins = Instantiate(go,FoodGroup);
            var x = Random.Range(4.0f, 7.0f);
            ins.transform.localPosition = new Vector3(x, 0, 0);
            var rf = ins.GetComponent<RoastFood>();
            rf.Module = this;
            _roastFoods.Add(rf);
        }
        
        public void StartBarbecue()
        {
            _handler?.Clear();

            _isStart = true;
            _addition = 0;
            _remainTimer = _curRecipe.duration;
            GameOverText.gameObject.SetActive(false);
            this.UpdateAsObservable()
                .Where(_ => _roastFoods.Count > 0&&_isStart)
                .Subscribe(RoastFood)
                .AddTo(_handler);
            
            this.UpdateAsObservable()
                .Where(_ => _isStart && Input.GetKeyDown(KeyCode.Space))
                .Subscribe(_ =>
                {//每按一次空格就加速一点
                    _addition += _curRecipe.FanAddValue;
                    _addition = Mathf.Clamp(_addition, 0, _curRecipe.AddValueLimit);
                    AccelerateFanAnimation(1.0f+_addition/_curRecipe.AddValueLimit);
                })
                .AddTo(_handler);

            Observable.Interval(TimeSpan.FromSeconds(1))
                .Where(_=>_isStart)
                .Subscribe(TemperatureAttenuate).AddTo(_handler);

            var timer = Observable
                .Timer(TimeSpan.FromSeconds(_curRecipe.duration))
                .Select(_=>false);
            
            _completeTopic?.Dispose();
            _completeTopic = new Subject<bool>();
            Observable.Amb(timer, _completeTopic).Subscribe(GameOver).AddTo(_handler);
        }

        private void GameOver(bool success)
        {//todo 清理资源
            _isStart = false;
            _handler.Clear();
            GameOverText.gameObject.SetActive(true);
        }

        private void TemperatureAttenuate(long param)
        {
            _remainTimer -= 1;
            TimerText.text = ZString.Format("{0}",_remainTimer);
            _addition -= _curRecipe.Attenuation;
            _addition = Mathf.Clamp(_addition, 0, _curRecipe.AddValueLimit);
            if (_addition > 0)
            {
                StartFanAnimation();
            }
            else
            {
                StopFanAnimation();
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
            int count = 0;
            foreach (var oneFood in _roastFoods)
            {
                if(!oneFood.inBox)continue;
                var distance = Vector2.Distance(oneFood.transform.position, RoastArea.transform.position);
                distance = Mathf.Clamp(distance, 0, (RoastArea.size.x + RoastArea.offset.x) / 2f);
                var heat = heatCurve.Evaluate(distance);
                heat += _addition;
                oneFood.AddHeat(heat * Time.deltaTime);
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

        public Vector3 CellCenterInWorld(Vector3Int cellPos)
        {
            return BarbecueGrid.GetCellCenterWorld(cellPos);
        }
        
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
        
        
    }
}