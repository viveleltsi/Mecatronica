using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Dan.SceneSmoothLoader.Utils
{
    public class WatchTimer
    {
        /// <summary>
        /// TimeLimit
        /// </summary>
        private long _tickLimit;

        private double _timeLimit;

        private DateTime _startDate;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="timeLimit"></param>
        private WatchTimer(double timeLimit)
        {
            _startDate = DateTime.Now;
            var endDate = _startDate.AddMilliseconds(timeLimit);
            _tickLimit = endDate.Ticks;
            _timeLimit = timeLimit;
        }

        /// <summary>
        /// Create a watch timer
        /// </summary>
        /// <param name="timeLimit"></param>
        /// <returns></returns>
        public static WatchTimer CreateNew(double timeLimit)
        {
            return new WatchTimer(timeLimit);
        }

        /// <summary>
        /// Do we have time
        /// </summary>
        /// <returns></returns>
        public bool HaveTime()
        {
            //var timeUsed = (DateTime.Now - _startDate).TotalMilliseconds;
            if (_tickLimit > DateTime.Now.Ticks)
                return true;
            else
            {
                return false;
            }
                
        }

        public double TimeSinceStart()
        {
            return (DateTime.Now - _startDate).Ticks;
        }

        public void Log(string text)
        {
            //Debug.Log($"{GetElapsedTime()} - {text}" + Environment.NewLine);
        }
    }
}
