using System.ComponentModel;
using Unity.Entities;
using UnityEngine;

namespace Components.Projectile
{
    public class TurretBulletGameObjectPrefab : IComponentData
    {
        public GameObject Value;
    }
}