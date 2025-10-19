using System.Collections;
using System.Collections.Generic;
using UIPanelLib;
using UnityEngine;

public class UIControllerGetter : MonoSingleton<UIControllerGetter>
{
    [field: SerializeField]
    public UIController UIControler_1080_1920 { get; private set; }
    [field: SerializeField]
    public UIController UIControler_LegacyResolution { get; private set; }
    [field: SerializeField]
    public UIController UIController_JourneyCameraLegacyResolution { get; private set; }
}

public static class PanelName
{
    public const string Prebooster = "StateBoosterSelect";
    public const string Pause = "Pause";
    public const string AndroidSettings = "AndroidSettings";
    public const string iOSSettings = "iOSSettings";
    public const string JourneyCurrentGoalPanel = "CurrentGoalPanel";
    public const string JourneyNormalPregame = "PreGameUINormal";
    public const string JourneyHardPregame = "PreGameUIHard";
}
