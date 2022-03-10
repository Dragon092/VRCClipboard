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
using UnityEngine.UI;

namespace VRCClipboard
{
    public class VRCClipboard : MelonMod
    {
        public override void OnApplicationStart()
        {
            ExpansionKitApi.GetExpandedMenu(ExpandedMenu.QuickMenu).AddSimpleButton("Play URL", ShowVideoMenu);
            ExpansionKitApi.GetExpandedMenu(ExpandedMenu.QuickMenu).AddSimpleButton("Play/Pause", PlayPause);

            LoggerInstance.Msg("Initialized!");
        }

        private void ShowVideoMenu()
        {
            VRCUrlInputField[] foundUrlInputFields = GameObject.FindObjectsOfType<VRCUrlInputField>();
            InputField[] foundInputFields = GameObject.FindObjectsOfType<InputField>();

            if (foundUrlInputFields.Length == 1 && foundInputFields.Length == 0)
            {
                VRCUrlInputField inputField = foundUrlInputFields[0];

                LoggerInstance.Msg("Found VRCUrlInputField: " + inputField.name);

                HandleVRCUrlInputField(inputField);
            }
            else if(foundUrlInputFields.Length > 1 || foundInputFields.Length > 0)
            {
                var menu = ExpansionKitApi.CreateCustomQuickMenuPage(LayoutDescription.QuickMenu4Columns);

                foreach (VRCUrlInputField inputField in foundUrlInputFields)
                {
                    LoggerInstance.Msg("Found VRCUrlInputField: " + inputField.name);

                    menu.AddSimpleButton(inputField.name, () => {
                        HandleVRCUrlInputField(inputField);
                    });
                }

                foreach (InputField inputField in foundInputFields)
                {
                    LoggerInstance.Msg("Found InputField: " + inputField.name);

                    menu.AddSimpleButton(inputField.name, () => {
                        HandleInputField(inputField);
                    });
                }

                menu.Show();
            }
            else
            {
                LoggerInstance.Msg("No VRCUrlInputField or InputField found!");
            }
        }

        private void HandleVRCUrlInputField(VRCUrlInputField inputField)
        {
            VRCUrl url = new VRCUrl(Clipboard);
            inputField.SetUrl(url);
            inputField.onEndEdit.Invoke(inputField.text);
        }

        private void HandleInputField(InputField inputField)
        {
            inputField.text = Clipboard;
            inputField.onEndEdit.Invoke(inputField.text);
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
