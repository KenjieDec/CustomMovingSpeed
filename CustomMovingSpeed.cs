using UnityEngine;
using HarmonyLib;
using System;
using System.Globalization;
using System.Threading.Tasks;

public class CustomMovingSpeed : Mod
{
    private const string SName = "<color=#0000FF>Custom Moving Speed : </color>";
    private const string Error = "<color=#ff0000>Custom Moving Speed Error : </color>";
    private const string Loaded = "<color=#0000FF>Custom Moving Speed : </color><color=#ffd900>Custom Moving Speed Mod has been Loaded!</color>";
    private const string UnLoaded = "<color=#0000FF>Custom Moving Speed : </color><color#ff0000>Custom Moving Speed Mod has been UnLoaded!</color>";
    private const float defaultSpeed = 1f; // Just for default
    private const float defaultRaft = 1.5f;
    private const float normalWalk = 3f;
    private const float normalSprint = 6f;
    private const float normalSwim = 2f;
    private static float changedRaft = 1.5f;
    private static bool anchored;
    private static bool godmode;
    public void Start()
    {
        Debug.Log(Loaded);
    }

    public override void WorldEvent_WorldLoaded()
    {

    }

    public void OnModUnload()
    {
        Debug.Log(UnLoaded);
        if (!Semih_Network.InLobbyScene)
        {
               Raft raft = ComponentManager<Raft>.Value;
            if (raft != null)
            {
            var defaultRaftSpeed = defaultSpeed * defaultRaft;
            raft.maxSpeed = defaultRaftSpeed;
            raft.maxVelocity = defaultRaftSpeed;
            raft.waterDriftSpeed = defaultRaftSpeed;
            changedRaft = defaultRaftSpeed;
            if (anchored)
            {
                
            }
            else
            {
                ComponentManager<Raft>.Value.RemoveAnchor(10);
            }
        }
            if (godmode)
            {

            }
            else
            {
                GameModeValueManager.GetCurrentGameModeValue().nourishmentVariables.foodDecrementRateMultiplier = 1f;
                GameModeValueManager.GetCurrentGameModeValue().nourishmentVariables.thirstDecrementRateMultiplier = 1f;
                RAPI.GetLocalPlayer().Stats.stat_oxygen.SetOxygenLostPerSecond(1f);
                GameModeValueManager.GetCurrentGameModeValue().playerSpecificVariables.damageTakenMultiplier = 1f;
            }
            RAPI.GetLocalPlayer().PersonController.normalSpeed = 3;
            RAPI.GetLocalPlayer().PersonController.sprintSpeed = 6;
            RAPI.GetLocalPlayer().PersonController.swimSpeed = 2;
        }
    }

    [ConsoleCommand("showSpeed", "Log current speeds")]
    private static void showSpeeds(string[] args)
    {
        if (Semih_Network.InLobbyScene)
        {
            Debug.Log(Error + "Is In Lobby");
            return;
        }
        var showraftspeed = FindObjectOfType<Raft>();
        object value = Traverse.Create(showraftspeed).Field("currentMovementSpeed").GetValue();
        Debug.Log(SName + "\n" +
            "Raft Speed   : " + value.ToString() + "( Default = 1.5 )\n" +
            "Walk Speed   : " + RAPI.GetLocalPlayer().PersonController.normalSpeed + "( Default = 3 )\n" +
            "Sprint Speed : " + RAPI.GetLocalPlayer().PersonController.sprintSpeed + "( Default = 6 )\n" +
            "Swim Speed   : " + RAPI.GetLocalPlayer().PersonController.swimSpeed + "( Default = 2 )"
            );
    }

