using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] Button startButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void StartGame()
    {
        print("game start");
    }
}
