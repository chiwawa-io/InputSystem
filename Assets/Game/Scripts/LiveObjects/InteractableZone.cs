using System;
using UnityEngine;
using Game.Scripts.UI;
using UnityEngine.InputSystem;


namespace Game.Scripts.LiveObjects
{
    public class InteractableZone : MonoBehaviour
    {
        private enum ZoneType
        {
            Collectable,
            Action,
            HoldAction
        }

        private enum KeyState
        {
            Press,
            PressHold
        }

        [SerializeField]
        private ZoneType _zoneType;
        [SerializeField]
        private int _zoneID;
        [SerializeField]
        private int _requiredID;
        [SerializeField]
        [Tooltip("Press the (---) Key to .....")]
        private string _displayMessage;
        [SerializeField]
        private GameObject[] _zoneItems;
        private bool _inZone = false;
        private bool _itemsCollected = false;
        private bool _actionPerformed = false;
        [SerializeField]
        private Sprite _inventoryIcon;
        [SerializeField]
        private KeyCode _zoneKeyInput;
        [SerializeField]
        private KeyState _keyState;
        [SerializeField]
        private GameObject _marker;

        private bool _inHoldState = false;

        private static int _currentZoneID = 0;
        public static int CurrentZoneID
        {
            get
            {
                return _currentZoneID;
            }
            set
            {
                _currentZoneID = value;

            }
        }


        public static event Action<InteractableZone> onZoneInteractionComplete;
        public static event Action<int> onHoldStarted;
        public static event Action<int> onHoldEnded;

        public static event Action<InteractableZone> onBigBreakPerformed;
        public static event Action<InteractableZone> onMultiBreakPerformed;

        private void OnEnable()
        {
            InteractableZone.onZoneInteractionComplete += SetMarker;
        }

        private void Start()
        {
            GameInputManager.Instance.InputActions.Player.Interact.performed += InteractPerformed;
            GameInputManager.Instance.InputActions.Player.Interact.canceled += InteractCanceled;
            GameInputManager.Instance.InputActions.Player.MultiTapBreak.performed += MultiTapBreak;
            GameInputManager.Instance.InputActions.Player.HoldBreak.canceled += HoldBreak;
        }

        void InteractPerformed(InputAction.CallbackContext context)
        {
            if (!_inZone) return;

            if (_keyState == KeyState.Press)
            {
                switch (_zoneType)
                {
                    case ZoneType.Action:
                        if (_inHoldState == false)
                        {
                            _inHoldState = true;
                            PerformAction();
                            onHoldStarted?.Invoke(_zoneID);
                            UIManager.Instance.DisplayInteractableZoneMessage(false);
                        }
                        break;
                    case ZoneType.Collectable:
                        if (_itemsCollected == false)
                        {
                            CollectItems();
                            _itemsCollected = true;
                            UIManager.Instance.DisplayInteractableZoneMessage(false);
                        }
                        break;
                }
            }
            else if (_keyState == KeyState.PressHold && _inHoldState == false)
            {
                // Start hold action
                _inHoldState = true;
                PerformHoldAction();
                onHoldStarted?.Invoke(_zoneID);
                UIManager.Instance.DisplayInteractableZoneMessage(false);
            }
        }

        void InteractCanceled(InputAction.CallbackContext context)
        {
            if (!_inZone) return;
            if (_keyState == KeyState.PressHold)
            {
                _inHoldState = false;
                onHoldEnded?.Invoke(_zoneID);
            }
        }

        void MultiTapBreak(InputAction.CallbackContext context)
        {
            if (!_inZone) return;
            onMultiBreakPerformed?.Invoke(this);
        }

