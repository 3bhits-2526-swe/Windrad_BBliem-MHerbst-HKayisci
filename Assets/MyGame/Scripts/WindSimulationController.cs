using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WindSimulationController : MonoBehaviour
{
    private const float StoppedWindThreshold = 0.001f;

    [Header("Wind Settings")]
    [SerializeField] private float maxWindSpeed = 30f;
    [SerializeField] private float startWindSpeed = 8f;

    [Header("Rotation")]
    [SerializeField] private float maxRotorDegreesPerSecond = 720f;
    [SerializeField] private Vector3 rotorRotationAxis = Vector3.forward;

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

        UpdateWindFromSlider();

        if (rotors.Count == 0)
        {
            RefreshRotors();
        }

        RotateWindmills();
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
            UpdateWindFromSlider();
        }
    }

    private void HandleWindSliderChanged(float sliderValue)
    {
        if (windSliders.Count == 0)
        {
            return;
        }

        Slider sourceSlider = windSliders[0];
        SetWindSpeed(CalculateWindSpeedFromSlider(sourceSlider.minValue, sourceSlider.maxValue, sliderValue, maxWindSpeed));
    }

    private void UpdateWindFromSlider()
    {
        if (windSliders.Count > 0 && windSliders[0] != null)
        {
            HandleWindSliderChanged(windSliders[0].value);
        }
    }

    private void RefreshRotors()
    {
        rotors.Clear();

        Transform[] allTransforms = FindObjectsByType<Transform>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (Transform candidate in allTransforms)
        {
            if (candidate.name != "RotorHub" && candidate.name != "RotorUnit")
            {
                continue;
            }

            if (HasSelectedRotorParent(candidate))
            {
                continue;
            }

            rotors.Add(candidate);
        }
    }

    private static bool HasSelectedRotorParent(Transform candidate)
    {
        Transform parent = candidate.parent;
        while (parent != null)
        {
            if (parent.name == "RotorHub" || parent.name == "RotorUnit")
            {
                return true;
            }

            parent = parent.parent;
        }

        return false;
    }

    private void RotateWindmills()
    {
        float normalizedWind = Mathf.InverseLerp(0f, maxWindSpeed, CurrentWindSpeed);
        if (!ShouldAnimateAtWindSpeed(CurrentWindSpeed))
        {
            return;
        }

        float degreesThisFrame = CalculateRotorStep(normalizedWind, maxRotorDegreesPerSecond, Time.deltaTime);
        foreach (Transform rotor in rotors)
        {
            if (rotor != null)
            {
                rotor.Rotate(rotorRotationAxis.normalized, degreesThisFrame, Space.Self);
            }
        }
    }

    public void SetWindSpeed(float windSpeed)
    {
        CurrentWindSpeed = Mathf.Clamp(windSpeed, 0f, maxWindSpeed);
    }

    public static bool ShouldAnimateAtWindSpeed(float windSpeed)
    {
        return windSpeed > StoppedWindThreshold;
    }

    public static float CalculateWindSpeedFromSlider(float sliderMin, float sliderMax, float sliderValue, float maxWindSpeed)
    {
        if (Mathf.Approximately(sliderMin, sliderMax))
        {
            return 0f;
        }

        float normalizedValue = Mathf.InverseLerp(sliderMin, sliderMax, sliderValue);
        return Mathf.Clamp01(normalizedValue) * maxWindSpeed;
    }

    public static float CalculateRotorStep(float normalizedWind, float maxDegreesPerSecond, float deltaTime)
    {
        if (normalizedWind <= StoppedWindThreshold)
        {
            return 0f;
        }

        return Mathf.Clamp01(normalizedWind) * maxDegreesPerSecond * deltaTime;
    }

}
