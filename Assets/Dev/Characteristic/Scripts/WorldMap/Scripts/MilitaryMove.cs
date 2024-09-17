using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreData.UniFlow.Common
{
    public class MilitaryMove : MonoSingleton<MilitaryMove>
    {
        [SerializeField]
        private Transform browMove;

        Vector3 startPos;
        Vector3 endPos;

        private bool isMove = false;
        private Action _onComplete;
        private Transform militaryMove;
        private float speed = 0.005f;
        private List<PlayerActionMove> PlayerActionMoves = new List<PlayerActionMove>();
        public void MoveMilitary(Vector3 begin , Vector3 target , Transform military, Action onComplete)
        {
            startPos = begin;
            endPos = target;
            isMove = true;
            militaryMove = military;
            _onComplete = onComplete;
        }
        protected void Update()
        {
            if (!isMove)
                return;
            // Move our position a step closer to the target.
            var step = speed * Time.deltaTime; // calculate distance to move
            militaryMove.transform.localPosition = Vector3.MoveTowards(militaryMove.transform.localPosition, endPos, step);
            // Check if the position of the cube and sphere are approximately equal.
            if (militaryMove.transform.localPosition == endPos)
            {
                isMove = false;
                // Swap the position of the cylinder.
                endPos *= -1.0f;
                _onComplete?.Invoke();
            }
        }
    }
}