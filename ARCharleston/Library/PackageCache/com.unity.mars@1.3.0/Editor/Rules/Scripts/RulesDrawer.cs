using UnityEditor;
using UnityEditor.MARS.Simulation;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Unity.MARS.Rules.RulesEditor
{
    static class RulesDrawer
    {
        const string k_Directory = "Packages/com.unity.mars/Editor/Rules/Scripts/";
        const string k_WindowXMLPath = k_Directory + "RulesWindow.uxml";
        const string k_WindowStylePath = k_Directory + "RulesWindow.uss";

        const string k_AddRuleName = "addRule";
        const string k_RootClassName = "root";
        const string k_LightModeClassName = "light-mode";
        const string k_DarkModeClassName = "dark-mode";
        const string k_RulesAreaName = "rulesArea";
        const string k_SessionButtonName = "session";
        const string k_RebuildButtonName = "rebuild";
        const string k_DrillUpButtonName = "drillUp";
        const string k_GroupSettingsButtonName = "groupSettings";

        const string k_AddRuleButtonAnalyticsLabel = "Add Rule Button";
        const string k_GroupSettingsButtonAnalyticsLabel = "Group settings button";
        const string k_DeleteKeyAnalyticsLabel = "Delete rule with keyboard";
        const string k_AddKeyAnalyticsLabel = "Add rule with keyboard";
        const string k_SelectPreviousAnalyticsLabel = "Select previous with keyboard";
        const string k_SelectNextAnalyticsLabel = "Select next with keyboard";

        static VisualElement s_RootVisualElement;
        static VisualElement s_RulesContainer;
        static Button s_AddRuleButton;
        static Button s_GroupSettingsButton;
        static Button s_DrillUpButton;

        internal static void Init(VisualElement ve)
        {
            if (!MARSEntitlements.instance.IsEntitled())
                return;

            s_RootVisualElement = ve;

            RulesModule.OnRuleAdded += (replicator, proxy) => BuildReplicatorsList();
            QuerySimulationModule.simulationDone += BuildReplicatorsList;
            RulesModule.OnObjectPicked += BuildReplicatorsList;
            ActionsDropdownItem.ActionCreated = BuildReplicatorsList;
            Undo.undoRedoPerformed += BuildReplicatorsList;

            SetupUI();
        }

        static void SetupUI()
        {
            var windowXml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_WindowXMLPath);
            windowXml.CloneTree(s_RootVisualElement);

            s_AddRuleButton = s_RootVisualElement.Q<Button>(k_AddRuleName);
            s_AddRuleButton.clicked += AddRuleButton;

            s_AddRuleButton.RegisterCallback<ExecuteCommandEvent>(RulesModule.PickObject);

            var windowStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>(k_WindowStylePath);
            s_RootVisualElement.styleSheets.Add(windowStyle);
            s_RootVisualElement.AddToClassList(k_RootClassName);

            s_RootVisualElement.AddToClassList(EditorGUIUtility.isProSkin ? k_DarkModeClassName : k_LightModeClassName);

            s_RulesContainer = s_RootVisualElement.Q<VisualElement>(k_RulesAreaName);

            var sessionButton = s_RootVisualElement.Q<Button>(k_SessionButtonName);
            sessionButton.clicked += () => RulesModule.SetSelectedComponent(MARSSession.Instance);

            s_RootVisualElement.Q<Button>(k_RebuildButtonName).clicked += () =>
            {
                s_RootVisualElement.Clear();
                SetupUI();
            };

            s_DrillUpButton = s_RootVisualElement.Q<Button>(k_DrillUpButtonName);
            s_DrillUpButton.clicked += () =>
            {
                RulesModule.RuleSetInstance = RulesModule.ParentRuleSet;
                BuildReplicatorsList();
            };

            s_GroupSettingsButton = s_RootVisualElement.Q<Button>(k_GroupSettingsButtonName);
            s_GroupSettingsButton.clicked += () =>
            {
                var group = RulesModule.RuleSetInstance.GetComponent<ProxyGroup>();
                Selection.activeGameObject = group.gameObject;
                group.transform.hideFlags = HideFlags.HideInInspector;

                EditorEvents.RulesUiUsed.Send(new RuleUiArgs
                {
                    label = k_GroupSettingsButtonAnalyticsLabel
                });
            };

            BuildReplicatorsList();
        }

        static void AddRuleButton()
        {
            RulesModule.DoAddRule();

            EditorEvents.RulesUiUsed.Send(new RuleUiArgs
            {
                label = k_AddRuleButtonAnalyticsLabel
            });
        }

        static void UpdateGroupButtons()
        {
            RulesModule.CollectRuleSceneObjects();

            s_DrillUpButton.SetEnabled(RulesModule.ParentRuleSet != null);

            s_GroupSettingsButton.SetEnabled(RulesModule.RuleSetInstanceExisting != null &&
                                             RulesModule.RuleSetInstanceExisting.GetComponent<ProxyGroup>());

            s_AddRuleButton.style.display =
                RulesModule.ParentRuleSet == null
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;
        }

        internal static void BuildReplicatorsList()
        {
            if (s_RulesContainer == null)
                return;

            s_RulesContainer.Clear();
            RuleNodeAddBar.ClearAll();
            RuleNode.LastCreatedRuleNode = null;

            for (var sceneIdx = 0; sceneIdx < SceneManager.sceneCount; sceneIdx++)
            {
                var existingRuleset = RulesModule.RuleSetInstanceExisting;
                if (!existingRuleset)
                    continue;

                foreach (Transform child in existingRuleset.transform)
                {
                    var replicator = child.GetComponent<Replicator>();
                    var proxy = child.GetComponent<Proxy>();

                    RuleNode ruleRow;

                    if (replicator != null)
                        ruleRow = new ReplicatorRow(replicator);
                    else if (proxy != null)
                        ruleRow = new ProxyRow(proxy);
                    else
                        return;

                    s_RulesContainer.Add(ruleRow);
                }
            }

            UpdateGroupButtons();
            RuleNodeAddBar.CreateAddBarsAtLocations(s_RulesContainer);

            var groupDisplay = RulesModule.ParentRuleSet == null ? DisplayStyle.None : DisplayStyle.Flex;
            s_DrillUpButton.style.display = groupDisplay;
            s_GroupSettingsButton.style.display = groupDisplay;
        }

        internal static void OnGUI()
        {
            RulesModule.ReParentNewObject();

            // Picker must be invoked from OnGUI for Event.current.commandName to be valid
            RulesModule.HandlePicker();

            HandleInput();
        }

        static void HandleInput()
        {
            var evt = Event.current;

            if (evt.type != EventType.KeyDown)
                return;

            if (evt.keyCode == KeyCode.Delete || evt.keyCode == KeyCode.Backspace)
            {
                RulesModule.DeleteSelectedRuleNode();

                EditorEvents.RulesUiUsed.Send(new RuleUiArgs
                {
                    label = k_DeleteKeyAnalyticsLabel
                });
            }
            else if (evt.keyCode == KeyCode.Space)
            {
                var targetNode = RuleNode.HoveredAddBarNode ?? RuleNode.SelectedNode;
                RulesModule.AddNode(targetNode);

                EditorEvents.RulesUiUsed.Send(new RuleUiArgs
                {
                    label = k_AddKeyAnalyticsLabel
                });
            }
            else if (evt.keyCode == KeyCode.UpArrow)
            {
                RuleNode.SelectPrevious();

                EditorEvents.RulesUiUsed.Send(new RuleUiArgs
                {
                    label = k_SelectPreviousAnalyticsLabel
                });
            }
            else if (evt.keyCode == KeyCode.DownArrow)
            {
                RuleNode.SelectNext();

                EditorEvents.RulesUiUsed.Send(new RuleUiArgs
                {
                    label = k_SelectNextAnalyticsLabel
                });
            }
        }
    }
}
