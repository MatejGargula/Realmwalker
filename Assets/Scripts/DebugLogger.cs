using Realmwalker.Combat;
using UnityEngine;

public class DebugLogger
{
    public bool showLogs = false;

    public void Log(bool logOn, string message)
    {
        if (logOn) Debug.Log(message);
    }

    public void LogDamage(bool logOn, int damage, CombatCharacter source, CombatCharacter target)
    {
        if (logOn) Debug.Log(source.name + " dealt " + damage + " damage to " + target.name);
    }
}