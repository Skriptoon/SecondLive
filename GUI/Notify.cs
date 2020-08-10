using GTANetworkAPI;

namespace SecondLive.GUI
{
    class Notify : Script
    {
        internal enum Type
        {
            Alert,
            Error,
            Success,
            Info,
            Warning
        }
        internal enum Position
        {
            Top,
            TopLeft,
            TopCenter,
            TopRight,
            Center,
            CenterLeft,
            CenterRight,
            Bottom,
            BottomLeft,
            BottomCenter,
            BottomRight
        }
        public static void Send(Player client, string msg, Type type = Type.Info, Position pos = Position.BottomCenter, int time = 5000)
        {
            Trigger.ClientEvent(client, "notify", type, pos, msg, time);
        }
    }
}
