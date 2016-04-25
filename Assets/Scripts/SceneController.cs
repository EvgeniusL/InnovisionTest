using UnityEngine;
using System.Collections;

namespace Innovision
{
    public abstract class SceneController : MonoBehaviour
    {
        #region Properties
        protected bool _initialized;
        #endregion

        #region Methods
        protected abstract void Init();
        protected abstract void Cycle();
        protected abstract void Destroy();
        #endregion

        #region Unity methods
        private void Start()
        {
            Init();
        }

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