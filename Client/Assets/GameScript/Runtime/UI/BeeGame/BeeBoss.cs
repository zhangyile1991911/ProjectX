using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BeeBoss : MonoBehaviour
{
    public RectTransform Controller;
    public RectTransform Shot;
    public RectTransform DialogueRT;
    public TextMeshProUGUI Dialogue;
    private Vector2 newPos;
    private Vector2 destinationPos;
    
    public float FlySpeed => flySpeed;
    private float flySpeed;

    private int bullet_num;
    private int bullet_num_remain;
    private float reload_bullet_interval;//装弹间隔
    private float cur_reload_interval;
    private float shot_invertal;
    private float cur_shot_invertval;

    public bool IsAlive => healthPoint > 0;
    public Vector2 Direction => direction;
    private int healthPoint;
    private Vector2 direction;
    
    private Animator _animator;
    private Image _avatar;
    
    public void ConfigProperty(Vector2 startPos,Vector2 destPos,int hp,float speed = 0.7f,float reload = 2.5f,float shotInterval = 0.2f)
    {
        newPos = startPos;
        destinationPos = destPos;
        
        Controller.anchoredPosition = startPos;
        flySpeed = speed;
        
        cur_reload_interval = reload_bullet_interval = reload;
        cur_shot_invertval = shot_invertal = shotInterval;
        
        direction = (destPos - Controller.anchoredPosition).normalized;
        healthPoint = hp+3;

        bullet_num_remain = bullet_num = Mathf.Clamp(hp,1, 3);
        
        if(_animator == null)
            _animator = GetComponent<Animator>();
        if (_avatar == null)
            _avatar = GetComponent<Image>();
    }
    
    public void SetDestination(Vector2 dest)
    {
        destinationPos = dest;
        direction = (dest - Controller.anchoredPosition).normalized;
    }
    
    public bool Move()
    {
        var dist = Vector2.Distance(Controller.anchoredPosition, destinationPos);
        if (dist <= 0.5f)
        {
            return false;
        }
        newPos.x += direction.x * flySpeed;
        newPos.y += direction.y * flySpeed;
        Controller.anchoredPosition = newPos;
        return true;
    }
    
    public bool Fire()
    {
        if (cur_reload_interval > 0)
        {//还在装弹
            cur_reload_interval -= Time.deltaTime;
            return false;
        }

        if (cur_shot_invertval > 0)
        {
            cur_shot_invertval -= Time.deltaTime;
            return false;
        }
        
        cur_shot_invertval = shot_invertal;
        bullet_num_remain--;
        if(bullet_num_remain < 0)
        {
            cur_reload_interval = reload_bullet_interval;
            bullet_num_remain = bullet_num;
            return false;
        }
        return true;
    }

    public void ShowDialogue(string str)
    {
        Dialogue.text = str;
        DialogueRT.gameObject.SetActive(true);
    }

    public void HideDialogue()
    {
        DialogueRT.gameObject.SetActive(false);
    }

    private readonly Color transparent = new Color(255,255,255,0);
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(tag))
        {
            // Debug.Log("BeeBoss OnTriggerEnter2D");
            healthPoint -= 1;
            healthPoint = Mathf.Max(healthPoint,0);
            _avatar.DOColor(transparent, 0.2f).SetLoops(2, LoopType.Yoyo);
        }
    }

    public void Reset()
    {
        newPos = Vector3.zero;
        destinationPos = Vector3.zero;
        cur_reload_interval = reload_bullet_interval;
        cur_shot_invertval = shot_invertal;
        _avatar.color = Color.white;
        
        HideDialogue();
        flySpeed = 0.5f;
        shot_invertal = 0.5f;
        healthPoint = 3;
    }
    
    public void Show()
    {
        if(_animator)_animator.enabled = true;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        if(_animator)_animator.enabled = false;
        gameObject.SetActive(false);
    }
}
