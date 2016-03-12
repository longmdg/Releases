namespace STAutoPlay
{
    using System;
    using System.Reflection.Emit;

    using LeagueSharp;
    using LeagueSharp.Common;

    public abstract class Core
    {
        internal static readonly Champion Instance;

        public static readonly Menu Menu = new Menu("[ST] AutoPlay", "st_ap", true);

        protected static readonly Obj_AI_Hero Player = ObjectManager.Player;

        protected static readonly string ChampionName = Player.ChampionName;

        static Core()
        {
            Menu.AddToMainMenu();

            var type = Type.GetType($"STAutoPlay.Champions.{ChampionName}");

            if (type == null)
                return;

            var target = type.GetConstructor(Type.EmptyTypes);
            var deType = target?.DeclaringType;

            if (deType == null)
                return;

            var dynamic = new DynamicMethod(string.Empty, type, new Type[0], deType);
            var il = dynamic.GetILGenerator();

            il.DeclareLocal(deType);
            il.Emit(OpCodes.Newobj, target);
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ret);

            Instance = (dynamic.CreateDelegate(typeof(Func<Champion>)) as Func<Champion>)?.Invoke();

            Message.Say(new Message($"while test ({Message.Randomxd}"));

            byte a = 200;

            while (a-- != 0)
            {
                Message.Say(new Message($"while test ({Message.Randomxd}): {a}"));
            }
        }

        protected virtual void OnStart()
        {

        }

        protected static void OnStartNative(EventArgs args)
        {
            Utility.DelayAction.Add(2000, Instance.OnStart);
        }

        protected static void OnLoadNative(EventArgs args)
        {
            if (Instance == null)
            {
                Game.PrintChat($"[ST] AutoPlay - Couldn't load champion extension for {ChampionName}");
                Menu.AddItem(new MenuItem("st_error", $"{ChampionName} is not supported ATM!"));
                return;
            }

            Instance.OnLoad();
        }

        internal virtual bool AllowSpeaking
        {
            get
            {
                return true;
                //  return ModeState != Mode.Attacking && ModeState != Mode.Defending;
            }
        }

        protected static Mode ModeState
        {
            get
            {
                if (Player.InFountain() && (Player.HealthPercent < 0.75f || Player.ManaPercent < 0.75f && Player.MaxMana > 100))
                {
                    return Mode.Waiting;
                }

          //      if (Utility.CountEnemiesInRange(2000) < 2 && )
                {
                    
                }


                return default(Mode);
            }
        }
    }
}
