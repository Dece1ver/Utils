using Utils.WinAPI.pInvoke;

namespace Utils.WinAPI.Windows
{
    public class Keyboard
    {

        private const int KeyeventfExtendedkey = 1;
        private const int KeyeventfKeyup = 2;

        public static void KeyDown(Keys vKey)
        {
            User32.keybd_event((byte)vKey, 0, KeyeventfExtendedkey, 0);
        }

        public static void KeyUp(Keys vKey)
        {
            User32.keybd_event((byte)vKey, 0, KeyeventfExtendedkey | KeyeventfKeyup, 0);
        }

        public static void KeyPress(Keys vKey)
        {
            KeyDown(vKey);
            KeyUp(vKey);
        }

    }
}
