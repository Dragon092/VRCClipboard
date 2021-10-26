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
    public class MyMod : MelonMod
    {
        public override void OnApplicationStart()
        {
            ExpansionKitApi.GetExpandedMenu(ExpandedMenu.QuickMenu).AddSimpleButton("Video URL", ShowVideoMenu);

            //ExpansionKitApi.GetExpandedMenu(ExpandedMenu.QuickMenu).AddSimpleButton("Get Video", () => getVideoURL());
            //ExpansionKitApi.GetExpandedMenu(ExpandedMenu.QuickMenu).AddSimpleButton("Get Video Old", () => getVideoURLOld());

            MelonLogger.Msg("Initialized!");
        }

        private void ShowVideoMenu()
        {
            VRCUrlInputField[] foundInputFields = GameObject.FindObjectsOfType<VRCUrlInputField>();

            if(foundInputFields.Length == 0)
            {
                MelonLogger.Msg("No InputField found!");
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
                    MelonLogger.Msg("Found InputField: " + inputField.name);

                    menu.AddSimpleButton(inputField.name, () => {
                        VRCUrl url = new VRCUrl(Clipboard);
                        inputField.SetUrl(url);
                        inputField.onEndEdit.Invoke(inputField.text);
                    });
                }

                menu.Show();
            }
        }

            private static void loadVideo()
        {
            VRCUrlInputField[] foundInputFields = GameObject.FindObjectsOfType<VRCUrlInputField>();

            foreach (VRCUrlInputField inputField in foundInputFields)
            {
                MelonLogger.Msg("Found InputField: "+inputField.name);

                VRCUrl url = new VRCUrl(Clipboard);
                inputField.SetUrl(url);
                inputField.onEndEdit.Invoke(inputField.text);
            }
            }

        
        public static string Clipboard
        {
            get { return GUIUtility.systemCopyBuffer; }
            set { GUIUtility.systemCopyBuffer = value; }
        }
    }
}
