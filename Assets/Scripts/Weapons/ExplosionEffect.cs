// File mới: ExplosionEffect.cs
using UnityEngine;
public class ExplosionEffect : MonoBehaviour
{
    void Start() { Destroy(gameObject, 0.25f); }
}