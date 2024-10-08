﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public delegate void GameStateChangeHandler();
    public static event GameStateChangeHandler OnUseItem;
    public static event GameStateChangeHandler OnGetItem;
    public static event GameStateChangeHandler OnUpdateItem;

    public static CarController instance;

    public float carHorsePower;
    public TrailRenderer[] trails;

    public SpriteRenderer spriteRenderer;
    public ItemBase itemController;

    public float currentHorsePower;
    float torque = -150f;
    public float speed, steeringAmount;
    float driftSticky = 0.6f;
    float driftSlippy = 1f;
    float maxStickyVelocity = 0.7f;
    float defaultDragValue = 1;
    public float driftFactor;
    public bool inWater;
    bool inDrift;

    public bool boost;
    public bool hasItem = false;


    public bool wrongDirection { get; private set; }
    Vector2 waterVelocity;

    public Rigidbody2D rb;
    public AudioSource driftAudioSource;
    AudioSource carEngine;

    public Sprite[] carSprites;

    private void Awake()
    {
        instance = this;
        inDrift = false;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        carEngine = GetComponent<AudioSource>();
        itemController = GetComponent<ItemBase>();
    }

    private void OnEnable()
    {
        CanvasController.OnResetRace += ResetCar;
    }

    private void Update()
    {
        if(inDrift)
        {
            for (int i = 0; i < trails.Length; i++)
            {
                trails[i].enabled = true;
            }
            if (!driftAudioSource.isPlaying && rb.velocity.magnitude > 0.25f)
                driftAudioSource.PlayOneShot(driftAudioSource.clip);
            else if (rb.velocity.magnitude <= 0.25f)
                driftAudioSource.Stop();
        }
        else
        {
            for (int i = 0; i < trails.Length; i++)
            {
                trails[i].enabled = false;
            }
            if (driftAudioSource.isPlaying)
                driftAudioSource.Stop();
        }
        carEngine.pitch = Mathf.Lerp(0.8f, 3, rb.velocity.magnitude/5);
    }

    void FixedUpdate()
    {
        if (Input.GetButton("Accelerate"))
        {
            HandleAcceleration();
        }
        else 
        {
            if (Input.GetButton("Break"))
            {
                HandleBraking();
            }
        }
        if (Input.GetButton("UseItem"))
        {
            HandleItemUsage();
        }
        HandleSteering();
        HandleDrifting();
        HandleSubmerged();
        UpdateVelocity();
        UpdateDrift();
        UpdateDrag();
    }

    public void HandleAcceleration()
    {
        if (boost)
        {
            rb.AddForce(transform.up * currentHorsePower);
        }
        else
        {
            rb.AddForce(transform.up * currentHorsePower);
        }
    }

    void HandleBraking()
    {
        rb.AddForce(-transform.up * currentHorsePower / 2);
    }

    public void HandleItemUsage()
    {
        if (hasItem == true)
        {
            OnUseItem?.Invoke();
            hasItem = false;
        }
    }

    void HandleSteering()
    {
        steeringAmount = IsDrivingForward() ? Input.GetAxis("Horizontal") : -Input.GetAxis("Horizontal");
        float t = Mathf.Lerp(0, torque, rb.velocity.magnitude);
        rb.angularVelocity = steeringAmount * t;
    }

    void HandleDrifting()
    {
        driftFactor = RightVelocty().magnitude > maxStickyVelocity ? driftSlippy : driftSticky;
        if (Input.GetButton("HandBreak"))
        {
            driftFactor = driftSlippy;
            if (currentHorsePower > 0) currentHorsePower -= Time.deltaTime * 5;
            else currentHorsePower = 0;
        }
        else
        {
            currentHorsePower = carHorsePower;
        }
    }

    public void HandleSubmerged()
    {
        waterVelocity = Vector2.zero;
        if (inWater)
        {
            driftFactor = driftSticky;
            waterVelocity = -Vector2.up * 0.07f;
        }
    }

    void UpdateVelocity()
    {
        rb.velocity = ForwardVelocity() + RightVelocty() * driftFactor + waterVelocity;
    }

    void UpdateDrift()
    {
        inDrift = driftFactor == driftSlippy ? true : false;
    }

    public void ApplySpeedBoost(float boostMultiplier = 2, float boostDuration = 5)
    {
        StartCoroutine(SpeedBoostCoroutine(boostMultiplier, boostDuration));
    }

    private IEnumerator SpeedBoostCoroutine(float boostMultiplier, float boostDuration)
    {
        boost = true;
        carHorsePower *= boostMultiplier;

        yield return new WaitForSeconds(boostDuration);

        boost = false;
        carHorsePower = 6;
    }

    private void UpdateDrag()
    {
        rb.drag = defaultDragValue;
        if (Input.GetButton("HandBreak")) { rb.drag += 2; }
        if (inWater) rb.drag += 1;
    }

    private Vector2 ForwardVelocity()
    {
        // Dot - How much of the velocity is 'going up'
        return transform.up * Vector2.Dot(rb.velocity, transform.up);
    }
    private Vector2 RightVelocty()
    {
        // Dot - How much of the velocity is 'going right'
        return transform.right * Vector2.Dot(rb.velocity, transform.right);
    }

    private bool IsDrivingForward()
    {
        return Vector2.Dot(transform.up.normalized, rb.velocity.normalized) >= 0 ? true : false; 
    }

    public void UpdateItem()
    {
        OnUpdateItem?.Invoke();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("FinishLine"))
        {
            if (Vector2.Dot(rb.velocity, collision.transform.up) >= 0 && !wrongDirection)
            {
                RaceController.instance.StartRace();
            }
            else if(Vector2.Dot(rb.velocity, collision.transform.up) >= 0 && wrongDirection)
            {
                wrongDirection = false;
            }
            else
            {
                wrongDirection = true;
            }
        }
        else if (collision.CompareTag("Water"))
        {
            inWater = true;
            collision.GetComponent<AudioSource>().PlayOneShot(collision.GetComponent<AudioSource>().clip);
        }

        else if (collision.CompareTag("Boost"))
        {
            carHorsePower *= 3.5f;
            boost = true;
        }

        else if (collision.CompareTag("Item"))
        {

            itemController.ID = collision.GetComponent<Item>().ID;
            Debug.Log("Item Controller ID Set to " + itemController.ID);
            collision.GetComponent<Item>().UseItem();
            hasItem = true;
            OnGetItem?.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("FinishLine"))
        {
            if (Vector2.Dot(rb.velocity, collision.transform.up) >= 0 && !wrongDirection)
            {
                wrongDirection = false;
            }
            else if (Vector2.Dot(rb.velocity, collision.transform.up) >= 0 && wrongDirection)
            {
                wrongDirection = false;
            }
            else
            {
                wrongDirection = true;
            }
        }
        else if(collision.CompareTag("Water"))
        {
            inWater = false;
        }

        else if (collision.CompareTag("Boost"))
        {
            boost = false;
            carHorsePower = 6f;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {

            // Calculate bounce direction
            Vector2 bounceDirection = Vector2.Reflect(rb.velocity.normalized, collision.contacts[0].normal);

            // Apply bounce force
            rb.velocity = bounceDirection * rb.velocity.magnitude * 1f;

            float torqueDirection = Vector3.Cross(collision.contacts[0].normal, rb.velocity).z > 0 ? -1f : 1f;

            //Apply a small torque to simulate car rotation on impact
            rb.AddTorque(torqueDirection * 20f);
        }
    }

    private void ResetCar()
    {
        torque = -150f;
        driftSticky = 0.6f;
        driftSlippy = 1f;
        maxStickyVelocity = 0.7f;
        defaultDragValue = 1;
        hasItem = false;
    }
}
