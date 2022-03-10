using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using UnityEngine;
using VRC;
using VRC.Core;
using VRChatUtilityKit.Utilities;
using UIExpansionKit.API;
using VRC.SDK3.Video.Components.AVPro;
using VRC.SDK3.Components;
using VRC.SDKBase;
using System.Reflection;
using VRC.Udon;
using VRC.SDK3.Video.Components;

namespace VRCClipboard
{
    public class VRCClipboard : MelonMod
    {
        public override void OnApplicationStart()
        {
            ExpansionKitApi.GetExpandedMenu(ExpandedMenu.QuickMenu).AddSimpleButton("Video URL", ShowVideoMenu);
            ExpansionKitApi.GetExpandedMenu(ExpandedMenu.QuickMenu).AddSimpleButton("Play/Pause", PlayPause);

            LoggerInstance.Msg("Initialized!");
        }

        private void ShowVideoMenu()
        {
            VRCUrlInputField[] foundInputFields = GameObject.FindObjectsOfType<VRCUrlInputField>();

            if(foundInputFields.Length == 0)
            {
                LoggerInstance.Msg("No InputField found!");
            }
            else if (foundInputFields.Length == 1)
            {
                VRCUrlInputField inputField = foundInputFields[0];

                VRCUrl url = new VRCUrl(Clipboard);
                inputField.SetUrl(url);
                inputField.onEndEdit.Invoke(inputField.text);
            }
            else
            {
                var menu = ExpansionKitApi.CreateCustomQuickMenuPage(LayoutDescription.QuickMenu4Columns);

                foreach (VRCUrlInputField inputField in foundInputFields)
                {
                    LoggerInstance.Msg("Found InputField: " + inputField.name);

                    menu.AddSimpleButton(inputField.name, () => {
                        VRCUrl url = new VRCUrl(Clipboard);
                        inputField.SetUrl(url);
                        inputField.onEndEdit.Invoke(inputField.text);
                    });
                }

                menu.Show();
            }
        }

        private void PlayPause()
        {
            string playPauseFunction = "OnPlayButtonPress";

            UdonBehaviour[] foundUdonBehaviours = GameObject.FindObjectsOfType<UdonBehaviour>();
            List<UdonBehaviour> foundPlayerUdonBehaviours = new List<UdonBehaviour>();

            foreach (UdonBehaviour udonBehaviour in foundUdonBehaviours)
            {
                foreach (string eventName in udonBehaviour.GetPrograms())
                {
                    if(eventName == playPauseFunction)
                    {
                        foundPlayerUdonBehaviours.Add(udonBehaviour);
                        //udonBehaviour.SendCustomEvent(playPauseFunction);
                        //return;
                    }
                }
            }

            if (foundPlayerUdonBehaviours.Count == 0)
            {
                LoggerInstance.Msg("No Player Buttons found!");
            }
            else if (foundPlayerUdonBehaviours.Count == 1)
            {
                UdonBehaviour udonBehaviour = foundPlayerUdonBehaviours.First();
                LoggerInstance.Msg("Found Button: " + udonBehaviour.name);
                udonBehaviour.SendCustomEvent(playPauseFunction);
            }
            else
            {
                var menu = ExpansionKitApi.CreateCustomQuickMenuPage(LayoutDescription.QuickMenu4Columns);

                foreach (UdonBehaviour udonBehaviour in foundPlayerUdonBehaviours)
                {
                    LoggerInstance.Msg("Found Button: " + udonBehaviour.name);

                    menu.AddSimpleButton(udonBehaviour.name, () => {
                        udonBehaviour.SendCustomEvent(playPauseFunction);
                    });
                }

                menu.Show();
            }
        }

        public static string Clipboard
        {
            get { return GUIUtility.systemCopyBuffer; }
            set { GUIUtility.systemCopyBuffer = value; }
        }
    }
}
