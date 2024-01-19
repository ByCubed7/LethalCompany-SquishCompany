using SquishCompany.Extensions;
using System.Linq;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;

namespace SquishCompany.MonoBehaviours
{
    public class CustomPhysicsProp : PhysicsProp
    {
        static public void BroadcastAudioSource(AudioSource audiosource, bool isInShipRoom = false)
        {
            float volume = Mathf.Sqrt(audiosource.maxDistance * audiosource.volume);
            Plugin.logger.LogInfo($"Playing {audiosource.name} at {volume} volume");
            RoundManager.Instance.PlayAudibleNoise(audiosource.transform.position, volume, 0.5f, 0, isInShipRoom);
        }

        public virtual void DEBUG_PRINT_STATE()
        {
            Debug.Log("");
            Debug.Log("#### ############### ####");
            Debug.Log("#### GrabbableObject ####");

            Debug.Log($"- grabbable {grabbable}");
            Debug.Log($"- isHeld {isHeld}");
            Debug.Log($"- isHeldByEnemy {isHeldByEnemy}");
            Debug.Log($"- deactivated {deactivated}");
            Debug.Log($"- parentObject {parentObject}");

            Debug.Log($"- targetFloorPosition {targetFloorPosition}");
            Debug.Log($"- startFallingPosition {startFallingPosition}");
            Debug.Log($"- floorYRot {floorYRot}");
            Debug.Log($"- fallTime {fallTime}");
            Debug.Log($"- hasHitGround {hasHitGround}");
            Debug.Log($"- scrapValue {scrapValue}");
            Debug.Log($"- itemUsedUp {itemUsedUp}");
            Debug.Log($"- playerHeldBy {playerHeldBy}");
            Debug.Log($"- isPocketed {isPocketed}");
            Debug.Log($"- isBeingUsed {isBeingUsed}");
            Debug.Log($"- isInElevator {isInElevator}");
            Debug.Log($"- isInShipRoom {isInShipRoom}");
            Debug.Log($"- isInFactory {isInFactory}");
            Debug.Log($"- useCooldown {useCooldown}");
            Debug.Log($"- currentUseCooldown {currentUseCooldown}");
            Debug.Log($"- itemProperties {itemProperties}");
            Debug.Log($"- insertedBattery {insertedBattery}");
            Debug.Log($"- customGrabTooltip {customGrabTooltip}");

            Debug.Log($"- propBody {propBody}");
            Debug.Log($"- propColliders {propColliders}");
            Debug.Log($"- originalScale {originalScale}");
            Debug.Log($"- wasOwnerLastFrame {wasOwnerLastFrame}");
            Debug.Log($"- mainObjectRenderer {mainObjectRenderer}");
            Debug.Log($"- scrapPersistedThroughRounds {scrapPersistedThroughRounds}");
            Debug.Log($"- heldByPlayerOnServer {heldByPlayerOnServer}");
            Debug.Log($"- radarIcon {radarIcon}");
            Debug.Log($"- reachedFloorTarget {reachedFloorTarget}");
            Debug.Log($"- grabbableToEnemies {grabbableToEnemies}");
        }
    }
}
