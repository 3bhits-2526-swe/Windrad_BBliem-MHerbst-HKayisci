using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WindradDashboard : MonoBehaviour
{
    private const float AirDensity = 1.225f;

    [Header("Wind Settings")]
    [SerializeField] private float baseWindSpeed = 8f;
    [SerializeField] private float windVariation = 2.5f;
    [SerializeField] private float windVariationSpeed = 0.35f;

    [Header("Calculation Settings")]
    [SerializeField] private float rotorRadiusMeters = 3f;
    [SerializeField] private float maxTurbineEfficiency = 0.59f;
    [SerializeField] private float historySeconds = 30f;
    [SerializeField] private float historySampleInterval = 1f;

    private readonly List<TurbineDisplay> turbines = new();
    private readonly List<HistorySample> history = new();

    private TextMeshProUGUI windText;
    private TextMeshProUGUI totalEfficiencyText;
    private Image totalEfficiencyFill;
    private RectTransform historyChart;
    private float nextHistorySampleTime;

    public float CurrentWindSpeed { get; private set; }
    public float TotalEfficiency { get; private set; }

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

        if (FindFirstObjectByType<WindradDashboard>() != null)
        {
            return;
        }

        GameObject dashboardObject = new("Windrad Dashboard");
        dashboardObject.AddComponent<WindradDashboard>();
    }

    private void Start()
    {
        BuildDashboard();
        RefreshTurbines();
    }

    private void Update()
    {
        if (turbines.Count == 0)
        {
            RefreshTurbines();
        }

        CurrentWindSpeed = baseWindSpeed + Mathf.Sin(Time.time * windVariationSpeed) * windVariation;
        UpdateMetrics();
        UpdateHistory();
    }

    private void RefreshTurbines()
    {
        turbines.Clear();

        Slider[] sliders = FindObjectsByType<Slider>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Slider slider in sliders)
        {
            if (!slider.name.Contains("WindmillSpeedSlider"))
            {
                continue;
            }

            string displayName = FindWindmillName(slider.transform);
            Color color = GetDisplayColor(displayName);
            TurbineDisplay display = CreateTurbineRow(displayName, color);
            display.Slider = slider;
            turbines.Add(display);
        }
    }

    private void BuildDashboard()
    {
        Canvas canvas = CreateDashboardCanvas();

        RectTransform panel = CreatePanel("DashboardPanel", canvas.transform, new Vector2(420f, 360f));
        panel.anchorMin = new Vector2(0f, 0.5f);
        panel.anchorMax = new Vector2(0f, 0.5f);
        panel.pivot = new Vector2(0f, 0.5f);
        panel.anchoredPosition = new Vector2(24f, 0f);

        VerticalLayoutGroup layout = panel.gameObject.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(22, 22, 20, 20);
        layout.spacing = 14f;
        layout.childControlWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        CreateText("DashboardTitle", panel, "Windrad Dashboard", 30f, FontStyles.Bold);
        windText = CreateText("WindText", panel, "Wind: -- m/s", 23f, FontStyles.Bold);

        RectTransform totalRow = CreateMetricBlock("Gesamtwirkungsgrad", panel, new Color(0.2f, 0.72f, 0.55f), out totalEfficiencyText, out totalEfficiencyFill);
        totalRow.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 70f);

        CreateText("TurbineHeader", panel, "Windraeder", 22f, FontStyles.Bold);

        GameObject rows = new("TurbineRows");
        rows.transform.SetParent(panel, false);
        RectTransform rowsRect = rows.AddComponent<RectTransform>();
        rowsRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 150f);
        VerticalLayoutGroup rowsLayout = rows.AddComponent<VerticalLayoutGroup>();
        rowsLayout.spacing = 12f;
        rowsLayout.childControlWidth = true;
        rowsLayout.childControlHeight = false;
        rowsLayout.childForceExpandWidth = true;
        rowsLayout.childForceExpandHeight = false;
        turbineRowsParent = rowsRect;

        historyChart = CreateHistoryChart(panel);
        historyChart.gameObject.SetActive(false);
    }

    private RectTransform turbineRowsParent;

    private Canvas CreateDashboardCanvas()
    {
        GameObject canvasObject = new("WindradDashboardCanvas");
        canvasObject.transform.SetParent(transform, false);

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 30;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    private TurbineDisplay CreateTurbineRow(string displayName, Color color)
    {
        RectTransform row = CreateMetricBlock(displayName, turbineRowsParent, color, out TextMeshProUGUI valueText, out Image fill);
        row.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 48f);

        return new TurbineDisplay
        {
            Name = displayName,
            ValueText = valueText,
            Fill = fill
        };
    }

    private RectTransform CreateMetricBlock(string label, Transform parent, Color fillColor, out TextMeshProUGUI valueText, out Image fillImage)
    {
        GameObject block = new(label + "Block");
        block.transform.SetParent(parent, false);
        RectTransform blockRect = block.AddComponent<RectTransform>();

        VerticalLayoutGroup layout = block.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 3f;
        layout.childControlWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        GameObject header = new(label + "Header");
        header.transform.SetParent(block.transform, false);
        RectTransform headerRect = header.AddComponent<RectTransform>();
        headerRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 20f);

        HorizontalLayoutGroup headerLayout = header.AddComponent<HorizontalLayoutGroup>();
        headerLayout.childControlWidth = true;
        headerLayout.childForceExpandWidth = true;

        CreateText(label + "Label", header.transform, label, 19f, FontStyles.Bold);
        valueText = CreateText(label + "Value", header.transform, "-- %", 19f, FontStyles.Bold);
        valueText.alignment = TextAlignmentOptions.Right;

        GameObject track = new(label + "Gauge");
        track.transform.SetParent(block.transform, false);
        RectTransform trackRect = track.AddComponent<RectTransform>();
        trackRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 16f);
        Image trackImage = track.AddComponent<Image>();
        trackImage.color = new Color(1f, 1f, 1f, 0.18f);

        GameObject fill = new("Fill");
        fill.transform.SetParent(track.transform, false);
        RectTransform fillRect = fill.AddComponent<RectTransform>();
        fillRect.anchorMin = new Vector2(0f, 0f);
        fillRect.anchorMax = new Vector2(1f, 1f);
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        fillImage = fill.AddComponent<Image>();
        fillImage.color = fillColor;
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
        fillImage.fillOrigin = 0;
        fillImage.fillAmount = 0f;

        return blockRect;
    }

    private RectTransform CreatePanel(string name, Transform parent, Vector2 size)
    {
        GameObject panel = new(name);
        panel.transform.SetParent(parent, false);
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);

        Image image = panel.AddComponent<Image>();
        image.color = new Color(0.02f, 0.025f, 0.03f, 0.94f);
        return rect;
    }

    private RectTransform CreateHistoryChart(Transform parent)
    {
        GameObject chart = new("HistoryChart");
        chart.transform.SetParent(parent, false);
        RectTransform rect = chart.AddComponent<RectTransform>();
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 82f);

        Image background = chart.AddComponent<Image>();
        background.color = new Color(1f, 1f, 1f, 0.12f);

        return rect;
    }

    private TextMeshProUGUI CreateText(string name, Transform parent, string text, float size, FontStyles style)
    {
        GameObject textObject = new(name);
        textObject.transform.SetParent(parent, false);
        TextMeshProUGUI textComponent = textObject.AddComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.fontSize = size;
        textComponent.fontStyle = style;
        textComponent.color = Color.white;
        textComponent.enableWordWrapping = false;
        textComponent.outlineWidth = 0.12f;
        textComponent.outlineColor = new Color(0f, 0f, 0f, 0.8f);
        textComponent.raycastTarget = false;

        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size + 8f);
        return textComponent;
    }

    private void UpdateMetrics()
    {
        float totalEfficiency = 0f;
        float rotorArea = Mathf.PI * rotorRadiusMeters * rotorRadiusMeters;
        float availableWindPower = 0.5f * AirDensity * rotorArea * Mathf.Pow(CurrentWindSpeed, 3f);

        foreach (TurbineDisplay turbine in turbines)
        {
            float sliderEfficiency = GetSliderEfficiency(turbine.Slider);
            float turbineEfficiency = sliderEfficiency * maxTurbineEfficiency;
            float outputPower = availableWindPower * turbineEfficiency;

            turbine.ValueText.text = $"{turbineEfficiency * 100f:0}%  {outputPower / 1000f:0.0} kW";
            turbine.Fill.fillAmount = turbineEfficiency / maxTurbineEfficiency;
            totalEfficiency += turbineEfficiency;
        }

        TotalEfficiency = turbines.Count == 0 ? 0f : totalEfficiency / turbines.Count;
        windText.text = $"Wind: {CurrentWindSpeed:0.0} m/s";
        totalEfficiencyText.text = $"{TotalEfficiency * 100f:0}%";
        totalEfficiencyFill.fillAmount = TotalEfficiency / maxTurbineEfficiency;
    }

    private void UpdateHistory()
    {
        if (Time.time < nextHistorySampleTime)
        {
            return;
        }

        nextHistorySampleTime = Time.time + historySampleInterval;
        history.Add(new HistorySample(Time.time, TotalEfficiency));
        history.RemoveAll(sample => Time.time - sample.Time > historySeconds);

        for (int i = historyChart.childCount - 1; i >= 0; i--)
        {
            Destroy(historyChart.GetChild(i).gameObject);
        }

        int sampleCount = Mathf.Min(history.Count, 30);
        for (int i = 0; i < sampleCount; i++)
        {
            HistorySample sample = history[Mathf.Max(0, history.Count - sampleCount + i)];
            GameObject bar = new("HistoryBar");
            bar.transform.SetParent(historyChart, false);
            RectTransform rect = bar.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(i / 30f, 0f);
            rect.anchorMax = new Vector2((i + 0.75f) / 30f, Mathf.Clamp01(sample.Efficiency / maxTurbineEfficiency));
            rect.offsetMin = new Vector2(2f, 2f);
            rect.offsetMax = new Vector2(-2f, -2f);

            Image image = bar.AddComponent<Image>();
            image.color = new Color(0.32f, 0.72f, 1f, 0.85f);
        }
    }

    private static float GetSliderEfficiency(Slider slider)
    {
        if (slider == null || Mathf.Approximately(slider.maxValue, slider.minValue))
        {
            return 0f;
        }

        return Mathf.InverseLerp(slider.minValue, slider.maxValue, slider.value);
    }

    private static string FindWindmillName(Transform source)
    {
        Transform current = source;
        while (current != null)
        {
            if (current.name.Contains("Windmill"))
            {
                return current.name.Replace("(Clone)", string.Empty).Trim();
            }

            current = current.parent;
        }

        return source.name;
    }

    private static Color GetDisplayColor(string name)
    {
        string lowerName = name.ToLowerInvariant();
        if (lowerName.Contains("red"))
        {
            return new Color(0.95f, 0.28f, 0.23f);
        }

        if (lowerName.Contains("green"))
        {
            return new Color(0.24f, 0.75f, 0.38f);
        }

        return new Color(0.27f, 0.55f, 1f);
    }

    private sealed class TurbineDisplay
    {
        public string Name;
        public Slider Slider;
        public TextMeshProUGUI ValueText;
        public Image Fill;
    }

    private readonly struct HistorySample
    {
        public readonly float Time;
        public readonly float Efficiency;

        public HistorySample(float time, float efficiency)
        {
            Time = time;
            Efficiency = efficiency;
        }
    }
}
