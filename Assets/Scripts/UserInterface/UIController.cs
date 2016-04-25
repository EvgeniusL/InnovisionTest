using UnityEngine;
using System.Collections;

namespace Innovision.UserInterface
{
    public abstract class UIController : MonoBehaviour
    {
        #region Properties
        protected bool _initialized;
        #endregion

        #region Methods
        public abstract void Init();
        public abstract void Cycle();
        public abstract void Destroy();
        #endregion

        #region Unity methods
        private void Update()
        {
            if (_initialized)
                Cycle();
        }

        private void OnDisable()
        {
            Destroy();
        }
        #endregion
    }
}