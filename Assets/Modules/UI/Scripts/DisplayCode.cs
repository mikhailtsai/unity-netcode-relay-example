using TMPro;
using UnityEngine;

public class DisplayCode : MonoBehaviour
{
    public ConnectionController connectionController;

    private TMP_Text tmpText;

    void Start()
    {
        tmpText = GetComponent<TMP_Text>();
    }

    void FixedUpdate()
    {
        tmpText.text = $"Session code: {connectionController.code}";
    }
}
