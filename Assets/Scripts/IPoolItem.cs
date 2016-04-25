using UnityEngine;
using System.Collections;

namespace Innovision.Helpers
{
    public interface IPoolItem
    {
        #region Methods
        void Init();
        void Activate(Vector3 position);
        void Deactivate();
        #endregion
    }
}