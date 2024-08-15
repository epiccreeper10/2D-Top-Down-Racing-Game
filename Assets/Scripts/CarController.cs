using System.Collections;
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

    float currentHorsePower;
    float torque = -150f;
    float speed, steeringAmount;
    float driftSticky = 0.6f;
    float driftSlippy = 1f;
    float maxStickyVelocity = 0.7f;
    float defaultDragValue = 1;
    bool inWater;
    bool inDrift;
    bool boost;
    bool hasItem = true;

    public bool wrongDirection { get; private set; }
    Vector2 waterVelocity;

    Rigidbody2D rb;
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
            if (boost)
            {
                rb.AddForce(transform.up * currentHorsePower * 5);
            }
            else
            {
                rb.AddForce(transform.up * currentHorsePower);
            }
        }
        else if(Input.GetButton("Break"))
        {
            rb.AddForce(-transform.up * currentHorsePower / 2);
        }
        else if (Input.GetButton("UseItem"))
        {
            if (hasItem == true)
            {
                OnUseItem?.Invoke();
                hasItem = false;
            }
        }

        steeringAmount = IsDrivingForward() ? Input.GetAxis("Horizontal") : -Input.GetAxis("Horizontal");

        float t = Mathf.Lerp(0, torque, rb.velocity.magnitude);
        rb.angularVelocity = steeringAmount * t;


        float driftFactor = RightVelocty().magnitude > maxStickyVelocity ? driftSlippy : driftSticky;
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

        waterVelocity = Vector2.zero;
        if (inWater)
        {
            driftFactor = driftSticky;
            waterVelocity = -Vector2.up * 0.07f;
        }

        rb.velocity = ForwardVelocity() + RightVelocty() * driftFactor + waterVelocity;

        inDrift = driftFactor == driftSlippy ? true : false;
        UpdateDrag();
    }

    public void ApplySpeedBoost(float boostMultiplier = 2, float boostDuration = 5)
    {
        StartCoroutine(SpeedBoostCoroutine(boostMultiplier, boostDuration));
    }

    private IEnumerator SpeedBoostCoroutine(float boostMultiplier, float boostDuration)
    {
        boost = true;
        currentHorsePower *= boostMultiplier;

        yield return new WaitForSeconds(boostDuration);

        boost = false;
        currentHorsePower = carHorsePower;
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
            boost = true;
        }

        else if (collision.CompareTag("Item"))
        {

            itemController.ID = collision.GetComponent<Item>().ID;
            Debug.Log("Item Controller ID Set to " + itemController.ID);
            Destroy(collision.gameObject);
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
        }
    }
}
