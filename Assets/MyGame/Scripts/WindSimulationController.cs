using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WindSimulationController : MonoBehaviour
{
    [Header("Wind Settings")]
    [SerializeField] private float maxWindSpeed = 30f;
    [SerializeField] private float startWindSpeed = 8f;

    [Header("Rotation")]
    [SerializeField] private float maxRotorDegreesPerSecond = 720f;

    private readonly List<Transform> rotors = new();
    private readonly List<Slider> windSliders = new();

    public static WindSimulationController Instance { get; private set; }
    public float CurrentWindSpeed { get; private set; }
    public float CurrentWindSpeedKmh => CurrentWindSpeed * 3.6f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void RegisterSceneBootstrap()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        SceneManager.sceneLoaded += HandleSceneLoaded;
        CreateForScene(SceneManager.GetActiveScene());
    }

    private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CreateForScene(scene);
    }

    private static void CreateForScene(Scene scene)
    {
        if (scene.name != "TestScene")
        {
            return;
        }

        if (FindFirstObjectByType<WindSimulationController>() != null)
        {
            return;
        }

        GameObject controllerObject = new("Wind Simulation Controller");
        controllerObject.AddComponent<WindSimulationController>();
    }

    private void Awake()
    {
        Instance = this;
        CurrentWindSpeed = Mathf.Clamp(startWindSpeed, 0f, maxWindSpeed);
    }

    private void Start()
    {
        RefreshWindSliders();
        RefreshRotors();
        SetWindSpeed(CurrentWindSpeed);
    }

    private void Update()
    {
        if (windSliders.Count == 0)
        {
            RefreshWindSliders();
        }

        if (rotors.Count == 0)
        {
            RefreshRotors();
        }

        RotateWindmills();
    }

    public void SetWindSpeed(float windSpeed)
    {
        CurrentWindSpeed = Mathf.Clamp(windSpeed, 0f, maxWindSpeed);
    }

    private void RefreshWindSliders()
    {
        windSliders.Clear();

        Slider[] sliders = FindObjectsByType<Slider>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Slider slider in sliders)
        {
            if (!slider.name.Contains("WindmillSpeedSlider"))
            {
                continue;
            }

            windSliders.Add(slider);
            slider.onValueChanged.RemoveListener(HandleWindSliderChanged);
            slider.onValueChanged.AddListener(HandleWindSliderChanged);
        }

        if (windSliders.Count > 0)
        {
            HandleWindSliderChanged(windSliders[0].value);
        }
    }

    private void HandleWindSliderChanged(float sliderValue)
    {
        if (windSliders.Count == 0)
        {
            return;
        }

        Slider sourceSlider = windSliders[0];
        float normalizedValue = Mathf.InverseLerp(sourceSlider.minValue, sourceSlider.maxValue, sliderValue);
        SetWindSpeed(normalizedValue * maxWindSpeed);
    }

    private void RefreshRotors()
    {
        rotors.Clear();

        Transform[] allTransforms = FindObjectsByType<Transform>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (Transform candidate in allTransforms)
        {
            if (candidate.name == "RotorHub" || candidate.name == "RotorUnit")
            {
                rotors.Add(candidate);
            }
        }
    }

    private void RotateWindmills()
    {
        float normalizedWind = Mathf.InverseLerp(0f, maxWindSpeed, CurrentWindSpeed);
        float degreesThisFrame = normalizedWind * maxRotorDegreesPerSecond * Time.deltaTime;

        foreach (Transform rotor in rotors)
        {
            if (rotor != null)
            {
                rotor.Rotate(0f, degreesThisFrame, 0f, Space.Self);
            }
        }
    }

}
