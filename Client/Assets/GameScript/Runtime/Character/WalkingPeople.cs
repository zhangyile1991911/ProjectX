using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.Pool;
using UnityEngine;
using YooAsset;

public class WalkingPeople : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;
    public ObjectPool<WalkingPeople> BelongPool;

    public void Config(int peopleId,Vector3 start,Vector3 destination)
    {
        SpriteRenderer.flipX = start.x < destination.x;
        loadPeopleSprite(peopleId);
        walk(start,destination);
    }

    public void Release()
    {
        SpriteRenderer.sprite = null;
        moveHanlder.Pause();
        moveHanlder?.Kill(false);
        moveHanlder = null;
    }
    
    async void loadPeopleSprite(int peopleId)
    {
        string path="Assets/GameRes/Picture/Restaurant/People/";
        switch (peopleId)
        {
            case 1:
                path += "people01.png";
                break;
            case 2:
                path += "people02.png";
                break;
            case 3:
                path += "people03.png";
                break;
            case 4:
                path += "people04.png";
                break;
            case 5:
                path += "people05.png";
                break;
            case 6:
                path += "people06.png";
                break;
            case 7:
                path += "people07.png";
                break;
            case 8:
                path += "people08.png";
                break;
        }

        var handler = YooAssets.LoadAssetAsync<Sprite>(path);
        await handler.ToUniTask(this);
        SpriteRenderer.sprite = handler.AssetObject as Sprite;
        SpriteRenderer.sortingOrder = (0 - Random.Range(50, 80));
    }
    private Vector3 curPos;
    private Tweener moveHanlder = null;
    private float JumpHeight = 0.3f;
    void walk(Vector3 start,Vector3 destination)
    {
        var delay = 3f + Random.Range(0, 10.0f);
        curPos = start;
        transform.position = curPos;
        moveHanlder = DOTween.To(() => start.x,updateMove, destination.x, 35f).SetDelay(delay).OnComplete(moveComplete).SetAutoKill(true);
    }

    public void PauseWalk()
    {
        if(moveHanlder != null)
            moveHanlder.timeScale = 0;
    }

    public void ResumeWalk()
    {
        if(moveHanlder != null)
            moveHanlder.timeScale = 1;
    }
    
    void updateMove(float x)
    {
        curPos.x = x;
        curPos.y = Mathf.Sin(x)*JumpHeight;
        
        transform.position = curPos;
    }

    void moveComplete()
    {
        BelongPool.Release(this);
        moveHanlder = null;
    }
    
}
