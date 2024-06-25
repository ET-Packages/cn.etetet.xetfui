
using System;
using FairyGUI;
using UnityEngine;

namespace ET.Client
{
    public static partial class FUIComponentSystem
    {
        public static void AllPanelTranslateText(this FUIComponent self, SystemLanguage currentLanguage, Func<string, string, string> translator)
        {
            foreach (var kv in self.IdToEntity)
            {
                self.OnePanelTranslateText(currentLanguage, kv.Value, translator);
            }
        }

        private static void OnePanelTranslateText(this FUIComponent self, SystemLanguage currentLanguage, FUIEntity fuiEntity, Func<string, string, string> translator)
        {
            if (fuiEntity == null || fuiEntity.IsDisposed)
            {
                return;
            }

            self.TranslateComponent(fuiEntity.GComponent, translator);

            fuiEntity.Language = currentLanguage;
        }
        
        private static void TranslateComponent(this FUIComponent self, GComponent component, Func<string, string, string> translator)
        {
            int n = component.numChildren;
            for (int i = 0; i < n; i++)
            {
                GObject child = component.GetChildAt(i);
                switch (child)
                {
                    case GTextField textField:
                    {
                        if (textField.parent?.resourceURL != null)
                        {
                            string key = $"{textField.parent.resourceURL[5..]}-{textField.id}";
                            textField.text = translator(key, textField.text);
                        }
                    }
                    break;

                    case GButton button:
                    {
                        if (button.parent?.resourceURL != null)
                        {
                            string key = $"{button.parent.resourceURL[5..]}-{button.id}";
                            button.title = translator(key, button.title);
                            button.selectedTitle = translator($"{key}-0", button.title);
                        }
                    }
                    break;

                    case GLabel label:
                    {
                        if (label.parent?.resourceURL != null)
                        {
                            string key = $"{label.parent.resourceURL[5..]}-{label.id}";
                            label.title = translator(key, label.title);
                        }
                    }
                    break;

                    case GComponent subComponent:
                    {
                        if (subComponent.resourceURL != null)
                        {
                            self.TranslateComponent(subComponent, translator);
                        }
                    }
                    break;
                }
            }
        }
    }
}