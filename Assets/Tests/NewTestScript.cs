using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class CarControllerTests : MonoBehaviour
{
    private GameObject rigidbodyObject;
    private Rigidbody2D rb;
    private GameObject tilemapObject;

    [UnityTest]
    public IEnumerator TestItemUsage()
    {
        var carObject = new GameObject();
        TrailRenderer[] noTrails = new TrailRenderer[0];
        var carController = carObject.AddComponent<CarController>();
        var rb = carObject.AddComponent<Rigidbody2D>();
        var driftAudioSource = carObject.AddComponent<AudioSource>();
        carController.trails = noTrails;
        carController.driftAudioSource = driftAudioSource;
        carController.hasItem = true;

        carController.HandleItemUsage();
        yield return null;

        Assert.IsFalse(carController.hasItem);
    }

    [UnityTest]
    public IEnumerator TestSubmerged()
    {
        var carObject = new GameObject();
        TrailRenderer[] noTrails = new TrailRenderer[0];
        var carController = carObject.AddComponent<CarController>();
        var rb = carObject.AddComponent<Rigidbody2D>();
        var driftAudioSource = carObject.AddComponent<AudioSource>();
        carController.trails = noTrails;
        carController.driftAudioSource = driftAudioSource;
        carController.inWater = true;

        var baseDriftFactor = 1f;
        carController.driftFactor = baseDriftFactor;
        carController.HandleSubmerged();
        yield return null;

        Assert.IsTrue(carController.driftFactor < baseDriftFactor);
    }

    [UnityTest]
    public IEnumerator TestAcceleration()
    {
        var carObject = new GameObject();
        TrailRenderer[] noTrails = new TrailRenderer[0];
        var carController = carObject.AddComponent<CarController>();
        var rb = carObject.AddComponent<Rigidbody2D>();
        var driftAudioSource = carObject.AddComponent<AudioSource>();
        carController.trails = noTrails;
        carController.driftAudioSource = driftAudioSource;
        carController.rb = rb;

        var startingVelocity = rb.velocity;
        carController.boost = false;
        carController.currentHorsePower = 10f;
        carController.HandleAcceleration();
        yield return null;

        Assert.IsFalse(rb.velocity == startingVelocity);
    }

}
