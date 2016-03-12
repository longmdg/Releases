namespace STAutoPlay.Champions
{
    using System.Timers;

    public class Annie : Champion
    {
        private static uint i;

        public override void OnLoad()
        {
            new Timer(2000d) { Enabled = true }.Elapsed += delegate
            {
                Message.Say(new Message($"Annie has spoken {++i} times"));
            };
        }
    }
}