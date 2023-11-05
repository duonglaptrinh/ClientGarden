using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine.EventSystems;

public static class UIExtension
{
    public static IObservable<Unit> OnSelectUIAsObservable(this UIBehaviour ui) => ui.OnPointerDownAsObservable()
        .SelectMany(_ => Observable.EveryUpdate().TakeUntil(ui.OnPointerUpAsObservable()).AsUnitObservable());
}