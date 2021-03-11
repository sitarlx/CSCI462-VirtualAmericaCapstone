using System;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEditor.MARS.Data
{
    [MovedFrom("Unity.MARS.Providers")]
    public static class GeoLocationShortcutButtons
    {
        public static bool DrawShortcutButtons(string title, Action<double, double> shortcutAction,
            bool useFoldout = false, bool isOpen = true)
        {
            using (new EditorGUILayout.VerticalScope())
            {
                if (useFoldout)
                    isOpen = EditorGUILayout.Foldout(isOpen, title);
                else
                    EditorGUILayout.LabelField(title, EditorStyles.boldLabel);

                if (!useFoldout || isOpen)
                {
                    foreach (var shortcutButton in MarsUserGeoLocations.instance.UserGeoLocations)
                    {
                        if (GUILayout.Button(shortcutButton.Name))
                        {
                            shortcutAction(shortcutButton.Location.latitude, shortcutButton.Location.longitude);
                        }
                    }
                }
            }

            return isOpen;
        }
    }
}
