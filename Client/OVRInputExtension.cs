using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public static class OVRInputExtension
{
    public static IObservable<Unit> GetDownButtonOVR(this Component component, OVRInput.Button button)
    {
        return component.UpdateAsObservable()
            .Where(_ => OVRInput.GetDown(button))
            .AsUnitObservable();
    }

    public static IObservable<Unit> GetDownButtonOVR(this Component component, OVRInput.RawButton button)
    {
        return component.UpdateAsObservable()
            .Where(_ => OVRInput.GetDown(button))
            .AsUnitObservable();
    }

    public static IObservable<Unit> GetButtonOVR(this Component component, OVRInput.Button button)
    {
        return component.UpdateAsObservable()
            .Where(_ => OVRInput.Get(button))
            .AsUnitObservable();
    }

    public static IObservable<Unit> GetUpButtonOVR(this Component component, OVRInput.Button button)
    {
        return component.UpdateAsObservable()
            .Where(_ => OVRInput.GetUp(button))
            .AsUnitObservable();
    }

    public static IObservable<Unit> GetDownDoubleButtonsOVR(this Component component, OVRInput.Button button1, OVRInput.Button button2)
    {
        return component.UpdateAsObservable()
            .Where(_ => OVRInput.Get(button1) && OVRInput.Get(button2))
            .AsUnitObservable();
    }
}