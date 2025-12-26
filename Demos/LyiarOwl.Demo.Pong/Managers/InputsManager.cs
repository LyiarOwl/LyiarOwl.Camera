using MonoGame.Extended.Input;

namespace LyiarOwl.Demo.Pong.Managers
{
    public class InputsManager
    {
        public static InputsManager Instance { get; private set; }
        public KeyboardStateExtended Keyboard;
        public InputsManager()
        {
            Instance = this;
        }
        public void Update()
        {
            KeyboardExtended.Update();
            Keyboard = KeyboardExtended.GetState();
        }
    }
}