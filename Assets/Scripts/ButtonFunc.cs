using UnityEngine;

public class ButtonFunc : MonoBehaviour
{
    [SerializeField] private GameControll _gameControll;
    
    public void StartButton()
    {
        _gameControll.newGameButton = true;
        _gameControll.startButton = true;
    }

    public void MenuButton()
    {
        _gameControll.startButton = false;
    }
    
    public void ResumeButton()
    {
        _gameControll.startButton = true;
    }
    public void ExitButton()
    {
        Application.Quit();
    }
}
