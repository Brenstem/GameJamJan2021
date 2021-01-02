using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DanesUnityLibrary
{
    /// <summary>
    /// Simple timer class with optional action for when the timer runs out
    /// </summary>
    public class Timer
    {
        private float startTime;
        private float currentTime;
        private bool expired;
        public bool Expired { get { return expired; } }
        private Action timerFinishAction;

        public Timer(float time, Action timerFinishAction = null)
        {
            startTime = time;
            currentTime = startTime;
            this.timerFinishAction = timerFinishAction;
        }

        /// <summary>
        /// Call this method on tick
        /// </summary>
        /// <param name="decrement"></param>
        public void UpdateTimer(float decrement)
        {
            currentTime -= decrement;

            if (currentTime <= 0)
            {
                expired = true;

                if (timerFinishAction != null) 
                    timerFinishAction();
            }
        }

        /// <summary>
        /// Resets the timer with latest time
        /// </summary>
        public void Reset()
        {
            currentTime = startTime;
            expired = false;
        }

        /// <summary>
        /// Resets the timer with a new time
        /// </summary>
        /// <param name="time"></param>
        public void Reset(float time)
        {
            startTime = time;
            currentTime = time;
            expired = false;
        }
    }

}
