using TMPro;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class ConnectionUI : MonoBehaviour
{
    public ConnectionController connectionController;

    public GameObject connectUi;
    public GameObject connectingUi;
    public GameObject connectedUi;

    enum UIToDisplay
    {
        Connect,
        Connecting,
        Connected
    }

    void Update()
    {
        if (connectionController.isConnecting)
        {
            SetActiveUI(UIToDisplay.Connecting);
        }
        else if (connectionController.connected)
        {
            SetActiveUI(UIToDisplay.Connected);
        }
        else
        {
            SetActiveUI(UIToDisplay.Connect);
        }
    }

    void SetActiveUI(UIToDisplay uIToDisplay)
    {
        connectUi.SetActive(uIToDisplay == UIToDisplay.Connect);
        connectingUi.SetActive(uIToDisplay == UIToDisplay.Connecting);
        connectedUi.SetActive(uIToDisplay == UIToDisplay.Connected);
    }
}
