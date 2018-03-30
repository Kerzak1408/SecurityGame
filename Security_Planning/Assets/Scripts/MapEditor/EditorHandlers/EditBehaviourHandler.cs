﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Extensions;
using Assets.Scripts.Model;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.MapEditor.EditorHandlers
{
    public class EditBehaviourHandler : BaseUserSelectableHandler
    {
        private GameObject panelEditBehaviour;
        private Transform togglesParent;
        private Dictionary<EdgeType, Toggle> tileEdgeToggles;
        private Dictionary<PlanningEdgeType, Toggle> planningEdgeToggles;
        private Burglar selectedBurglar;
        private bool toggleChangedFromCode;

        public Dictionary<Toggle, bool> PreviousValues;

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
            IntantiateToggles(EdgeTypeUtils.BannableEdgeType, tileEdgeToggles, burglar => burglar.Data.ForbiddenEdgeTypes, GetToggleText);
            IntantiateToggles(PlanningEdgeTypeUtils.BannableEdgeTypes, planningEdgeToggles, burglar => burglar.Data.ForbiddenPlanningEdgeTypes, GetToggleText);
            PreviousValues = new Dictionary<Toggle, bool>();
        }

        private void IntantiateToggles<T>(IEnumerable<T> bannableEdgeTypes, Dictionary<T, Toggle> toggleDictionary,
            Func<BaseCharacter, List<T>> getForbiddenEdges, Func<T, string> toString)
        {
            foreach (T edgeType in bannableEdgeTypes)
            {
                GameObject toggleObject = gridManager.InstantiateGameObject(ResourcesHolder.Instance.BehaviourToggle);
                toggleObject.transform.parent = togglesParent;
                var toggle = toggleObject.GetComponent<Toggle>();
                toggle.GetComponentInChildren<Text>().text = toString(edgeType);
                toggleDictionary[edgeType] = toggle;

                toggle.onValueChanged.AddListener(isOn =>
                {
                    List<T> forbiddenEdges = getForbiddenEdges(selectedBurglar);
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
            }
        }

        private void InitializeTogglesValues<T>(Dictionary<T, Toggle> dictionary, List<T> forbiddenEdgeTypes)
        {
            foreach (KeyValuePair<T, Toggle> keyValuePair in dictionary)
            {
                T edgeType = keyValuePair.Key;
                Toggle toggle = keyValuePair.Value;
                bool isOn = !forbiddenEdgeTypes.Contains(edgeType);
                toggle.isOn = isOn;
                PreviousValues[toggle] = isOn;
            }
        }


    }
}