    #region Raft
    [ConsoleCommand("setRaftSpeed", "Set raft Speed up to 20")]
    private static void setRaftSpeed(string[] args)
    {
        if (Semih_Network.InLobbyScene)
        {
            Debug.Log(Error + "Is In Lobby");
            return;
        }
        if (args.Length < 1)
        {
            Debug.Log(Error + "Unknown Speed");
            return;
        }

        var value = float.Parse(args[0], CultureInfo.InvariantCulture);
        int raftSpeedint = 1;
        if (!int.TryParse(args[0], out raftSpeedint))
        {
            decimal raftSpeedDefault = 1;
            if (!decimal.TryParse(args[0], out raftSpeedDefault))
            {
                Debug.Log(Error + "Invalid Speed");
                return;
            }
            Raft defaultr = ComponentManager<Raft>.Value;
            if ((float) raftSpeedDefault > 25.001f)
            {
                Debug.Log(Error + "Maximum Speed = 25.001");
                return;
            }
            
            if ((float)raftSpeedDefault <= 0f)
            {
                Debug.Log(Error + "Minimum Speed = 1");
                return;
            }
            if (defaultr != null)
            {
                var defaulted = defaultSpeed * ((float)raftSpeedDefault);
                defaultr.maxSpeed = defaulted;
                defaultr.maxVelocity = defaulted;
                defaultr.waterDriftSpeed = defaulted;
                changedRaft = defaulted;
            }
            return;
        }

        if(raftSpeedint > 25.001f)
        {
            Debug.Log(Error + "Maximum Speed = 25.001");
            return;
        }
        if (raftSpeedint <= 0f)
        {
            Debug.Log(Error + "Minimum Speed = 1");
            return;
        }

        Raft raft = ComponentManager<Raft>.Value;
        if (raft != null)
        {
            var moddedSpeed = defaultSpeed * raftSpeedint;
            raft.maxSpeed = moddedSpeed * 4;
            raft.maxVelocity = moddedSpeed;
            raft.waterDriftSpeed = moddedSpeed;
            changedRaft = moddedSpeed;
        }
        else
        {
            Debug.Log(Error + "NoRaft");
            return;
        }
    }

    
    [ConsoleCommand("anchorRaft", "Force Anchor Raft")]
    private static void ForceAnchor()
    {
        if (Semih_Network.InLobbyScene)
        {
            Debug.Log(Error + "Is In Lobby");
            return;
        }
        Raft raft = ComponentManager<Raft>.Value;
        if (raft == null)
        {
            Debug.Log(Error + "NoRaft");
            return;
        }
        anchored = !anchored;
        if (anchored)
        {
            ComponentManager<Raft>.Value.AddAnchor(false, null);
        }
        else
        {
            ComponentManager<Raft>.Value.RemoveAnchor(10);
        }
    }
    #endregion

    #region Walk


    [ConsoleCommand("setWalkSpeed", "Set Walk Speed")]
    private static void setWalkSpeed(string[] args)
    {
        if (Semih_Network.InLobbyScene)
        {
            Debug.Log(Error + "Is In Lobby");
            return;
        }
        if (args.Length < 1)
        {
            Debug.Log(Error + "Unknown Speed");
            return;
        }
        var value = float.Parse(args[0], CultureInfo.InvariantCulture);
        int Speedint = 1;
        if (!int.TryParse(args[0], out Speedint))
        {
                Debug.Log(Error + "Invalid Speed");
                return;
        }

        if(Speedint > 20f)
        {
            Debug.Log(Error + "Max Speed = 20");
            return;
        }
        if (Speedint < 2f)
        {
            Debug.Log(Error + "Minimum Speed = 2");
            return;
        }
        RAPI.GetLocalPlayer().PersonController.normalSpeed = defaultSpeed * Speedint;
    }

    [ConsoleCommand("walkSpeed", "Set Walk Speed")]
    private static void walkSpeed(string[] args)
    {
        if (Semih_Network.InLobbyScene)
        {
            Debug.Log(Error + "Is In Lobby");
            return;
        }
        if (args.Length < 1)
        {
            Debug.Log(Error + "Unknown Speed");
            return;
        }
        var value = float.Parse(args[0], CultureInfo.InvariantCulture);
        int Speedint = 1;
        if (!int.TryParse(args[0], out Speedint))
        {
            Debug.Log(Error + "Invalid Speed");
            return;
        }

        if (Speedint > 20f)
        {
            Debug.Log(Error + "Max Speed = 20");
            return;
        }
        if (Speedint < 2f)
        {
            Debug.Log(Error + "Minimum Speed = 2");
            return;
        }
        RAPI.GetLocalPlayer().PersonController.normalSpeed = defaultSpeed * Speedint;
    }
    #endregion

    #region Sprint
    [ConsoleCommand("setSprintSpeed", "Set Sprint Speed")]
    private static void setSprintSpeed(string[] args)
    {
        if (Semih_Network.InLobbyScene)
        {
            Debug.Log(Error + "Is In Lobby");
            return;
        }
        if (args.Length < 1)
        {
            Debug.Log(Error + "Unknown Speed");
            return;
        }
        var value = float.Parse(args[0], CultureInfo.InvariantCulture);
        int Speedint = 1;
        if (!int.TryParse(args[0], out Speedint))
        {
            Debug.Log(Error + "Invalid Speed");
            return;
        }

        if (Speedint > 25f)
        {
            Debug.Log(Error + "Max Speed = 25");
            return;
        }
        if (Speedint < 5f)
        {
            Debug.Log(Error + "Minimum Speed = 5");
            return;
        }
        RAPI.GetLocalPlayer().PersonController.sprintSpeed = defaultSpeed * Speedint;
    }

