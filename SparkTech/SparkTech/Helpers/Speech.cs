namespace SparkTech.Helpers
{
    using System;
    using System.Speech.Recognition;
    using System.Speech.Synthesis;

    using LeagueSharp.SDK;

    /// <summary>
    /// Helps you manage all the speech features the library offers :^)
    /// </summary>
    public static class Speech
    {
        private static readonly DictationGrammar Grammar = new DictationGrammar();

        /// <summary>
        /// Says the message aloud using the speech synthesizer
        /// </summary>
        /// <param name="message">Text that will be spoken</param>
        public static void Announce(string message)
        {
            try
            {
                using (var synth = new SpeechSynthesizer())
                {
                    synth.SetOutputToDefaultAudioDevice();
                    synth.Speak(message);
                }
            }
            catch (Exception ex)
            {
                Settings.Logger.Catch(ex);
            }
        }

        /// <summary>
        /// Pretty straightforward and simple usage.
        /// Just remember that it will return null if speech recognition is turned off in the menu or if the recognition failed
        /// </summary>
        public static string RecognizedSpeech
        {
            get
            {
                string myMessage = null;
                var myNotif = new Notification(LanguageData.GetTranslation("speak_now"));
                Notifications.Add(myNotif);

                using (var recognizer = new SpeechRecognitionEngine())
                {
                    try
                    {
                        recognizer.LoadGrammar(Grammar);
                        recognizer.SetInputToDefaultAudioDevice();
                        myMessage = recognizer.Recognize().Text;
                    }
                    catch (Exception ex)
                    {
                        ex.Catch();
                    }
                    finally
                    {
                        recognizer.UnloadAllGrammars();
                    }
                }
                return myMessage;
            }
        }

        #region Trash

        /*
private static string myMessage;

static Speech()
{
Game.OnInput += args =>
{
bool swNew = args.Input.StartsWith(@"/new");
bool swSend = args.Input.StartsWith(@"/send");
bool swRemove = args.Input.StartsWith(@"/clear");

if (!swNew && !swSend && !swRemove)
{
    return;
}

args.Process = false;

//TODO: Make a check here - if some menu item is active, then return.

if (swRemove)
{
    myMessage = null;
}
else if (swNew)
{
    myMessage = null;

    using (var recognizer = new SpeechRecognitionEngine())
    {
        try
        {
            recognizer.LoadGrammar(new DictationGrammar());
            recognizer.SetInputToDefaultAudioDevice();
            myMessage = recognizer.Recognize().Text;
            Game.PrintChat("Type /send to send or /remove to decline that :");
            Game.PrintChat(myMessage);
        }
        catch (Exception ex)
        {
            Logger.Catch(ex);
        }
        finally
        {
            recognizer.UnloadAllGrammars();
        }
    }
}
else
{
    if (myMessage == null)
    {
        return;
    }
    if (args.Input.Contains("all"))
    {
        Game.Say("/all " + myMessage);
    }
    else
    {
        Game.Say(myMessage);
    }
}
};
}
*/

        #endregion

    }
}