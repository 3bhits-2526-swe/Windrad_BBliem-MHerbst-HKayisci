using UnityEngine;
using TMPro;

public class ObjectSwitcher : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private GameObject[] objects;

    void Start()
    {
        if (dropdown == null || objects.Length == 0)
        {
            Debug.LogWarning("ObjectSwitcher: Bitte Dropdown und Objects zuweisen!");
            return;
        }

        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        ShowObject(0);
    }

    void OnDropdownValueChanged(int index)
    {
        ShowObject(index);
    }

    void ShowObject(int index)
    {
        foreach (GameObject obj in objects)
        {
            obj.SetActive(false);
        }

        if (index >= 0 && index < objects.Length)
        {
            objects[index].SetActive(true);
        }
    }

    void OnDestroy()
    {
        if (dropdown != null)
        {
            dropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);
        }
    }
}
