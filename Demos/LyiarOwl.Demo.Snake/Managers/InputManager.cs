using MonoGame.Extended.Input;

namespace LyiarOwl.Demo.Snake.Managers;

public class InputManager
{
    public static InputManager Instance { get; private set; }
    public KeyboardStateExtended Keyboard;
    public InputManager()
    {
        Instance = this;
    }
    public void Update()
    {
        KeyboardExtended.Update();
        this.Keyboard = KeyboardExtended.GetState();
    }
}