    [ConsoleCommand("sprintSpeed", "Set Sprint Speed")]
    private static void sprintSpeed(string[] args)
    {
        if (Semih_Network.InLobbyScene)
        {
            Debug.Log(Error + "Is In Lobby");
            return;
        }
        if (args.Length < 1)
        {
            Debug.Log(Error + "Unknown Speed");
            return;
        }
        var value = float.Parse(args[0], CultureInfo.InvariantCulture);
        int Speedint = 1;
        if (!int.TryParse(args[0], out Speedint))
        {
            Debug.Log(Error + "Invalid Speed");
            return;
        }

        if (Speedint > 25f)
        {
            Debug.Log(Error + "Max Speed = 25");
            return;
        }
        if (Speedint < 5f)
        {
            Debug.Log(Error + "Minimum Speed = 5");
            return;
        }
        RAPI.GetLocalPlayer().PersonController.sprintSpeed = defaultSpeed * Speedint;
    }

    #endregion

    #region Swim
    [ConsoleCommand("setSwimSpeed", "Set Swimming Speed")]
    private static void setSwimSpeed(string[] args)
    {
        if (Semih_Network.InLobbyScene)
        {
            Debug.Log(Error + "Is In Lobby");
            return;
        }
        if (args.Length < 1)
        {
            Debug.Log(Error + "Unknown Speed");
            return;
        }
        var value = float.Parse(args[0], CultureInfo.InvariantCulture);
        int Speedint = 1;
        if (!int.TryParse(args[0], out Speedint))
        {
            Debug.Log(Error + "Invalid Speed");
            return;
        }

        if (Speedint > 15f)
        {
            Debug.Log(Error + "Max Speed = 15");
            return;
        }
        if (Speedint < 1f)
        {
            Debug.Log(Error + "min 1");
            return;
        }
        float sS = 1f;
        sS *= (RAPI.GetLocalPlayer().PlayerEquipment.GetEquipedIndexes().Contains(63) ? 1.4f : 1f);
        RAPI.GetLocalPlayer().PersonController.swimSpeed = sS * Speedint;
    }

    [ConsoleCommand("swimSpeed", "Set Swimming Speed")]
    private static void swimSpeed(string[] args)
    {
        if (Semih_Network.InLobbyScene)
        {
            Debug.Log(Error + "Is In Lobby");
            return;
        }
        if (args.Length < 1)
        {
            Debug.Log(Error + "Unknown Speed");
            return;
        }
        var value = float.Parse(args[0], CultureInfo.InvariantCulture);
        int Speedint = 1;
        if (!int.TryParse(args[0], out Speedint))
        {
            Debug.Log(Error + "Invalid Speed");
            return;
        }

        if (Speedint > 15f)
        {
            Debug.Log(Error + "Max Speed = 15");
            return;
        }
        if (Speedint < 1f)
        {
            Debug.Log(Error + "Minimum Speed = 1");
            return;
        }
        float sS = 1f;
        sS *= (RAPI.GetLocalPlayer().PlayerEquipment.GetEquipedIndexes().Contains(63) ? 1.4f : 1f);
        RAPI.GetLocalPlayer().PersonController.swimSpeed = sS * Speedint;
    }

    #endregion

    #region Extras
    [ConsoleCommand("refill", "refill health, thirst and hungry bar")]
    private static void refill(string[] args)
    {
        if (Semih_Network.InLobbyScene)
        {
            Debug.Log(Error + "Is In Lobby");
            return;
        }
        if (args.Length < 1)
        {
            RAPI.GetLocalPlayer().Stats.stat_hunger.Normal.Value = 100;
            RAPI.GetLocalPlayer().Stats.stat_thirst.Value = 100;
            RAPI.GetLocalPlayer().Stats.stat_health.Value = 100;
        }
        else
        {
            Debug.Log(Error + "use cmd \"refill\" only!");
        }
    }

    [ConsoleCommand("godmode", "godmode! Can't die!")]
    private static void godmodeisreal(string[] args)
    {
        if (Semih_Network.InLobbyScene)
        {
            Debug.Log(Error + "Is In Lobby");
            return;
        }
        godmode = !godmode;
        if (godmode)
        {
            GameModeValueManager.GetCurrentGameModeValue().nourishmentVariables.foodDecrementRateMultiplier = 0f;
            GameModeValueManager.GetCurrentGameModeValue().nourishmentVariables.thirstDecrementRateMultiplier = 0f;
            RAPI.GetLocalPlayer().Stats.stat_oxygen.SetOxygenLostPerSecond(0f);
            GameModeValueManager.GetCurrentGameModeValue().playerSpecificVariables.damageTakenMultiplier = 0f;
        }
        else
        {
            GameModeValueManager.GetCurrentGameModeValue().nourishmentVariables.foodDecrementRateMultiplier = 1f;
            GameModeValueManager.GetCurrentGameModeValue().nourishmentVariables.thirstDecrementRateMultiplier = 1f;
            RAPI.GetLocalPlayer().Stats.stat_oxygen.SetOxygenLostPerSecond(1f);
            GameModeValueManager.GetCurrentGameModeValue().playerSpecificVariables.damageTakenMultiplier = 1f;
        }
    }
    #endregion
}