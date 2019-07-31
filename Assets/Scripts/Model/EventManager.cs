using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager {

    public delegate void EventReciever(params object[] parameterContainer);
    private static Dictionary<GameEvent, EventReciever> _events;

    public static void AddEventListener(GameEvent eT, EventReciever listener)
    {
        if(_events == null)
        {
            _events = new Dictionary<GameEvent, EventReciever>();
        }
        if(!_events.ContainsKey(eT))
        {
            _events.Add(eT, null);
        }
        _events[eT] += listener;
    }

    public static void RemoveEventListener(GameEvent eT, EventReciever listener)
    {
        if(_events != null)
        {
            if(_events.ContainsKey(eT))
            {
                _events[eT] -= listener;
            }
        }
    }

    public static void DispatchEvent(GameEvent eT)
    {
        DispatchEvent(eT, null);
    }

    public static void DispatchEvent(GameEvent eT, params object[] paramsWrapper)
    {
        if(_events == null)
        {
            Debug.Log("No events suscribed");
            return;
        }
        if(_events.ContainsKey(eT))
        {
            if (_events[eT] != null)
                _events[eT](paramsWrapper);
        }
    }
}

public enum GameEvent
{
    NULL,
    CAR_SPAWN,
    CAR_DESTROY,
    START_GAME,
    END_GAME
}
