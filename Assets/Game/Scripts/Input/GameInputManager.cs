using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInputManager : MonoBehaviour
{
    public static GameInputManager Instance { get; private set; }
    public GameInputAsset InputActions { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InputActions = new GameInputAsset();
        }
        else
        {
            Debug.LogWarning("Multiple instances of GameInputAsset detected. Using the first instance found.");
        }
    }

    private void OnEnable()
    {
        InputActions.Enable();
        EnablePlayerControls();
        InputActions.Player.HoldBreak.Disable();
    }

    private void OnDisable()
    {
        InputActions.Disable();
        InputActions.Player.Disable();
        InputActions.Drone.Disable();
        InputActions.Forklift.Disable();
        InputActions.Hacked.Disable();
    }
    public void EnablePlayerControls()
    {
        InputActions.Player.Enable();

        InputActions.Drone.Disable();
        InputActions.Forklift.Disable();
        InputActions.Hacked.Disable();
    }

    public void EnableDroneControls()
    {
        InputActions.Drone.Enable();

        InputActions.Player.Disable();
        InputActions.Forklift.Disable();
        InputActions.Hacked.Disable();
    }

    public void EnableForkliftControls()
    {
        InputActions.Forklift.Enable();

        InputActions.Player.Disable();
        InputActions.Drone.Disable();
        InputActions.Hacked.Disable();
    }
    public void EnableHackedControls()
    {
        InputActions.Hacked.Enable();

        InputActions.Player.Disable();
        InputActions.Drone.Disable();
        InputActions.Forklift.Disable();
    }

    public void HoldBreakEnable()
    {
        InputActions.Player.HoldBreak.Enable();
    }
}
