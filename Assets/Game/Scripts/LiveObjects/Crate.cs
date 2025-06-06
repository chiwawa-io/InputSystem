using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace Game.Scripts.LiveObjects
{
    public class Crate : MonoBehaviour
    {
        [SerializeField] private float _punchDelay;
        [SerializeField] private GameObject _wholeCrate, _brokenCrate;
        [SerializeField] private Rigidbody[] _pieces;
        [SerializeField] private BoxCollider _crateCollider;
        [SerializeField] private InteractableZone _interactableZone;
        private bool _isReadyToBreak = false;

        private List<Rigidbody> _brakeOff = new List<Rigidbody>();

        private void OnEnable()
        {
            //InteractableZone.onZoneInteractionComplete += InteractableZone_onZoneInteractionComplete;
            InteractableZone.onBigBreakPerformed += InteractableZone_onZoneBigBreakComplete;
            InteractableZone.onMultiBreakPerformed += InteractableZone_onZoneMultiBreakComplete;

        }

        private void InteractableZone_onZoneBigBreakComplete(InteractableZone zone)
        {

            if (_isReadyToBreak == false && _brakeOff.Count > 0)
            {
                _wholeCrate.SetActive(false);
                _brokenCrate.SetActive(true);
                _isReadyToBreak = true;
            }

            //if (_isReadyToBreak && zone.GetZoneId == 6) //Crate zone            
            if (_isReadyToBreak) //Crate zone            
            {
                if (_brakeOff.Count > 0)
                {
                    BreakSmallPart();
                    StartCoroutine(PunchDelay());
                }
                else if (_brakeOff.Count <= 0)
                {
                    _isReadyToBreak = false;
                    _crateCollider.enabled = false;
                    _interactableZone.CompleteTask(6);
                    Debug.Log("Completely Busted");
                }
            }
        }

        private void InteractableZone_onZoneMultiBreakComplete(InteractableZone zone)
        {

            if (_isReadyToBreak == false && _brakeOff.Count > 0)
            {
                _wholeCrate.SetActive(false);
                _brokenCrate.SetActive(true);
                _isReadyToBreak = true;
                GameInputManager.Instance.HoldBreakEnable();
            }

            if (_isReadyToBreak) //Crate zone            
            {
                if (_brakeOff.Count > 0)
                {
                    BreakBigPart();
                    StartCoroutine(PunchDelay());
                }
                else if (_brakeOff.Count <= 0)
                {
                    _isReadyToBreak = false;
                    _crateCollider.enabled = false;
                    _interactableZone.CompleteTask(6);
                    Debug.Log("Completely Busted");
                }
            }
        }

        private void Start()
        {
            _brakeOff.AddRange(_pieces);

        }



        //public void BreakPart()
        //{
        //    int rng = Random.Range(0, _brakeOff.Count);
        //    _brakeOff[rng].constraints = RigidbodyConstraints.None;
        //    _brakeOff[rng].AddForce(new Vector3(1f, 1f, 1f), ForceMode.Force);
        //    _brakeOff.Remove(_brakeOff[rng]);            
        //}

        private void BreakSmallPart()
        {
            int rng = Random.Range(0, _brakeOff.Count);
            Rigidbody part = _brakeOff[rng];
            part.constraints = RigidbodyConstraints.None;
            part.AddForce(new Vector3(1f, 1f, 1f), ForceMode.Force);
            _brakeOff.Remove(part);
        }

        private void BreakBigPart()
        {
            int rng = _brakeOff.Count > 4 ? 4 : _brakeOff.Count;
            Rigidbody part = _brakeOff[rng];
            part.constraints = RigidbodyConstraints.None;
            part.AddForce(new Vector3(1f, 1f, 1f), ForceMode.Force);
            _brakeOff.Remove(part);
        }

        IEnumerator PunchDelay()
        {
            float delayTimer = 0;
            while (delayTimer < _punchDelay)
            {
                yield return new WaitForEndOfFrame();
                delayTimer += Time.deltaTime;
            }

            _interactableZone.ResetAction(6);
        }

        private void OnDisable()
        {
            InteractableZone.onBigBreakPerformed -= InteractableZone_onZoneBigBreakComplete;
            InteractableZone.onMultiBreakPerformed -= InteractableZone_onZoneMultiBreakComplete;
        }
    }
}
