using UniRx;
using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public static partial class DOTweenExtensions
{
    public static IObservable<Tween> OnCompleteAsObservable(this Tween tweener)
    {
        return Observable.Create<Tween>(o =>
        {
            tweener.OnComplete(() =>
            {
                o.OnNext(tweener);
                o.OnCompleted();
            });
            return Disposable.Create(() =>
            {
                tweener.Kill();
            });
        });
    }

    public static IObservable<Sequence> PlayAsObservable(this Sequence sequence)
    {
        return Observable.Create<Sequence>(o =>
        {
            sequence.OnComplete(() =>
            {
                o.OnNext(sequence);
                o.OnCompleted();
            });
            sequence.Play();
            return Disposable.Create(() =>
            {
                sequence.Kill();
            });
        });
    }

#if true //DOTweenPro Extensions
    public static IObservable<DOTweenAnimation> DOPlayAsObservable(
        this DOTweenAnimation animation,
        bool rewind = false)
    {
        return Observable.Create<DOTweenAnimation>(o =>
        {
            if (rewind) animation.DORewind();

            animation.tween.OnComplete(() =>
            {
                o.OnNext(animation);
                o.OnCompleted();
            });
            animation.DOPlay();
            return Disposable.Empty;
        });
    }

    public static IObservable<DOTweenAnimation> DOPlayByIdAsObservable(
        this DOTweenAnimation animation,
        string id,
        bool rewind = false)
    {
        return Observable.Create<DOTweenAnimation>(o =>
        {
            if (rewind) animation.DORewind();

            animation.tween.OnComplete(() =>
            {
                o.OnNext(animation);
                o.OnCompleted();
            });
            animation.DOPlayById(id);
            return Disposable.Empty;
        });
    }

    public static IObservable<DOTweenAnimation> DOPlayAllByIdAsObservable(
        this DOTweenAnimation animation,
        string id,
        bool rewind = false)
    {
        return Observable.Create<DOTweenAnimation>(o =>
        {
            if (rewind) animation.DORewind();

            animation.tween.OnComplete(() =>
            {
                o.OnNext(animation);
                o.OnCompleted();
            });
            animation.DOPlayAllById(id);
            return Disposable.Empty;
        });
    }

    public static IObservable<DOTweenAnimation> DORestartAsObservable(
        this DOTweenAnimation animation,
        bool fromHere = false)
    {
        return Observable.Create<DOTweenAnimation>(o =>
        {
            animation.tween.OnComplete(() =>
            {
                o.OnNext(animation);
                o.OnCompleted();
            });
            animation.DORestart(fromHere);
            return Disposable.Empty;
        });
    }

    public static IObservable<DOTweenAnimation> DORestartByIdAsObservable(
        this DOTweenAnimation animation,
        string id)
    {
        return Observable.Create<DOTweenAnimation>(o =>
        {
            animation.tween.OnComplete(() =>
            {
                o.OnNext(animation);
                o.OnCompleted();
            });
            animation.DORestartById(id);
            return Disposable.Empty;
        });
    }

    public static IObservable<DOTweenAnimation> DORestartAllByIdAsObservable(
        this DOTweenAnimation animation,
        string id)
    {
        return Observable.Create<DOTweenAnimation>(o =>
        {
            animation.tween.OnComplete(() =>
            {
                o.OnNext(animation);
                o.OnCompleted();
            });
            animation.DORestartAllById(id);
            return Disposable.Empty;
        });
    }
    
    
    
#endif //DOTweenPro Extensions
}
