using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace GameScript.CookPlay
{
    public class BarbecueModule : MonoBehaviour
    {
        public Grid BarbecueGrid;
        public Tilemap BarbecueMap;
        public BoxCollider2D RoastArea;
        public TileBase Placeable;
        public TileBase Obstacle;

        [InspectorName("距离热度比例")]
        public AnimationCurve heatCurve;

        private HashSet<Vector3Int> _occupiedGrid;

        private Vector2 _leftBottom;
        private Vector2 _rightTop;

        private List<RoastFood> _roastFoods;

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

            Observable.EveryUpdate()
                .Where(_ => _roastFoods.Count > 0)
                .Subscribe(RoastFood)
                .AddTo(gameObject);
        }

        private void RoastFood(long param)
        {
            foreach (var oneFood in _roastFoods)
            {
                var distance = Vector2.Distance(oneFood.transform.position, RoastArea.transform.position);
                distance = Mathf.Clamp(distance, 0, (RoastArea.size.x + RoastArea.offset.x) / 2f);
                var heat = heatCurve.Evaluate(distance);
                oneFood.AddHeat(heat * Time.deltaTime * 2f);
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