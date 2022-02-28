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

        public static string Clipboard
        {
            get { return GUIUtility.systemCopyBuffer; }
            set { GUIUtility.systemCopyBuffer = value; }
        }
    }
}
