using System.Linq;
using System.Reflection;
using Unity.Netcode.Components;
using UnityEngine;

namespace SquishCompany.Extensions
{
    public static class Extensions
    {

        static public bool EnsureNetworkTransform(this GameObject gameObject)
        {
            if (!EnsureComponent<NetworkTransform>(gameObject))
                return false;

            // If a component was added, configuare it
            NetworkTransform networkTransform = gameObject.GetComponent<NetworkTransform>();
            networkTransform.SlerpPosition = false;
            networkTransform.Interpolate = false;
            networkTransform.SyncPositionX = false;
            networkTransform.SyncPositionY = false;
            networkTransform.SyncPositionZ = false;
            networkTransform.SyncScaleX = false;
            networkTransform.SyncScaleY = false;
            networkTransform.SyncScaleZ = false;
            networkTransform.UseHalfFloatPrecision = true;

            return true;
        }

        static public bool EnsureComponent<T>(this GameObject gameObject) where T : Component
        {
            if (gameObject.GetComponent<T>() != null) return false;

            Plugin.logger.LogInfo($"Item has no {typeof(T).Name}! Adding it.");
            gameObject.AddComponent<T>();

            return true;
        }


        //static public void OverridePhysicsPropWith(this PhysicsProp prop, PhysicsProp component)
        //{
        //    // I hate this, but nothing tells me any other way of injecting our own custom GrabbableObject
        //    // -> Need to override GrabbableObject methods
        //    // -> Cant include scripts in .asset files and therefore prefabs
        //    // -> No way of accessing events
        //    // ect..

        //    // There's prob a much better way of doing this using reflection but this works for now :thisisfine:
        //    component.grabbable = prop.grabbable;
        //    component.isHeld = prop.isHeld;
        //    component.isHeldByEnemy = prop.isHeldByEnemy;
        //    component.deactivated = prop.deactivated;
        //    component.parentObject = prop.parentObject;
        //    component.targetFloorPosition = prop.targetFloorPosition;
        //    component.startFallingPosition = prop.startFallingPosition;
        //    component.floorYRot = prop.floorYRot;
        //    component.fallTime = prop.fallTime;
        //    component.hasHitGround = prop.hasHitGround;
        //    component.scrapValue = prop.scrapValue;
        //    component.itemUsedUp = prop.itemUsedUp;
        //    component.playerHeldBy = prop.playerHeldBy;
        //    component.isPocketed = prop.isPocketed;
        //    component.isBeingUsed = prop.isBeingUsed;
        //    component.isInElevator = prop.isInElevator;
        //    component.isInShipRoom = prop.isInShipRoom;
        //    component.isInFactory = prop.isInFactory;
        //    component.useCooldown = prop.useCooldown;
        //    component.currentUseCooldown = prop.currentUseCooldown;
        //    component.itemProperties = prop.itemProperties;
        //    component.insertedBattery = prop.insertedBattery;
        //    component.customGrabTooltip = prop.customGrabTooltip;
        //    component.propBody = prop.propBody;
        //    component.propColliders = prop.propColliders;
        //    component.originalScale = prop.originalScale;
        //    component.wasOwnerLastFrame = prop.wasOwnerLastFrame;
        //    component.mainObjectRenderer = prop.mainObjectRenderer;
        //    component.scrapPersistedThroughRounds = prop.scrapPersistedThroughRounds;
        //    component.heldByPlayerOnServer = prop.heldByPlayerOnServer;
        //    component.radarIcon = prop.radarIcon;
        //    component.reachedFloorTarget = prop.reachedFloorTarget;
        //    component.grabbableToEnemies = prop.grabbableToEnemies;
        //    Object.Destroy(prop);
        //}

        static public void OverridePhysicsPropWith<T>(this GameObject gameObject) 
            where T : PhysicsProp
                => gameObject.OverridePhysicsPropWith(typeof(T));

        static public void OverridePhysicsPropWith(this GameObject gameObject, System.Type type)
        {
            PhysicsProp prop = gameObject.GetComponent<PhysicsProp>();
            PhysicsProp component = gameObject.AddComponent(type) as PhysicsProp;
            prop.CopyTo(component);
            Object.Destroy(prop);
        }

        public static T1 CopyTo<T1, T2>(this T1 source, T2 destination)
            where T1 : class
            where T2 : T1
        {
            PropertyInfo[] srcProperties = source.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
            PropertyInfo[] destProperties = destination.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);

            foreach (var property in srcProperties)
            {
                //Debug.Log($"Copying Property {property.Name}");
                var dest = destProperties.FirstOrDefault(x => x.Name == property.Name);
                if (dest.CanWrite)
                    dest.SetValue(destination, property.GetValue(source));
            }


            FieldInfo[] srcFields = source.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetField);
            FieldInfo[] destFields = destination.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetField);

            foreach (var property in srcFields)
            {
                //Debug.Log($"Copying Field {property.Name}");
                var dest = destFields.FirstOrDefault(x => x.Name == property.Name);
                dest.SetValue(destination, property.GetValue(source));
            }

            return source;
        }
    }
}
