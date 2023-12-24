using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using cfg.character;
using cfg.common;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEditor.VersionControl;
using UnityEngine;
using Yarn.Unity;
using YooAsset;
using Random = UnityEngine.Random;

public class RestaurantNPC : RestaurantCharacter
{
    public enum Suffix
    {
        A,B,C
    }
    public Suffix suffix;
    public override void PlayAnimation(behaviour behaviourId)
    {
        string s="";
        switch (suffix)
        {
            case Suffix.A:
                s = "01";
                break;
            case Suffix.B:
                s = "02";
                break;
            case Suffix.C:
                s = "03";
                break;
        }
        switch (behaviourId)
        {
            case behaviour.Eating:
                _animator.Play("eat"+s);
                break;
            case behaviour.Talk:
                _animator.Play("talk"+s);
                break;
            case behaviour.WaitOrder:
                _animator.Play("waiting"+s);
                break;
            case behaviour.WaitReply:
                _animator.Play("waiting"+s);
                break;
            case behaviour.Comment:
                _animator.Play("waiting"+s);
                break;
            default:
                _animator.Play("waiting"+s);
                break;
        }
    }

    public override bool CanOrder()
    {
        return _npcData.TodayOrderCount < 1;
    }

    protected override void LoadTableData()
    {
        base.LoadTableData();

        var dataList = DataProviderModule.Instance.GetCharacterBubbleList(CharacterId);
        for (int i = 0; i < dataList.Count; i++)
        {
            switch (dataList[i].BubbleType)
            {
                case bubbleType.Omakase:
                case bubbleType.HybridOrder:
                case bubbleType.SpecifiedOrder:
                    _orderBubbleTB.Add(dataList[i]);
                    break;
                case bubbleType.SpecifiedComment:
                    SpecifiedOrderComment = dataList[i];
                    break;
                case bubbleType.HybridComment:
                    HybridOrderComment = dataList[i];
                    break;
                case bubbleType.OmakaseComment:
                    OMAKASEComment = dataList[i];
                    break;
            }
        }
    }
    
}