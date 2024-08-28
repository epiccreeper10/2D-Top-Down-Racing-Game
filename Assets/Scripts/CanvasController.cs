using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    public delegate void GameStateChangeHandler();
    public static event GameStateChangeHandler OnResetRace;

    public static int RaceId = 0;

    public GameObject playerCar;
    public GameObject raceCanvas;
    public GameObject menuCanvas;
    public GameObject tracksButtonsHolder;
    public GameObject raceTrackPrefab;
    public GameObject carSelectionCanvas;
    public GameObject itemBox;
    public GameObject itemBoxIcon;

    public Sprite[] itemSprites;

    public RaceTracksHolder raceTracksHolder;

    private void Awake()
    {
        raceCanvas.SetActive(false);        
        playerCar.SetActive(false);

        menuCanvas.SetActive(true);

        for (int i = tracksButtonsHolder.transform.childCount; i > 0; i--)
        {
            Destroy(tracksButtonsHolder.transform.GetChild(i - 1).gameObject);
        }
        for (int i = 0; i < raceTracksHolder.raceTracks.Length; i++)
        {
            int id = i;
            GameObject raceTrack = Instantiate(raceTrackPrefab, tracksButtonsHolder.transform);
            raceTrack.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Track " + (id + 1);
            raceTrack.name = id.ToString();
            raceTrack.GetComponent<Button>().onClick.AddListener(() => LoadTrack(id));
        }
    }

    public void OnEnable()
    {
        CarController.OnUpdateItem += UpdateItemBox;
    }

    public void OnDisable()
    {
        CarController.OnUpdateItem -= UpdateItemBox;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) QuitToMenu();
    }

    private void LoadTrack(int id)
    {
        RaceId = id;
        playerCar.SetActive(true);
        playerCar.transform.position = raceTracksHolder.raceTracks[id].transform.GetChild(0).transform.position;
        playerCar.transform.rotation = raceTracksHolder.raceTracks[id].transform.GetChild(0).transform.rotation;
        raceTracksHolder.raceTracks[id].SetActive(true);
        menuCanvas.SetActive(false);
        raceCanvas.SetActive(true);
        OpenWorldController.instance.ChangeRaceUIvisibility(true);
        RaceController.instance.OnRaceEnable();
    }

    public void LoadOpenWorld()
    {
        playerCar.SetActive(true);
        playerCar.transform.position = OpenWorldController.instance.world.transform.GetChild(0).transform.position;
        playerCar.transform.rotation = OpenWorldController.instance.world.transform.GetChild(0).transform.rotation;
        menuCanvas.SetActive(false);
        raceCanvas.SetActive(true);
        OpenWorldController.instance.world.SetActive(true);
        OpenWorldController.instance.ChangeRaceUIvisibility(false);
    }

    private void QuitToMenu()
    {
        OnResetRace?.Invoke();
        RaceController.instance?.ResetRace();
        OpenWorldController.instance.world.SetActive(false);
        raceCanvas.SetActive(false);
        playerCar.SetActive(false);
        for (int i = 0; i < raceTracksHolder.raceTracks.Length; i++)
        {
            raceTracksHolder.raceTracks[i].SetActive(false);
        }
        menuCanvas.SetActive(true);
    }

    public void Return()
    {
        carSelectionCanvas.SetActive(false);
        menuCanvas.SetActive(true);
    }

    public void ChangeCharacter()
    {
        menuCanvas.SetActive(false);
        carSelectionCanvas.SetActive(true);
    }

    public void UpdateItemBox()
    {
        Debug.Log("CanvasItemBox Update Called");
        int _ID = playerCar.GetComponent<CarController>().itemController.ID;
        Debug.Log("_ID = " + _ID);
        if (_ID <= 0)
        {
            itemBox.SetActive(false);
            itemBoxIcon.SetActive(false);
        }
        else if (_ID > 0 && _ID < 6)
        {
            itemBox.SetActive(true);
            itemBoxIcon.SetActive(true);
            if (_ID == 1)
            {
                itemBoxIcon.GetComponent<Image>().sprite = itemSprites[0];
            }
            else if (_ID == 2)
            {
                itemBoxIcon.GetComponent<Image>().sprite = itemSprites[1];
            }
        }
        else
        {
            itemBox.SetActive(false);
            itemBoxIcon.SetActive(false);
        }
    }

    public void ExitGame()
    {
#if UNITY_EDTIOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
