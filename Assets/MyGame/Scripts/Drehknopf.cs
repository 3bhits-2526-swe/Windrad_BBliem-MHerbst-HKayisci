using UnityEngine;

public class WindradController : MonoBehaviour
{
    // Objekt das sich drehen soll (deine Turbine)
    public Transform turbine;

    // Drehgeschwindigkeit
    public float speed = 200f;

    // Status ob die Turbine läuft
    private bool isRunning = false;

    void Update()
    {
        if (isRunning && turbine != null)
        {
            // Dreht die Turbine um die Z-Achse
            turbine.Rotate(0f, speed * Time.deltaTime, 0f);
        }
    }

    // Diese Funktion dem Button zuweisen
    public void ToggleTurbine()
    {
    Debug.Log("Turbine eingeschaltet");
    isRunning = !isRunning;
    }
}