        void HoldBreak(InputAction.CallbackContext context)
        {
            if (!_inZone) return;
            onBigBreakPerformed?.Invoke(this);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && _currentZoneID > _requiredID)
            {
                switch (_zoneType)
                {
                    case ZoneType.Collectable:
                        if (_itemsCollected == false)
                        {
                            _inZone = true;
                            if (_displayMessage != null)
                            {
                                //string message = $"Press the {_zoneKeyInput.ToString()} key to {_displayMessage}.";
                                string message = $"Press the Interaction key to {_displayMessage}.";
                                UIManager.Instance.DisplayInteractableZoneMessage(true, message);
                            }
                            else
                                //UIManager.Instance.DisplayInteractableZoneMessage(true, $"Press the {_zoneKeyInput.ToString()} key to collect");
                                UIManager.Instance.DisplayInteractableZoneMessage(true, $"Press the Interaction key to perform action");
                        }
                        break;

                    case ZoneType.Action:
                        if (_actionPerformed == false)
                        {
                            _inZone = true;
                            if (_displayMessage != null)
                            {
                                //string message = $"Press the {_zoneKeyInput.ToString()} key to {_displayMessage}.";
                                string message = $"Press the Interaction key to {_displayMessage}.";
                                UIManager.Instance.DisplayInteractableZoneMessage(true, message);
                            }
                            else
                                //UIManager.Instance.DisplayInteractableZoneMessage(true, $"Press the {_zoneKeyInput.ToString()} key to perform action");
                                UIManager.Instance.DisplayInteractableZoneMessage(true, $"Press the Interaction key to perform action");
                        }
                        break;

                    case ZoneType.HoldAction:
                        _inZone = true;
                        if (_displayMessage != null)
                        {
                            //string message = $"Press the {_zoneKeyInput.ToString()} key to {_displayMessage}.";
                            string message = $"Hold the Interaction key to {_displayMessage}.";
                            UIManager.Instance.DisplayInteractableZoneMessage(true, message);
                        }
                        else
                            //UIManager.Instance.DisplayInteractableZoneMessage(true, $"Hold the {_zoneKeyInput.ToString()} key to perform action");
                            UIManager.Instance.DisplayInteractableZoneMessage(true, $"Hold the Interaction key to perform action");
                        break;
                }
            }
        }

        private void Update()
        {
            //if (_inZone == true)
            //{

            //    //if (Input.GetKeyDown(_zoneKeyInput) && _keyState != KeyState.PressHold)
            //    {
            //        //press
            //        switch (_zoneType)
            //        {
            //            case ZoneType.Collectable:
            //                if (_itemsCollected == false)
            //                {
            //                    CollectItems();
            //                    _itemsCollected = true;
            //                    UIManager.Instance.DisplayInteractableZoneMessage(false);
            //                }
            //                break;

            //            case ZoneType.Action:
            //                if (_actionPerformed == false)
            //                {
            //                    PerformAction();
            //                    _actionPerformed = true;
            //                    UIManager.Instance.DisplayInteractableZoneMessage(false);
            //                }
            //                break;
            //        }
            //    }
            //    //else if (Input.GetKey(_zoneKeyInput) && _keyState == KeyState.PressHold && _inHoldState == false)
            //    {
            //        _inHoldState = true;



            //        switch (_zoneType)
            //        {                      
            //            case ZoneType.HoldAction:
            //                PerformHoldAction();
            //                break;           
            //        }
            //    }

            //    //if (Input.GetKeyUp(_zoneKeyInput) && _keyState == KeyState.PressHold)
            //    {
            //        _inHoldState = false;
            //        onHoldEnded?.Invoke(_zoneID);
            //    }


            //}
        }

        private void CollectItems()
        {
            foreach (var item in _zoneItems)
            {
                item.SetActive(false);
            }

            UIManager.Instance.UpdateInventoryDisplay(_inventoryIcon);

            CompleteTask(_zoneID);

            onZoneInteractionComplete?.Invoke(this);

        }

        private void PerformAction()
        {
            foreach (var item in _zoneItems)
            {
                item.SetActive(true);
            }

            if (_inventoryIcon != null)
                UIManager.Instance.UpdateInventoryDisplay(_inventoryIcon);
            _actionPerformed = true;

            onZoneInteractionComplete?.Invoke(this);
        }

        private void PerformHoldAction()
        {
            UIManager.Instance.DisplayInteractableZoneMessage(false);
            onHoldStarted?.Invoke(_zoneID);
        }

        public GameObject[] GetItems()
        {
            return _zoneItems;
        }

        public int GetZoneID()
        {
            return _zoneID;
        }

        public void CompleteTask(int zoneID)
        {
            if (zoneID == _zoneID)
            {
                _currentZoneID++;
                onZoneInteractionComplete?.Invoke(this);
            }
            Debug.Log($"Zone {zoneID} completed. Current Zone ID: {_currentZoneID}");
        }

        public void ResetAction(int zoneID)
        {
            if (zoneID == _zoneID)
                _actionPerformed = false;
        }

        public void SetMarker(InteractableZone zone)
        {
            if (_zoneID == _currentZoneID)
                _marker.SetActive(true);
            else
                _marker.SetActive(false);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _inZone = false;
                UIManager.Instance.DisplayInteractableZoneMessage(false);
            }
        }

        private void OnDisable()
        {
            InteractableZone.onZoneInteractionComplete -= SetMarker;

            GameInputManager.Instance.InputActions.Player.Interact.performed -= InteractPerformed;
            GameInputManager.Instance.InputActions.Player.Interact.canceled -= InteractCanceled;
            GameInputManager.Instance.InputActions.Player.MultiTapBreak.performed -= MultiTapBreak;
            GameInputManager.Instance.InputActions.Player.HoldBreak.canceled -= HoldBreak;
        }

    }
}


