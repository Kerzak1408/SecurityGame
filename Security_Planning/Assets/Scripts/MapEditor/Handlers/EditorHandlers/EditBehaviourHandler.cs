using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.DataHandlers;
using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Entities.Characters.Goals;
using Assets.Scripts.Extensions;
using Assets.Scripts.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.MapEditor.Handlers.EditorHandlers
{
    public class EditBehaviourHandler : BaseUserSelectableHandler
    {
        private readonly GameObject panelEditBehaviour;
        private readonly Transform togglesParent;
        private readonly Dictionary<EdgeType, Toggle> tileEdgeToggles;
        private readonly Dictionary<PlanningEdgeType, Toggle> planningEdgeToggles;
        private Burglar selectedBurglar;

        public Dictionary<Toggle, bool> PreviousValues { get; private set; }
        public float PreviousSliderValue { get; private set; }

        public EditBehaviourHandler(GridManager gridManager) : base(gridManager)
        {
            panelEditBehaviour = gridManager.PanelEditBehaviour;
            foreach (Transform child in panelEditBehaviour.transform)
            {
                if (child.gameObject.HasScriptOfType<GridLayoutGroup>())
                {
                    togglesParent = child;
                }
            }
            tileEdgeToggles = new Dictionary<EdgeType, Toggle>();
            planningEdgeToggles = new Dictionary<PlanningEdgeType, Toggle>();
            InstantiateToggles(EdgeTypeUtils.BannableEdgeType, tileEdgeToggles,
                burglar => burglar.Data.ForbiddenEdgeTypes, GetToggleText);
            InstantiateToggles(PlanningEdgeTypeUtils.BannableEdgeTypes, planningEdgeToggles,
                burglar => burglar.Data.ForbiddenPlanningEdgeTypes, GetToggleText);
            PreviousValues = new Dictionary<Toggle, bool>();
        }

        private void InstantiateToggles<T>(IEnumerable<T> bannableEdgeTypes, Dictionary<T, Toggle> toggleDictionary,
            Func<BaseCharacter, List<T>> getForbiddenEdges, Func<T, string> toString, bool isOnIfContains = false)
        {
            foreach (T edgeType in bannableEdgeTypes)
            {
                GameObject toggleObject = GridManager.InstantiateGameObject(ResourcesHolder.Instance.BehaviourToggle);
                toggleObject.transform.parent = togglesParent;
                var toggle = toggleObject.GetComponent<Toggle>();
                toggle.GetComponentInChildren<Text>().text = toString(edgeType);
                toggleDictionary[edgeType] = toggle;

                toggle.onValueChanged.AddListener(isOn =>
                {
                    List<T> forbiddenEdges = getForbiddenEdges(selectedBurglar);
                    if (isOnIfContains)
                    {
                        isOn = !isOn;
                    }
                    if (isOn)
                    {
                        forbiddenEdges.Remove(edgeType);
                    }
                    else
                    {
                        if (!forbiddenEdges.Contains(edgeType))
                        {
                            forbiddenEdges.Add(edgeType);
                        }
                    }
                });
            }
        }

        private string GetToggleText(PlanningEdgeType planningEdgeType)
        {
            switch (planningEdgeType)
            {
                case PlanningEdgeType.CAMERA:
                    return "DESTROY CAMERAS";
                case PlanningEdgeType.PIR:
                    return "DESTROY PIR DETECTORS";
                default:
                    return "UNDEFINED";
            }
        }

        private string GetToggleText(EdgeType edgeType)
        {
            switch (edgeType)
            {
                case EdgeType.WINDOW:
                    return "BREAK WINDOWS";
                case EdgeType.FENCE:
                    return "CUT FENCES";
                default:
                    return "UNDEFINED";
            }
        }

        public override void Start()
        {
        }

        public override void LeftButton(RaycastHit[] raycastHits)
        {
            if (raycastHits.Any(HasScriptOfTypePredicate<Burglar>()))
            {
                RaycastHit burglarHit = raycastHits.First(HasScriptOfTypePredicate<Burglar>());
                selectedBurglar = burglarHit.transform.gameObject.GetComponent<Burglar>();
                InitializeTogglesValues(planningEdgeToggles, selectedBurglar.Data.ForbiddenPlanningEdgeTypes);
                InitializeTogglesValues(tileEdgeToggles, selectedBurglar.Data.ForbiddenEdgeTypes);
                panelEditBehaviour.SetActive(true);
                float loadedSliderValue = selectedBurglar.Data.MaxVisibilityMeasure * (NavigationGoal.PATHS_COUNT - 1);
                ChangeMaxVisibilityMeasure(loadedSliderValue, true);
                PreviousSliderValue = loadedSliderValue;
                GridManager.InputSensitivity.text = selectedBurglar.Data.Sensitivity.ToString();
            }
        }

        private void InitializeTogglesValues<T>(Dictionary<T, Toggle> dictionary, List<T> forbiddenEdgeTypes, 
            bool isOnIfContains=false)
        {
            foreach (KeyValuePair<T, Toggle> keyValuePair in dictionary)
            {
                T edgeType = keyValuePair.Key;
                Toggle toggle = keyValuePair.Value;
                bool isOn = !forbiddenEdgeTypes.Contains(edgeType);
                if (isOnIfContains)
                {
                    isOn = !isOn;
                }
                toggle.isOn = isOn;
                PreviousValues[toggle] = isOn;
            }
        }

        public void ChangeMaxVisibilityMeasure(float value, bool calledFromCode=false)
        {
            float newMeasure = value / (NavigationGoal.PATHS_COUNT - 1);
            GridManager.TextMaxVisibilityMeasure.text = newMeasure.ToString();
            GridManager.HandleMaxVisibilitySlider.color = GridManager.Colors[(int)value];
            selectedBurglar.Data.MaxVisibilityMeasure = newMeasure;
            if (calledFromCode)
            {
                GridManager.SliderMaxVisibility.value = value;
            }
        }

        public void ChangeSensitivity(int value)
        {
            selectedBurglar.Data.Sensitivity = value;
        }
    }
}
