using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Entity.Building
{
    /// <summary>
    /// Interface for all placeholder type
    /// </summary>
    public interface IPlaceholder
    {
        /// <summary>
        /// Assign the building to construct
        /// </summary>
        /// <param name="buildingId"></param>
        void SetBuildingToConstruct(string buildingId);
    }
}
