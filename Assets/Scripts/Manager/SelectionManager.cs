using Assets.Scripts.Entity;
using Dan.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Manager
{
    /// <summary>
    /// Handle selected entity
    /// </summary>
    public class SelectionManager : Singleton<SelectionManager>
    {
        /// <summary>
        /// Effect to apply to selected entity
        /// </summary>
        [SerializeField]
        [LinkToScriptableObject(typeof(PrefabDescriptor), "Selection Circle")]
        private string SelectionCircleId;

        /// <summary>
        /// All selected entity
        /// </summary>
        private List<ISelectableEntity> _selected = new List<ISelectableEntity>();

        /// <summary>
        /// All selection circle effect used
        /// </summary>
        private Dictionary<ISelectableEntity,GameObject> _selectionCircles = new Dictionary<ISelectableEntity,GameObject>();


        public void UnselectAll()
        {
            for (int i = _selected.Count()-1; i >= 0; i--)
            {
                ProcessEntityUnselect(_selected[i]);
            }
        }

        public void Unselect(ISelectableEntity entity)
        {
            ProcessEntityUnselect(entity);
        }

        public void UnselectAllBut(ISelectableEntity butThis)
        {
            var entityToUnselect = _selected.Where(x => x != butThis).ToList();
            for(int i = entityToUnselect.Count() - 1; i >= 0; i--)
            {
                ProcessEntityUnselect(entityToUnselect[i]);
            }
        }

        public void Select(ISelectableEntity entity)
        {
            if(_selected.Contains(entity))
            {
                if (_selected.Count() > 1)
                {
                    UnselectAllBut(entity);
                }
                else
                {
                    Unselect(entity);
                }
            }
            else
            {
                UnselectAll();
                ProcessEntitySelect(entity);
            }
        }

        private void ProcessEntityUnselect(ISelectableEntity entity)
        {
            if (_selectionCircles.ContainsKey(entity))
            {
                Destroy(_selectionCircles[entity]);
                _selectionCircles.Remove(entity);
            }
            entity.Unselect();
            _selected.Remove(entity);
        }

        private void ProcessEntitySelect(ISelectableEntity entity)
        {
            if(_selectionCircles.ContainsKey(entity) == false)
            {
                var selectionCircle = Instantiate<GameObject>(
                    PrefabManager.Instance.GetPrefab(SelectionCircleId),
                    Vector3.zero,
                    Quaternion.identity,
                    entity.GetTransform()
                    );
                selectionCircle.transform.localPosition = Vector3.zero;
                _selectionCircles.Add(entity, selectionCircle);
            }
            UIManager.Instance.WikiUIBehaviour.SetWikiInformations((entity as EntityMonoBehaviour)?.ElementId);
            UIManager.Instance.WikiUIBehaviour.UIWindowBehaviour.OpenWindow();
            _selected.Add(entity);
            entity.Select();
        }

    }
}
